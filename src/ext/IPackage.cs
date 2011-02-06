using System;

namespace MooGet {
	public static class IPackageExtensions {
		public static PackageDependency ToPackageDependency(this IPackage package) {
			return new PackageDependency(string.Format("{0} {1}", package.Id, package.Version));
		}
	}
}
