using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents the primary API for most MooGet actions</summary>
	public partial class Moo {

		public static string OfficialNugetFeed = "http://go.microsoft.com/fwlink/?LinkID=199193";

		public static string UserAgent { get { return Moo.Version; } }

		public static string Version { get { return "Moo " + Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

		/// <summary>Entry method</summary>
		public static void Main(string[] args) {
			if (args.Length == 0)
				args = new string[] { "help" };

			if (args[0] == "-v" || args[0] == "--version")
				Console.WriteLine(Moo.Version);
			else
				FindAndRunCommand(args);
		}

		public static List<Command> Commands = Command.GetCommands();

		public static void FindAndRunCommand(string[] args) {
			var arguments   = new List<string>(args);
			var commandName = arguments.First(); arguments.RemoveAt(0);
			var command     = Commands.FirstOrDefault(c => c.Name == commandName);

			if (command != null)
				command.Run(arguments.ToArray());
			else
				Console.WriteLine("Command not found: {0}\n\nCommands: {1}", commandName, string.Join("\n", Commands.Select(c => c.Name).ToArray()));
		}
			
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
			Console.WriteLine("Unpacked {0}", package.IdAndVersion);
			return package;
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

			Console.WriteLine("Installed {0}", package.IdAndVersion);
		}

		public static void InstallFromSource(string name) {
			var package = new Source(Moo.OfficialNugetFeed).Get(name); // TODO should use configured user sources
			if (package == null) {
				Console.WriteLine("Package not found: {0}", name);
			} else {
				package.Install();
			}
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

