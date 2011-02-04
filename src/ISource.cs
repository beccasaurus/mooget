using System;
using System.Collections.Generic;

namespace MooGet {

	// All Sources (representing a group of Packages) must implement ISource 
	// for MooGet to be able to work with them
	public interface ISource {

		// This should return a Package with matching Id and Version strings, else null.
		// The Id should be case insensitive.
		Package Get(string id, string version);

		// This should return ALL of this Source's packages.
		List<Package> Packages { get; }

		// This should return only the latest versions of this Source's packages.
		List<Package> LatestPackages { get; }

		// This should return ALL packages with the given Id, eg. "NUnit".
		// The Id should be case insensitive.
		List<Package> GetPackagesWithId(string id);

		// This should return all packages matching a given PackageDependency, eg. NUnit >= 1.0
		List<Package> GetPackagesMatchingDependency(PackageDependency dependency);
	}
}
