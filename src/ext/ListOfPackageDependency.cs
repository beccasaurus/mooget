using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Extension methods for System.Collections.Generic.List of PackageDependency</summary>
	public static class ListOfPackageDependencyExtensions {
		
		public static List<string> Ids(this List<PackageDependency> dependencies) {
			return dependencies.Select(dep => dep.Id).ToList();
		}
	}
}
