using System;
using System.Text;

namespace MooGet.Commands {

	public class ListCommand {

		[Command(Name = "list", Description = "Displays list of installed packages")]
		public static object Run(string[] args) {
			var response = new StringBuilder();
			var packages = Moo.Packages;
			if (packages.Count == 0)
				response.AppendLine("No installed packages");
			else {
				response.AppendLine("Listing installed packages:");
				foreach (var package in packages)
					response.AppendFormat("{0} ({1})\n", package.Id, package.Version); // TODO list all installed versions
			}
			return response;
		}
	}
}
