using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo extensions</summary>
	public class ExtensionsCommand {

		[Command("extensions", "Display all installed MooGet extensions", Debug = true)]
		public static object Run(string[] args) {
			if (Moo.Extensions.Count == 0)
				return "No installed extensions\n";

			var output = new StringBuilder();
			output.Line("Installed extensions:");
			foreach (var assembly in Moo.Extensions) {
				output.Line("  " + assembly.Location);

				// Print out this Extension's Commands
				var commands = Command.GetCommands(assembly);
				if (commands.Count > 0) {
					output.Line("    Commands:");
					foreach (var command in commands)
						output.Line("      " + command.Name);
				}

				// Print out this Extension's Filters
				var filters = CommandFilter.GetFilters(assembly);
				if (filters.Count > 0) {
					output.Line("    Filters:");
					foreach (var filter in filters)
						output.Line("      " + filter.Name);
				}
			}
			return output;
		}
	}
}
