using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {
	public static class ISourceExtensions {

		public static List<PackageVersion> VersionsAvailableOf(this ISource source, string id) {
			return source.GetPackagesWithId(id).Select(pkg => pkg.Version).ToList();
		}

		public static PackageVersion HighestVersionAvailableOf(this ISource source, string id) {
			return PackageVersion.HighestVersion(source.VersionsAvailableOf(id).ToArray());
		}

		public static PackageVersion HighestVersionMatching(this ISource source, PackageDependency dependency) {
			var packages = source.GetPackagesMatchingDependencies(dependency);
			return PackageVersion.HighestVersion(packages.Select(pkg => pkg.Version).ToArray());
		}

		public static IPackage GetLatest(this ISource source, string id) {
			var highest = source.HighestVersionAvailableOf(id);
			return source.Packages.FirstOrDefault(pkg => pkg.Version == highest);
		}

		public static IPackage GetLatest(this ISource source, PackageDependency dependency) {
			var highest = source.HighestVersionMatching(dependency);
			
			// do a Get for the EXACT version that we found
			return source.Get(new PackageDependency(string.Format("{0} = {1}", dependency.PackageId, highest)));
		}
	}
}
