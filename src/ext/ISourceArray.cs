using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {
	public static class ISourceArrayExtensions {
		public static IPackage GetLatest(this ISource[] sources, PackageDependency dependency) {
			var packages = new List<ISource>(sources).Select(source => source.GetLatest(dependency)).Where(package => package != null).ToList();
			if (packages.Count <= 1) return packages.FirstOrDefault();
			var highest = PackageVersion.HighestVersion(packages.Select(pkg => pkg.Version).ToArray());
			return packages.FirstOrDefault(pkg => pkg.Version == highest);
		}
	}
}
