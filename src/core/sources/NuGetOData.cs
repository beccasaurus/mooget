using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>An ISource representing an OData service with NuGet packages (using the "official" NuGet WCF service)</summary>
	public class NuGetOData : Source, ISource, IDirectory {

		public NuGetOData() : base() {}
		public NuGetOData(string path) : this() {
			Path = path;
		}

		/// <summary>Returns true if the given path looks like a valid NuGetOData, else false</summary>
		/// <remarks>
		/// Right now, this is true for ANY path that starts with http://
		/// </remarks>
		public static bool IsValidPath(string path) {
			return path.StartsWith("http");
		}

		/// <summary>Returns all of the Nupkg provided by this service</summary>
		public override List<IPackage> Packages {
			get { return null; }
		}

		// public override IPackage Get(PackageDependency dependency) {
		// 	return Packages.Where(pkg => dependency.Matches(pkg)).ToList().Latest();
		// }

		// public override List<IPackage> LatestPackages {
		// 	get { return Packages.Where(pkg => pkg.Version == this.HighestVersionAvailableOf(pkg.Id)).ToList(); }
		// }

		// public override List<IPackage> GetPackagesWithId(string id) {
		// 	id = id.ToLower();
		// 	return Packages.Where(pkg => pkg.Id.ToLower() == id).ToList();
		// }

		// public override List<IPackage> GetPackagesWithIdStartingWith(string query) {
		// 	query = query.ToLower();
		// 	return Packages.Where(pkg => pkg.Id.ToLower().StartsWith(query)).ToList();
		// }

		public override IPackageFile Fetch(PackageDependency dependency, string directory) {
			return null;
		}

		public override IPackage Push(IPackageFile file) {
			return null;
		}

		public override bool Yank(PackageDependency dependency) {
			return false;
		}
	}
}
