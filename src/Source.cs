using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/*
	 * Used for:
	 *  - useful base class for ISource implementations
	 *  - managing local moo sources
	 */
	public class Source : ISource {

		public virtual Package Get(string id, string version) {
			return null;
		}

		public virtual List<Package> Packages {
			get { return null; }
		}

		public virtual List<Package> LatestPackages {
			get { return null; }
		}

		public virtual List<Package> GetPackagesWithId(string id) {
			return null;
		}

		public virtual List<Package> GetPackagesMatchingDependency(PackageDependency dependency) {
			return null;
		}
	}
}
