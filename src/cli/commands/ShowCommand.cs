using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	// TODO copied lots of stuff from ListCommand (to get SourcesToQuery) into this ... refactor?
	///<summary>moo show</summary>
	public class ShowCommand : MooGetCommand {

		[Command("show", "Displays information about packages")]
		public static object Run(string[] args) { return new ShowCommand(args).Run(); }

		public ShowCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = true;
			Options = new OptionSet() {
				{ "s|source=",      v => Source        = v         },
				{ "l|local",        v => QueryLocal    = v != null },
				{ "r|remote",       v => QueryRemote   = v != null },
				{ "relative",       v => RelativePaths = v != null },
				{ "m|moodir",       v => MooPaths      = v != null },
				{ "d|dependencies", v => Dependencies  = v != null },
				{ "e|env=",         v => EnvironmentVariable = v   },
				{ "f|framework=",   v => FrameworkName       = v   }
			};
		}

		/// <summary>A particular source to query against</summary>
		public virtual string Source { get; set; }

		/// <summary>Whether the paths that we print should be relative to the source (eg. moo dir)</summary>
		public virtual bool RelativePaths { get; set; }

		/// <summary>Whether the paths that we print out should be formatted to use $(MOO_PATH) for use in MSBuild files</summary>
		public virtual bool MooPaths { get; set; }

		/// <summary>If set, we will format paths to be relative and use $(ENV_VAR_NAME) for use in MSBuild files (like we do for MooPaths)</summary>
		public virtual string EnvironmentVariable { get; set; }

		/// <summary>The name of a specific framework to list assemblies for (will also display global assemblies)</summary>
		public virtual string FrameworkName { get; set; }

		/// <summary>Whether we should also print out the paths to dependencies libraries/content/etc</summary>
		public virtual bool Dependencies { get; set; }

		/// <summary>Whether or not we should query the local (installed) packages</summary>
		public virtual bool? QueryLocal { get; set; }

		/// <summary>Whether or not we should query ALL remote sources (added via 'moo source')</summary>
		public virtual bool? QueryRemote { get; set; }

		/// <summary>If the first argument passed was a command, this returns it, else null</summary>
		public virtual string Command { get; set; }

		// -r doesn't do anything yet!  and you can only specify one -s right now!  i think?
		public List<ISource> SourcesToQuery {
			get {
				var sources = new List<ISource>();

				// ADD THIS FIRST! we don't want to query remote sources if we don't have to ...
				// -l --local
				if (QueryLocal == true)
					sources.Add(Moo.Dir);

				// -s --source
				if (Source != null) {
					var src = MooGet.Source.GetSource(Source);
					if (src == null)
						throw new HandledException("Source not found: {0}", Source);
					sources.Add(src);
				}

				return sources;
			}
		}

		void SetDefaults() {
			if (QueryLocal == null && QueryRemote == null && Source == null)
				QueryLocal = true;
		}

		/// <summary>The sub-commands that moo show has</summary>
		public string[] Commands = new string[]{ "libraries", "content",  "tools" };

		/// <summary>Parses the arguments to see if the first argument is a command.  Removes it from args and returns it.</summary>
		public virtual string ParseCommand() {
			var firstArg = Args.First();
			if (Commands.Contains(firstArg)) {
				Command = firstArg;
				Args.RemoveAt(0);
				return Command;
			} else
				return null;
		}

		public virtual IPackage GetPackage(string packageId) {
			return GetPackage(new PackageDependency(packageId));
		}

		public virtual IPackage GetPackage(PackageDependency dependency) {
			foreach (var source in SourcesToQuery) {
				var package = source.Get(dependency);
				if (package != null)
					return package;
			}
			return null;
		}

		/// <summary>moo show</summary>
		public override object RunDefault() {
			SetDefaults();
			ParseCommand();

			// For each argument we were passed, find that package from the sources we're looking in
			var packages = new List<IPackage>();
			foreach (var packageId in Args) {
				var package = GetPackage(packageId);
				if (package != null)
					packages.Add(package);
			}
			
			if (Dependencies) {
				var found = new List<IPackage>();
				foreach (var package in packages) {
					foreach (var dependency in package.Details.Dependencies) {
						if (found.Any(pkg => dependency.Matches(pkg)))    continue; // we already found a match
						if (packages.Any(pkg => dependency.Matches(pkg))) continue; // we already found a match

						// We haven't already found a package matching this dependency ... try to find it from the sources
						var fromSources = GetPackage(dependency);
						if (fromSources != null)
							found.Add(fromSources);
					}
				}
				packages.AddRange(found);
			}

			if (Command == null) {
				packages.ForEach(package => OutputGeneralPackageInfo(package));
			} else {
				switch (Command) {
					case "libraries": packages.ForEach(package => OutputLibraries(package)); break;
					default:
						return string.Format("We haven't implemented this command yet: {0}\n", Command);
				}
			}

			return Output;
		}

		/// <summary>Outputs general useful information about this package</summary>
		public void OutputGeneralPackageInfo(IPackage package) {
			Output.AppendFormat("Id:      {0}\n", package.Id);
			Output.AppendFormat("Version: {0}\n", package.Version);
			Output.AppendFormat("Source:  {0}\n", package.Source.Path);
			Output.Append("\n");
		}

		/// <summary>Outputs the libraries in this package ... package must be an IPackageWithFiles</summary>
		public void OutputLibraries(IPackage package) {
			var withFiles = package as IPackageWithFiles;
			if (withFiles == null) return;

			var sourceDir = package.Source.Path.AsDir();
			var libraries = (FrameworkName == null) ? withFiles.Libraries : withFiles.LibrariesFor(FrameworkName);

			foreach (var library in libraries) {
				var relative = sourceDir.Relative(library);

				if (MooPaths)
					Output.AppendFormat("$(MOO_DIR)/{0}\n", relative);
				else if (EnvironmentVariable != null)
					Output.AppendFormat("$({0})/{1}\n", EnvironmentVariable, relative);
				else if (RelativePaths)
					Output.AppendFormat("{0}\n", relative);
				else	
					Output.AppendFormat("{0}\n", library);
			}
		}
	}
}
