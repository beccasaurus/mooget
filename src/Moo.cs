using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents the primary API for most MooGet actions</summary>
	public partial class Moo {

		/// <summary>Entry method</summary>
		public static void Main(string[] args) {
			if (args.Length == 0) {
				Cow.Say("NuGet + Super Cow Powers = MooGet");
				Console.WriteLine("\nRun moo help for help documentation");
				return;
			}

			if (args[0] == "-v" || args[0] == "--version")
				Console.WriteLine(Moo.Version);
			else
				FindAndRunCommand(args);
		}

		public static string OfficialNugetFeed = "http://go.microsoft.com/fwlink/?LinkID=199193";

		public static string UserAgent { get { return Moo.Version; } }

		public static string Version { get { return "Moo " + Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

		public static List<Source> DefaultSources = new List<Source> {
			new Source(Moo.OfficialNugetFeed)
		};

		public static List<Command> Commands = Command.GetCommands();

		public static void FindAndRunCommand(string[] args) {
			var arguments   = new List<string>(args);
			var commandName = arguments.First(); arguments.RemoveAt(0);
			var commands    = Commands.Where(c => c.Name.StartsWith(commandName)).ToList();

			if (commands.Count == 0)
				Console.WriteLine("Command not found: {0}\n\nCommands: {1}", commandName, string.Join("\n", Commands.Select(c => c.Name).ToArray()));
			else if (commands.Count == 1)
				commands.First().Run(arguments.ToArray());
			else
				Console.WriteLine("Ambiguous command '{0}'.  Did you mean one of these?  {1}", commandName, string.Join(", ", commands.Select(c => c.Name).ToArray()));
		}

		public static List<Source> Sources { get { return Source.GetSources(); } }
			
		public static LocalPackage Unpack(string nupkg) {
			return Unpack(nupkg, Directory.GetCurrentDirectory());
		}

		public static LocalPackage Unpack(string nupkg, string directoryToUnpackInto) {
			// TODO check if file exists
			// TODO check if nuspec exists
			// TODO nuspec parse error

			// unpack into a temp directory so we can read the spec and find out about the package
			var tempDir = Path.Combine(Util.TempDir, Path.GetFileNameWithoutExtension(nupkg));
			Util.Unzip(nupkg, tempDir);

			// find and load the .nuspec
			var nuspecPath = Directory.GetFiles(tempDir, "*.nuspec", SearchOption.TopDirectoryOnly)[0];
			var package    = new LocalPackage(nuspecPath);

			// move unzipped temp dir to the directoryToUnpackInto/[package id]-[package version]
			package.MoveInto(directoryToUnpackInto);
			return package;
		}

		public static void Uninstall(string name) {
			var package = GetPackage(name);
			if (package == null)
				Console.WriteLine("Package not found: {0}", package);
			else
				package.Uninstall();
		}

		public static void Install(string package) {
			if (File.Exists(package))
				InstallFromNupkg(package);
			else
				InstallFromSource(package);
		}

		public static void InstallFromNupkg(string nupkg) {
			// initialize ~/.moo/ directories, if not already initialized
			Moo.InitializeMooDir();

			// unpack into ~/.moo/packages
			var package = Unpack(nupkg, Moo.PackageDir);

			// copy .nupkg to cache
			File.Copy(nupkg, Path.Combine(Moo.CacheDir, package.NupkgFileName));

			// copy .nuspec into ~/.moo/specifications
			File.Copy(package.NuspecPath, Path.Combine(Moo.SpecDir, package.IdAndVersion + ".nuspec"));

			Console.WriteLine("Installed {0}", package);
		}

		public static void InstallFromSource(string name) {
			var package = RemotePackage.FindLatestPackageByName(name);
			if (package == null)
				Console.WriteLine("Package not found: {0}", name);
			else {
				var allToInstall = new List<RemotePackage> { package };
				allToInstall.AddRange(package.FindDependencies());
				foreach (var toInstall in allToInstall) {
					if (toInstall.IsInstalled)
						Console.WriteLine("Already installed {0}", package);
					else
						toInstall.Install();
				}
			}
		}

		// This is a crappy name ... seems to only get a Local package ... what about Remote packages?
		public static LocalPackage GetPackage(string name) {
			return Packages.FirstOrDefault(p => p.Id == name);
		}

		// TODO need to clean up the different types of packages ... InstalledPackage : LocalPackage would probably be useful tho
		public static List<LocalPackage> Packages {
			get {
				var packages = new List<LocalPackage>();

				if (! MooDirExists)
					return packages;

				foreach (var nuspecPath in Directory.GetFiles(Moo.SpecDir)) {
					var package  = new LocalPackage(nuspecPath);
					package.Path = Path.Combine(Moo.PackageDir, package.IdAndVersion);
					packages.Add(package);
				}
				return packages;
			}
		}

		/// <summary>"Installs" MooGet to Moo.Dir (~/.moo or specified via --moo-dir or in a .moorc file)</summary>
		public static void InitializeMooDir() {
			Directory.CreateDirectory(Moo.PackageDir);
			Directory.CreateDirectory(Moo.BinDir);
			Directory.CreateDirectory(Moo.CacheDir);
			Directory.CreateDirectory(Moo.DocumentationDir);
			Directory.CreateDirectory(Moo.SpecDir);
		}

		public static bool MooDirExists {
			get { return Directory.Exists(Moo.Dir); }
		}

		public static string _dir = Path.Combine(Util.HomeDirectory, ".moo");
		public static string Dir {
			get { return _dir; }
		}

		public static string PackageDir       { get { return Path.Combine(Moo.Dir, "packages");       } }
		public static string BinDir           { get { return Path.Combine(Moo.Dir, "bin");            } }
		public static string CacheDir         { get { return Path.Combine(Moo.Dir, "cache");          } }
		public static string DocumentationDir { get { return Path.Combine(Moo.Dir, "doc");            } }
		public static string SpecDir          { get { return Path.Combine(Moo.Dir, "specifications"); } }
		public static string SourceFile       { get { return Path.Combine(Moo.Dir, "sources.list");   } }
	}
}

/*
 NOTES

	case "gui-test":
		System.Windows.Forms.MessageBox.Show("moo");
		break;
	case "embedded-stuff":
		foreach (string name in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
			Console.WriteLine(name);
			string text = "";
			using (var reader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name)))
				text = reader.ReadToEnd();
			Console.WriteLine(text);
		}
}
*/

