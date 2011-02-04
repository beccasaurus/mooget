using System;
using System.Collections.Generic;

namespace MooGet {

	// All Sources (representing a group of Packages) must implement ISource 
	// for MooGet to be able to work with them
	//
	// If any of the interface's methods aren't supported by the source, 
	// they should throw a NotSupportedException.
	//
	// Package IDs should always be treated as case insensitive.
	public interface ISource {

		// Some sources might require authentication, for just reading or (more often) for pushing/yanking/etc.
		// This property should be used to allow sources to use arbitrary auth data.
		object AuthData { get; set; }

		// This should return a Package with matching Id and Version strings, else null.
		Package Get(string id, string version);

		// This should return ALL of this Source's packages.
		List<Package> Packages { get; }

		// This should return only the latest versions of this Source's packages.
		List<Package> LatestPackages { get; }

		// This should return ALL packages with the given Id, eg. "NUnit".
		List<Package> GetPackagesWithId(string id);

		// This should return all packages matching a given PackageDependency, eg. NUnit >= 1.0
		List<Package> GetPackagesMatchingDependency(PackageDependency dependency);

		// Every source should give you a way to download one of its packages.
		Nupkg Fetch(string id, string version);

		// Some sources let you push your own package to them.
		// Returns the Source's representation of the Package if successful, else null.
		Package Push(Nupkg nupkg);

		// Some sources let you delete packages from them;
		bool Yank(string id, string version);
	}
}
