// TODO move everything out of here!  everything is being moved into src/Commands/ under the MooGet.Commands namespace!  classes should be called FooCommand

using System;
using System.IO;
using System.Linq;
using System.IO.Packaging;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet {

	// TODO i don't want these methods polluting the Moo class ... move them out!  OR rename them all to end with *Command, so it's not confusing
	public partial class Moo {

		[Command("Print configuration information")]
		public static void Config(string[] args) {
			Console.WriteLine("mooDir: {0}", Moo.Dir);
		}

		[Command("Unpack a package into the current directory")]
		public static void Unpack(string[] args) {
			var package = Moo.Unpack(args[0]);
			Console.WriteLine("Unpacked {0}", package.IdAndVersion);
		}

		[Command("Download a .nupkg to the current directory")]
		public static void FetchCommand(string[] args) {
			if (args.Length > 0) {
				var package = RemotePackage.FindLatestPackageByName(args[0]);
				if (package == null)
					Console.WriteLine("Package not found: {0}", args[0]);
				else {
					package.Fetch();
					Console.WriteLine("Downloaded {0}", package.Nupkg);
				}
			}
		}

		[Command("Display package details")]
		public static void ShowCommand(string[] args) {
			if (args.Length > 0) {
				var package = RemotePackage.FindLatestPackageByName(args[0]);
				if (package == null)
					Console.WriteLine("Package not found: {0}", args[0]);
				else
					Console.WriteLine(package.DetailString);
			}
		}

		[Command("Manage the list of NuGet feeds used to search/install packages")]
		public static void SourceCommand(string[] args) {
			var arguments = new List<string>(args);
			if (arguments.Count == 0) {
				Console.WriteLine("*** CURRENT SOURCES ***\n");
				foreach (var source in Moo.Sources)
					Console.WriteLine(source.Path);
				return;
			}

			var command = arguments.First(); arguments.RemoveAt(0);
			switch (command) {
				case "add":
					Source.AddSource(arguments.First()); break;
				case "rm":
				case "remove":
					Source.RemoveSource(arguments.First()); break;
				default:
					Console.WriteLine("Unknown source command: {0}", command); break;
			}
		}

		[Command("Displays list of installed packages")]
		public static void List(string[] args) {
			var packages = Packages;
			if (packages.Count == 0)
				Console.WriteLine("No installed packages");
			else {
				Console.WriteLine("Listing installed packages:");
				foreach (var package in packages)
					Console.WriteLine("{0} ({1})", package.Id, package.Version); // TODO list all installed versions
			}
		}

		[Command("Install a package into the local repository")]
		public static void Install(string[] args) {
			Install(args[0]);
		}

		[Command("Moo.")]
		public static void CowCommand(string[] args) {
			Cow.Say(string.Join(" ", args));
		}

		[Command("Search remote source for packages")]
		public static void Search(string[] args) {
			var packages = new List<RemotePackage>(); // TODO rename to RemotePackage ...
			var sources  = new List<string>();
			var opts     = new OptionSet() {
				{ "s|source=",  v => { if (v != null) sources.Add(v); }}
			};
			var extra = opts.Parse(args);
			var query = extra[0];

			if (sources.Count == 0)
				sources.AddRange(Moo.Sources.Select(src => src.Path).ToList());

			foreach (var source in sources)
				packages.AddRange(new Source(source).SearchByTitle(query));

			if (packages.Count == 0)
				Console.WriteLine("No packages matched: {0}", query);
			else {
				var packageIdsAndVersions = new Dictionary<string, List<PackageVersion>>();
				foreach (var package in packages.OrderBy(p => p.Id)) {
					if (! packageIdsAndVersions.ContainsKey(package.Id)) packageIdsAndVersions[package.Id] = new List<PackageVersion>();
					packageIdsAndVersions[package.Id].Add(package.Version);
				}
				foreach (var packageIdAndVersion in packageIdsAndVersions)
					Console.WriteLine("{0} ({1})", 
							packageIdAndVersion.Key, 
							string.Join(", ", packageIdAndVersion.Value.OrderBy(version => version).Select(version => version.ToString()).Reverse().ToArray()));
			}
		}

		[Command("Removes a package from the local repository")]
		public static void Uninstall(string[] args) {
			Uninstall(args[0]);
		}

		[Command("Alias for uninstall")]
		public static void Remove(string[] args) {
			Uninstall(args);
		}

		[Command("Generated a template")]
		public static void GenerateCommand(string[] args) {
			// TODO later, we'll make it so this is easy to extend.  for now, we just support generating nuspec files
			if (args[0] != "nuspec") {
				Console.WriteLine("Unknown template: {0}", args[0]);
				return;
			}

			var dirname     = Path.GetFileName(Directory.GetCurrentDirectory());
			var filename    = dirname + ".nuspec";
			var path        = Path.Combine(Directory.GetCurrentDirectory(), filename);
			var now         = Feed.Format(DateTime.Now);
			var version     = "1.0.0.0";
			var description = "";
			var author      = "me";
			var xml         = new XmlBuilder();

			xml.StartElement("package").
				StartElement("metadata").	
					WriteElement("id",          dirname).
					WriteElement("version",     version).
					WriteElement("description", description).
					StartElement("authors").
						WriteElement("author", author).
					EndElement().
					WriteElement("created",  now).
					WriteElement("modified", now).
				EndElement().
			EndElement();

			Util.WriteFile(path, xml.ToString());
			Console.WriteLine("Generated {0}", filename);
		}
	}
}
