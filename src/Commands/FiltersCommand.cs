using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet.Commands {

	///<summary></summary>
	public class FiltersCommand {

		[Command(Name = "filters", Description = "Display all of the installed command filters")]
		public static object Run(string[] args) {
			var response          = new StringBuilder();
			var builtinFilters    = new List<CommandFilter>();
			var filtersByAssembly = new Dictionary<AssemblyName, List<CommandFilter>>();

			foreach (var filter in Moo.Filters) {
				var assemblyName = filter.Method.DeclaringType.Assembly.GetName();
				if (assemblyName.Name == "moo")
					builtinFilters.Add(filter);
				else {
					if (! filtersByAssembly.ContainsKey(assemblyName))
						filtersByAssembly[assemblyName] = new List<CommandFilter>();
					filtersByAssembly[assemblyName].Add(filter);
				}
			}

			if (builtinFilters.Any()) {
				response.AppendLine("Built-in:");
				foreach (var filter in builtinFilters)
					response.AppendFormat("  {0} \t {1}\n", filter.Name, filter.Description);
			}

			if (filtersByAssembly.Any()) {
				foreach (var item in filtersByAssembly) {
					var name    = item.Key;
					var filters = item.Value;
					response.AppendFormat("{0}:\n", name.Name);
					foreach (var filter in filters)
						response.AppendFormat("  {0} \t {1}\n", filter.Name, filter.Description);
					response.AppendLine();
				}
			}

			return response;
		}
	}
}
