using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo uninstall</summary>
	public class UninstallCommand : MooGetCommand {

		[Command("uninstall", "Uninstall a package")]
		public static object Run(string[] args) { return new UninstallCommand(args).Run(); }

		public UninstallCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = true;
			Options = new OptionSet() {
				{ "v|version=", v => Version = v         },
				{ "A|all",      v => All     = v != null }
			};
		}

		/// <summary>The version of the given package to uninstall (optional)</summary>
		public string Version { get; set; }

		/// <summary>Whether or not ALL versions of this package should be uninstalled</summary>
		public bool? All { get; set; }

		/// <summary>moo uninstall</summary>
		public override object RunDefault() {
			if (Args.Count != 1) return Help;
			var packageId = Args.First();
				
			if (All != true) return Uninstall(packageId);

			var packages = Moo.Dir.GetPackagesWithId(packageId);
			if (packages.Count == 0)
				return string.Format("Could not uninstall {0}", packageId);
			else
				foreach (var package in packages)
					Output.Line(Uninstall(package.Id));
			return Output;
		}

		string Uninstall(string packageId) {
			var dependency = new PackageDependency(string.Format("{0} {1}", packageId, Version));
			var package    = Moo.Dir.Get(dependency);

			if (package != null && Moo.Dir.Uninstall(dependency, true)) // TODO add setting for uninstalling with/without dependencies
				return string.Format("Uninstalled {0}", package);
			else
				return string.Format("Could not uninstall {0}", dependency);
		}
	}
}
