using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo install</summary>
	public class InstallCommand : MooGetCommand {

		[Command("install", "Install a package from a remote source")]
		public static object Run(string[] args) { return new InstallCommand(args).Run(); }

		public InstallCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = true;
			Options = new OptionSet() {
				{ "s|source=",  v => Source  = v },
				{ "v|version=", v => Version = v }
			};
		}

		/// <summary>A particular source to install from (optional)</summary>
		public string Source { get; set; }

		/// <summary>The version of the given package to install (optional)</summary>
		public string Version { get; set; }

		/// <summary>moo install</summary>
		public override object RunDefault() {
			// TODO we should attempt to install *every* arg that gets passed as a package dependency
			if (Args.Count != 1) return Help;
			var packageId  = Args.First();

			// Install foo.nupkg
			if (File.Exists(packageId)) {
				var packageFile = NewPackage.FromFile(packageId);
				var pushed      = Moo.Dir.Push(packageFile);
				if (pushed == null)
					return string.Format("{0} could not be installed\n", packageId);
				else
					return string.Format("Installed {0}\n", pushed.IdAndVersion());
			}

			List<ISource> sources = new List<ISource>();
			if (Source != null) {
				var source = MooGet.Source.GetSource(Source);
				if (source == null)
					return string.Format("Source not found: {0}\n", Source);
				else
					sources.Add(source);
			} else
				sources.AddRange(Moo.Dir.Sources);

			var dependency = new PackageDependency(string.Format("{0} {1}", packageId, Version));

			try {
				var installed = Moo.Dir.Install(dependency, sources.ToArray());
				if (installed != null)
					return string.Format("Installed {0}\n", installed.IdAndVersion());
				else
					return string.Format("{0} could not be installed\n", dependency);
			} catch (MooGet.PackageNotFoundException ex) {
				return ex.Message + "\n"; // Package not found: X
			}
		}
	}
}
