using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {
	public static class ISourceExtensions {

		public static List<PackageVersion> VersionsAvailableOf(this ISource source, string id) {
			Console.WriteLine("VersionsAvailableOf id:{0} source:{1} ({2})", id, source, source.Path);
			return source.GetPackagesWithId(id).Select(pkg => pkg.Version).ToList();
		}

		public static PackageVersion HighestVersionAvailableOf(this ISource source, string id) {
			return PackageVersion.HighestVersion(source.VersionsAvailableOf(id).ToArray());
		}

		public static PackageVersion HighestVersionMatching(this ISource source, PackageDependency dependency) {
			Console.WriteLine("Looking for HighestVersionMatching dependency:{0} in source:{0}", dependency, source.Path);
			var packages = source.GetPackagesMatchingDependencies(dependency);
			Console.WriteLine("Done looking for HighestVersionMatching ... found packages: {0}", string.Join(", ", packages.Select(p => p.IdAndVersion()).ToArray()));
			return PackageVersion.HighestVersion(packages.Select(pkg => pkg.Version).ToArray());
		}

		public static IPackage GetLatest(this ISource source, string id) {
			Console.WriteLine("ISource.GetLatest({0}, {1})", source, id);
			Console.WriteLine("GetLatest id:{0} source:{1} ({2})", id, source, source.Path);
			var highest = source.HighestVersionAvailableOf(id);
			return source.Packages.FirstOrDefault(pkg => pkg.Version == highest);
		}

		public static IPackage GetLatest(this ISource source, PackageDependency dependency) {
			Console.WriteLine("ISource.GetLatest source:{0} dependency:{1}", source.Path, dependency);
			var highest = source.HighestVersionMatching(dependency);
			Console.WriteLine("Done with ISource.GetLatest, highest: {0}", highest);
			
			// do a Get for the EXACT version that we found
			Console.WriteLine("To return the latest, GetLatest is doing a Get() with dependency: {0}", new PackageDependency(string.Format("{0} = {1}", dependency.PackageId, highest)));
			return source.Get(new PackageDependency(string.Format("{0} = {1}", dependency.PackageId, highest)));
		}
	}
}
