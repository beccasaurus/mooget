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
		public abstract Nupkg          Fetch(PackageDependency dependency);
		public abstract IPackage       Push(Nupkg nupkg);
		public abstract bool           Yank(PackageDependency dependency);
		public abstract IPackage       Install(PackageDependency dependency, params ISource[] sourcesForDependencies);
		public abstract bool           Uninstall(PackageDependency dependency, bool uninstallDependencies);
		public abstract List<IPackage> Packages { get; }
		#endregion

		public virtual string Name     { get; set; }
		public virtual string Path     { get; set; }
		public virtual object AuthData { get; set; }

		public virtual IPackage Get(PackageDependency dependency) {
			return Packages.FirstOrDefault(pkg => dependency.Matches(pkg));
		}

		public virtual List<IPackage> LatestPackages { get { return null; } }

		public virtual List<IPackage> GetPackagesWithId(string id) {
			return null;
		}

		public virtual List<IPackage> GetPackagesWithIdStartingWith(string query) {
			return null;
		}

		public virtual List<IPackage> GetPackagesMatchingDependency(PackageDependency dependency) {
			return null;
		}
	}
}
