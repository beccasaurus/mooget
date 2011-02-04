using System;

namespace MooGet {

	// TODO rename to Package once we're done refactoring.
	
	/// <summary>Represents a NuGet package</summary>
	/// <remarks>
	///	MooGet.Package provides:
	///	 - a simple base class for other IPackage implementations
	///	 - static methods for general Package management
	/// </remarks>
	public class NewPackage : IPackage {

		public virtual string Id { get; set; }

		public virtual PackageVersion Version { get; set; }

		public virtual string VersionText {
			get { return Version.ToString();           }
			set { Version = new PackageVersion(value); }
		}

		public virtual ISource Source { get { return null; } }

		public virtual PackageDetails Details { get { return null; } }

		public virtual string[] Files { get { return null; } }
	}
}
