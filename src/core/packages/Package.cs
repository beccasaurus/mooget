using System;
using System.IO;
using System.Collections.Generic;

namespace MooGet {

	// TODO rename to Package once we're done refactoring.
	
	/// <summary>Represents a NuGet package</summary>
	/// <remarks>
	///	MooGet.Package provides:
	///	 - a simple base class for other IPackage implementations
	///	 - static methods for general Package management
	///
	///	This isn't meant to have much logic at ALL. 
	///	It's a VERY simple IPackage implementation.
	/// </remarks>
	public class NewPackage : IPackage {

		public virtual string Id { get; set; }

		public virtual PackageVersion Version { get; set; }

		public virtual string VersionText {
			get { return Version.ToString();           }
			set { Version = new PackageVersion(value); }
		}

		public virtual ISource Source { get; set; }

		public virtual PackageDetails Details { get; set; }

		public virtual List<string> Files { get; set; }

		public override string ToString() {
			return this.IdAndVersion();
		}

		public static IPackageFile FromFile(string path) {
			if (! File.Exists(path)) return null;

			// right now, we only support Nupkg files so ... just return one of those!
			return new Nupkg(path);
		}
	}
}
