using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {
	public static class IPackageExtensions {
		public static PackageDependency ToPackageDependency(this IPackage package) {
			return new PackageDependency(string.Format("{0} {1}", package.Id, package.Version));
		}

		public static List<IPackage> FindDependencies(this IPackage package, params ISource[] sources) {
			return Source.FindDependencies(package, sources);
		}

		public static string IdAndVersion(this IPackage package) {
			return string.Format("{0}-{1}", package.Id, package.Version);
		}
	}
}
