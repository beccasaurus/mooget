using System;

namespace MooGet {

	/// <summary>Represents a NuGet package</summary>
	/// <remarks>
	/// A MooGet.Package will be either a SourcePackage, with package information from 
	/// a given Source, or an InstalledPackage, with information about a package actually 
	/// installed on the local file system.
	///
	/// Package is an abstract class defining certain properties / functionality that 
	/// packages have, regardless of whether or not you've installed them yet.
	/// </remarks>
	public abstract class Package {

		public string Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public PackageVersion Version { get; set; }

		public string VersionString {
			get { return Version.ToString(); }
			set { Version = new PackageVersion(value); }
		}

		public override string ToString() {
			return string.Format("{0} ({1})", Id, Version);
		}
	}
}
