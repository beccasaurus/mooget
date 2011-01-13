using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents the primary API for most MooGet actions</summary>
	public partial class Moo {

		/// <summary>moo.exe Entry method</summary>
		/// <remarks>
		///	moo.exe runs by running all [CommandFilter] methods defined 
		///	in moo.exe or in any installed packages that have libraries 
		///	named MooGet.*.dll
		///
		///	The Moo.CommandRunnerFilter is meant to be the last filter that 
		///	gets run, as it finds and runs all [Command] methods defined 
		///	in moo.exe or in any installed packages that have libraries 
		///	named MooGet.*.dll and runs them.
		///
		///	Moo.Filters returns the full List&lt;CommandFilter&gt; that moo.exe runs.
		/// </remarks>
		public static void Main(string[] args) {
			var filters = new List<CommandFilter>(Moo.Filters);
			for (var i = 0; i < filters.Count - 1; i++)
				filters[i].InnerFilter = filters[i + 1];

			try {
				Console.Write(filters[0].Invoke(args));
			} catch (Exception ex) {

				// Get the first exception that's not a TargetInvocationException (which we get because we Invoke() our commands)
				Exception inner = ex;
				while (inner.InnerException != null && inner is TargetInvocationException)
					inner = inner.InnerException;

				// Use CowSay to display the error message
				Cow.Columns = 80;
				Cow.Say("Moo. There was a problem:                                                   {0}", inner);
			}
		}

		public static string OfficialNugetFeed = "http://go.microsoft.com/fwlink/?LinkID=199193";

		public static string UserAgent { get { return Moo.Version; } }

		public static string Version { get { return "Moo " + Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

		public static List<Source> DefaultSources = new List<Source> {
			new Source(Moo.OfficialNugetFeed)
		};

		static List<Assembly> _extensions;
		public static List<Assembly> Extensions {
			get {
				if (_extensions == null) {
					_extensions = new List<Assembly>();
					foreach (var package in Moo.Packages)
						foreach (var dll in package.GetFiles(package.LibDirectory, "MooGet.*.dll"))
							_extensions.Add(Assembly.LoadFile(dll)); // TODO add try/catch incase the assembly doesn't load cleanly
				}
				return _extensions;
			}
			set { _extensions = value; }
		}

		static List<CommandFilter> _filters;
		public static List<CommandFilter> Filters {
			get {
				if (_filters == null) {
					_filters = new List<CommandFilter>();
					foreach (var assembly in Extensions)
						_filters.AddRange(CommandFilter.GetFilters(assembly));
					_filters.AddRange(CommandFilter.GetFilters()); // currently executing assembly
				}
				return _filters;
			}
			set { _filters = value; }
		}

		static List<Command> _commands;
		public static List<Command> Commands {
			get {
				if (_commands == null) {
					_commands = new List<Command>();
					foreach (var assembly in Extensions)
						_commands.AddRange(Command.GetCommands(assembly));
					_commands.AddRange(Command.GetCommands()); // currently executing assembly
				}
				return _commands;
			}
			set { _commands = value; }
		}

		public static object FindAndRunCommand(string[] args) {
			var arguments   = new List<string>(args);
			var commandName = arguments.First(); arguments.RemoveAt(0);
			var commands    = Commands.Where(c => c.Name.StartsWith(commandName)).ToList();

			if (commands.Count == 0)
				return string.Format("Command not found: {0}\n\nCommands: {1}", commandName, string.Join("\n", Commands.Select(c => c.Name).ToArray()));
			else if (commands.Count == 1)
				return commands.First().Run(arguments.ToArray());
			else
				return string.Format("Ambiguous command '{0}'.  Did you mean one of these?  {1}", commandName, string.Join(", ", commands.Select(c => c.Name).ToArray()));
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

