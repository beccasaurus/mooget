using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Extension methods for System.Collections.Generic.List of Package</summary>
	public static class ListOfPackageExtensions {
		
		public static List<string> Ids(this List<IPackage> packages) {
			return packages.Select(pkg => pkg.Id).ToList();
		}

		public static List<IPackage> AddPackages(this List<IPackage> packages, List<Nupkg> nupkgs) {
			if (nupkgs == null) return packages;
			nupkgs.ForEach(pkg => packages.Add(pkg));
			return packages;
		}

		public static PackageVersion HighestVersion(this List<IPackage> packages) {
			return packages.Select(pkg => pkg.Version).Max();
		}

		public static IPackage Latest(this List<IPackage> packages) {
			var highest = packages.HighestVersion();
			return packages.FirstOrDefault(pkg => pkg.Version == highest);
		}

		public static PackageVersion LowestVersion(this List<IPackage> packages) {
			return packages.Select(pkg => pkg.Version).Min();
		}

		public static IPackage Oldest(this List<IPackage> packages) {
			var lowest = packages.LowestVersion();
			return packages.FirstOrDefault(pkg => pkg.Version == lowest);
		}
	}
}
