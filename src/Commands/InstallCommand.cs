using System;
using System.Text;

namespace MooGet.Commands {

	public class InstallCommand {

		[Command(Name = "install", Description = "Install a package into the local repository")]
		public static object Run(string[] args) {
			var package = Moo.Install(args[0]);
			if (package == null)
				return string.Format("Package not found: {0}", args[0]);
			else
				return string.Format("Installed {0}", package);
		}
	}
}
