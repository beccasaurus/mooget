// We'll eventually want to move certain complicated commands into their own 
// files but, for now, we put all commands in here ...

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet {

	// TODO i don't want these methods polluting the Moo class ... move them out!  OR rename them all to end with *Command, so it's not confusing
	public partial class Moo {

		[Command("Provide help on the 'moo' command")]
		public static void Help(string[] args) {
			ListCommands(args); // TODO add help
		}

		[Command("Print configuration information")]
		public static void Config(string[] args) {
			Console.WriteLine("mooDir: {0}", Moo.Dir);
		}

		[Command("Unpack a package into the current directory")]
		public static void Unpack(string[] args) {
			var package = Moo.Unpack(args[0]);
			Console.WriteLine("Unpacked {0}", package.IdAndVersion);
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

		[Command(Name = "commands", Description = "Lists all available Moo commands")]
		public static void ListCommands(string[] args) {
			Console.WriteLine("MOO commands:\n");
			foreach (var command in Commands)
				Console.WriteLine("    {0}{1}{2}", command.Name, Spaces(command.Name, 20), command.Description);
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
				sources.Add(Moo.OfficialNugetFeed); // should get user's saved sources ...

			foreach (var source in sources) {
				packages.AddRange(new Source(source).SearchByTitle(query));

			if (packages.Count == 0)
				Console.WriteLine("No packages matched: {0}", query);
			else
				foreach (var package in packages.OrderBy(p => p.Id))
					Console.WriteLine("{0} ({1})", package.Id, package.Version);
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

		// helper methods for getting spaces ... useful for commands ...
		static string Spaces(string str, int numSpaces) {
			string spaces = "";
			for (int i = 0; i < numSpaces - str.Length; i++)
				spaces += " ";
			return spaces;
		}
	}
}
