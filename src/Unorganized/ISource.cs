using System;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>All source must implement ISource for MooGet to work with them.</summary>
	/// <remarks>
	/// If any of the interface's methods aren't supported by the source, 
	/// they should throw a NotSupportedException.
	///
	/// Package IDs should always be treated as case insensitive.
	/// </remarks>
	public interface ISource {

		/// <summary>Name describing this source</summary>
		string Name { get; }

		/// <summary>This source's path.  Typically a file system path or remote url.</summary>
		string Path { get; }

		/// <summary>
		/// Some sources might require authentication, for just reading or (more often) for pushing/yanking/etc.
		/// This property should be used to allow sources to use arbitrary auth data.
		/// </summary>
		object AuthData { get; set; }

		/// <summary>This should return a Package with matching Id and Version strings, else null.</summary>
		Package Get(string id, string version);

		/// <summary>This should return ALL of this Source's packages.</summary>
		List<Package> Packages { get; }

		/// <summary>This should return only the latest versions of this Source's packages.</summary>
		List<Package> LatestPackages { get; }

		/// <summary>This should return ALL packages with the given Id, eg. "NUnit".</summary>
		List<Package> GetPackagesWithId(string id);

		/// <summary>This should return all packages matching a given PackageDependency, eg. NUnit >= 1.0</summary>
		List<Package> GetPackagesMatchingDependency(PackageDependency dependency);

		/// <summary>Every source should give you a way to download one of its packages.</summary>
		Nupkg Fetch(string id, string version);

		/// <summary>
		/// Some sources let you push your own package to them.
		/// Returns the Source's representation of the Package if successful, else null.
		/// </summary>
		Package Push(Nupkg nupkg);

		/// <summary>Some sources let you delete packages from them;</summary>
		bool Yank(string id, string version);
	}
}
