using System;

namespace MooGet {

	/// <summary>When we expect a IPackage to exist, but it doesn't ... we throw this.</summary>
	public class PackageNotFoundException : Exception {
		public PackageNotFoundException(string message) : base(message) {}
		public PackageNotFoundException(PackageDependency dependency) : this("Package not found: " + dependency.PackageId) {}
	}
}
