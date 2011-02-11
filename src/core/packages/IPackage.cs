using System;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>All packages must implement IPackage for MooGet to work with them.</summary>
	public interface IPackage {

		/// <summary>This package's unique identifier, eg. "NUnit"</summary>
		string Id { get; set; }

		/// <summary>This packages version, eg. 1.0 (as a PackageVersion)</summary>
		PackageVersion Version { get; set; }

		/// <summary>The Source this package is associated with, if any.</summary>
		ISource Source { get; }

		/// <summary>This packages Details, eg. Description, Author, etc (if available).</summary>
		PackageDetails Details { get; }

		/// <summary>A list of all of the files contained in this package (if available).</summary>
		// TODO REMOVE THIS!  Move into IPackageWithFiles
		List<string> Files { get; }
	}
}
