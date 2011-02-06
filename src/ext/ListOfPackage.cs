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
	}
}
