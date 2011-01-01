using System;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a NuGet package</summary>
	/// <remarks>
	/// A MooGet.Package will typically be:
	///
	///  1. SourcePackage: has package information from a given Source.
	///  2. InstalledPackage: has information about a package actually installed on the local file system.
	///
	/// Package is an abstract class that defines certain properties / functionality that 
	/// packages have, regardless of whether or not you've installed them yet.
	/// </remarks>
	public abstract class Package {

		List<string>            _authors      = new List<string>();
		List<string>            _tags         = new List<string>();
		List<PackageDependency> _dependencies = new List<PackageDependency>();

		public string Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string Language { get; set; }

		public string LicenseUrl { get; set; }

		public PackageVersion Version { get; set; }

		public bool RequireLicenseAcceptance { get; set; }

		public List<string> Authors { get { return _authors; } }

		public List<string> Tags { get { return _tags; } }

		public List<PackageDependency> Dependencies { get { return _dependencies; } }

		public string VersionString {
			get { return Version.ToString(); }
			set { Version = new PackageVersion(value); }
		}

		public override string ToString() {
			return string.Format("{0} ({1})", Id, Version);
		}
	}
}
