using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet.Commands {

	public class SearchCommand {

		[Command(Name = "search", Description = "Search remote source for packages")]
		public static object Run(string[] args) {
			var response = new StringBuilder();
			var packages = new List<RemotePackage>(); // TODO rename to RemotePackage ...
			var sources  = new List<string>();
			var opts     = new OptionSet() {
				{ "s|source=",  v => { if (v != null) sources.Add(v); }}
			};
			var extra = opts.Parse(args);
			var query = extra[0];

			if (sources.Count == 0)
				sources.AddRange(Moo.Sources.Select(src => src.Path).ToList());

			foreach (var source in sources)
				packages.AddRange(new OldSource(source).SearchByTitle(query));

			if (packages.Count == 0)
				response.AppendFormat("No packages matched: {0}\n", query);
			else {
				var packageIdsAndVersions = new Dictionary<string, List<PackageVersion>>();
				foreach (var package in packages.OrderBy(p => p.Id)) {
					if (! packageIdsAndVersions.ContainsKey(package.Id)) packageIdsAndVersions[package.Id] = new List<PackageVersion>();
					packageIdsAndVersions[package.Id].Add(package.Version);
				}
				foreach (var packageIdAndVersion in packageIdsAndVersions)
					response.AppendFormat("{0} ({1})\n", 
							packageIdAndVersion.Key, 
							string.Join(", ", packageIdAndVersion.Value.OrderBy(version => version).Select(version => version.ToString()).Reverse().ToArray()));
			}
			return response;
		}
	}
}
