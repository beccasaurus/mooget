using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {
	public static class ISourceArrayExtensions {
		public static IPackage GetLatest(this ISource[] sources, PackageDependency dependency) {
			Console.WriteLine("Looking for the latest version of:{0} in sources:{1}", dependency, string.Join(", ", sources.Select(s => s.Path).ToArray()));
			var packages = new List<ISource>(sources).Select(source => source.GetLatest(dependency)).Where(package => package != null).ToList();
			Console.WriteLine("Found packages: {0}", string.Join(", ", packages.Select(p => p.IdAndVersion()).ToArray()));
			if (packages.Count <= 1) return packages.FirstOrDefault();
			var highest = PackageVersion.HighestVersion(packages.Select(pkg => pkg.Version).ToArray());
			return packages.FirstOrDefault(pkg => pkg.Version == highest);
		}
	}
}
