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

		public static IDictionary<string, List<IPackage>> GroupById(this List<IPackage> packages) {
			var grouped = new Dictionary<string, List<IPackage>>();
			foreach (var group in packages.GroupBy(pkg => pkg.Id)) {
				grouped[group.Key] = new List<IPackage>();
				foreach (var package in group)
					grouped[group.Key].Add(package);
			}
			return grouped;
		}

		public static List<PackageVersion> Versions(this List<IPackage> packages) {
			return packages.Select(pkg => pkg.Version).ToList();
		}
	}
}
