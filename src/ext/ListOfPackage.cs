using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Extension methods for System.Collections.Generic.List of Package</summary>
	public static class ListOfPackageExtensions {
		
		public static List<string> Ids(this List<Package> packages) {
			return packages.Select(pkg => pkg.Id).ToList();
		}
	}
}
