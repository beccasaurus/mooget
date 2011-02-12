using System;

namespace MooGet.Commands {

	///<summary></summary>
	public class FetchCommand {

		[Command(Name = "fetch", Description = "Download a .nupkg to the current directory")]
		public static object Run(string[] args) {
			var package = RemotePackage.FindLatestPackageByName(args[0]);
			if (package == null)
				return string.Format("Package not found: {0}", args[0]);
			else {
				package.Fetch();
				return string.Format("Downloaded {0}", package.Nupkg);
			}
		}
	}
}
