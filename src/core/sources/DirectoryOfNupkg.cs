using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>An ISource representing a directory with .nupkg files in it (Nupkg packages)</summary>
	public class DirectoryOfNupkg : Source, ISource {

		public DirectoryOfNupkg() : base() {}
		public DirectoryOfNupkg(string path) : this() {
			Path = path;
		}

		/// <summary>Returns all of the Nupkg in this directory</summary>
		public override List<Package> Packages {
			get { return new List<Package>(); }
		}

		public override Nupkg Fetch(PackageDependency dependency) {
			return null;
		}
		
		public override Package Push(Nupkg nupkg) {
			return null;
		}
		
		public override bool Yank(PackageDependency dependency) {
			return false;
		}
		
		public override Package Install(PackageDependency dependency, params ISource[] sourcesForDependencies) {
			return null;
		}

		public override bool Uninstall(PackageDependency dependency, bool uninstallDependencies) {
			return false;
		}
	}
}
