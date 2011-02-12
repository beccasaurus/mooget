using System;

namespace MooGet.Commands {

	///<summary></summary>
	public class ShowCommand {

		[Command(Name = "show", Description = "Display package details")]
		public static object Run(string[] args) {
			var package = RemotePackage.FindLatestPackageByName(args[0]);
			if (package == null)
				return string.Format("Package not found: {0}", args[0]);
			else
				return package.DetailString;
		}
	}
}
