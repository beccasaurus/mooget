using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet.Commands {

	public class InstallCommand {

		public bool   DryRun = false;
		public string PackageToInstall;

		public object Go(string[] args) {
			var opts = new OptionSet() {
				{ "dry-run",  v => { if (v != null) DryRun = true; }}
			};
			var extra = opts.Parse(args);
			
			if (extra.Count == 1)
				return Install(extra.First());
			else
				return string.Format("Unexpected arguments: {0}", string.Join(" ", extra.ToArray()));
		}

		object Install(string packageName) {
			Package package = null;
			if (File.Exists(packageName))
				package = Moo.InstallFromNupkg(packageName); // this is a stupid place to pass in DryRun ... we should pass in an object ...
			else
				package = Moo.InstallFromSource(packageName, DryRun);
			
			if (package == null)
				return string.Format("Package not found: {0}\n", packageName);
			else
				return string.Format("Installed {0}\n", package);
		}

		[Command(Name = "install", Description = "Install a package into the local repository")]
		public static object Run(string[] args) {
			return new InstallCommand().Go(args);
		}
	}
}
