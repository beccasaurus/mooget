using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary></summary>
	public class FiltersCommand {

		[Command("filters", "Display all of the installed command filters", Debug = true)]
		public static object Run(string[] args) {
			var response          = new StringBuilder();
			var builtinFilters    = new List<CommandFilter>();
			var filtersByAssembly = new Dictionary<AssemblyName, List<CommandFilter>>();

			foreach (var filter in CommandFilter.Filters) {
				var assemblyName = filter.AssemblyName;
				if (assemblyName.Name == "moo")
					builtinFilters.Add(filter);
				else {
					if (! filtersByAssembly.ContainsKey(assemblyName))
						filtersByAssembly[assemblyName] = new List<CommandFilter>();
					filtersByAssembly[assemblyName].Add(filter);
				}
			}

			if (builtinFilters.Any()) {
				var spaces = builtinFilters.Select(f => f.Name.Length).Max() + 2;
				response.AppendLine("Built-in:");
				foreach (var filter in builtinFilters)
					response.AppendFormat("  {0}{1}\n", filter.Name.WithSpaces(spaces), filter.Description);
			}

			if (filtersByAssembly.Any()) {
				response.AppendLine();
				foreach (var item in filtersByAssembly) {
					var name    = item.Key;
					var filters = item.Value;
					var spaces  = filters.Select(f => f.Name.Length).Max() + 2;
					response.AppendFormat("{0}:\n", name.Name);
					foreach (var filter in filters)
						response.AppendFormat("  {0}{1}\n", filter.Name.WithSpaces(spaces), filter.Description);
					response.AppendLine();
				}
			}

			return response;
		}
	}
}
