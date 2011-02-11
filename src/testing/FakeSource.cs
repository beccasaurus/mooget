using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;

namespace MooGet.Test {

	/// <summary>Implementation of ISource geared towards testing</summary>
	public class FakeSource : Source, ISource {

		public FakeSource() {
			_packages = new List<IPackage>();
		}

		List<IPackage> _packages;

		public override List<IPackage> Packages { get { return _packages; } }

		public void Add(string nameAndVersion, params string[] dependencies) {
			Packages.Add(new FakePackage(nameAndVersion, dependencies));
		}

		// not implemented (yet) below here ...
		public override IPackageFile Fetch(PackageDependency dependency, string directory) { return null; }
		public override IPackage     Push(IPackageFile file) { return null; }
		public override bool         Yank(PackageDependency dependency) { return false; }
		public override IPackage     Install(PackageDependency dependency, params ISource[] sourcesForDependencies) { return null; }
		public override bool         Uninstall(PackageDependency dependency, bool uninstallDependencies) { return false; }
	}
}
