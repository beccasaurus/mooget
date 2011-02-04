using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace MooGet.Commands {

	public class SourceCommand {

		[Command(Name = "source", Description = "Manage the list of NuGet feeds used to search/install packages")]
		public static object Run(string[] args) {
			var response  = new StringBuilder();
			var arguments = new List<string>(args);
			if (arguments.Count == 0) {
				response.AppendLine("*** CURRENT SOURCES ***\n");
				foreach (var source in Moo.Sources)
					response.AppendLine(source.Path);
				return response;
			}

			var command = arguments.First(); arguments.RemoveAt(0);
			switch (command) {
				case "add":
					OldSource.AddSource(arguments.First()); break;
				case "rm":
				case "remove":
					OldSource.RemoveSource(arguments.First()); break;
				default:
					return string.Format("Unknown source command: {0}", command); break;
			}

			return response;
		}
	}
}
