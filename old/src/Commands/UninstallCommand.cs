using System;

namespace MooGet.Commands {

	public class UninstallCommand {

		[Command(Name = "uninstall", Description = "Removes a package from the local repository")]
		public static object Run(string[] args) {
			var package = Moo.Uninstall(args[0]);
			if (package == null)
				return string.Format("Package not found: {0}", args[0]);
			else
				return string.Format("Uninstalled {0}", package.IdAndVersion);
		}

		[Command("Alias for uninstall")]
		public static object Remove(string[] args) {
			return Run(args);
		}
	}
}
