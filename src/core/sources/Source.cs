using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>ISource implementation useful as a baseclass for other sources</summary>
	/// <remarks>
	/// Used for:
	///  - useful base class for ISource implementations
	///  - managing local moo sources
	/// </remarks>
	public abstract class Source : ISource {

		#region Abstract Members
		public abstract Nupkg         Fetch(PackageDependency dependency);
		public abstract Package       Push(Nupkg nupkg);
		public abstract bool          Yank(PackageDependency dependency);
		public abstract Package       Install(PackageDependency dependency, params ISource[] sourcesForDependencies);
		public abstract bool          Uninstall(PackageDependency dependency, bool uninstallDependencies);
		public abstract List<Package> Packages { get; }
		#endregion

		public virtual string Name     { get; set; }
		public virtual string Path     { get; set; }
		public virtual object AuthData { get; set; }

		public virtual Package Get(PackageDependency dependency) {
			return null;
		}

		public virtual List<Package> LatestPackages { get { return null; } }

		public virtual List<Package> GetPackagesWithId(string id) {
			return null;
		}

		public virtual List<Package> GetPackagesWithIdStartingWith(string query) {
			return null;
		}

		public virtual List<Package> GetPackagesMatchingDependency(PackageDependency dependency) {
			return null;
		}
	}
}
