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
		IPackage Get(PackageDependency dependency);

		/// <summary>This should return ALL of this Source's packages.</summary>
		List<IPackage> Packages { get; }

		/// <summary>This should return only the latest versions of this Source's packages.</summary>
		/// <remarks>
		/// You can get the Latest packages from any List of Package with list.Latest(), 
		/// however, this allows for a Source to use a native, optimized method of giving us 
		/// just the latest packages.
		/// </remarks>
		List<IPackage> LatestPackages { get; }

		/// <summary>This should return ALL packages with the given Id, eg. "NUnit".</summary>
		List<IPackage> GetPackagesWithId(string id);

		/// <summary>This should return ALL packages with an Id that starts with the given string</summary>
		/// <remarks>
		/// If we start adding lots of other search functionality to ISource, we'll likely want to 
		/// extract out those methods to an ISearchableSource iterface OR just specify 1 Search() 
		/// method that takes the package properties to search and their queries.
		/// </remarks>
		List<IPackage> GetPackagesWithIdStartingWith(string query);

		/// <summary>This should return all packages matching a given PackageDependency, eg. NUnit >= 1.0</summary>
		List<IPackage> GetPackagesMatchingDependency(PackageDependency dependency);

		/// <summary>Every source should give you a way to download one of its packages to a local Nupkg.</summary>
		/// <remarks>
		/// We specify a PackageDependency because that can be as simple as "NUnit" or it could be "NUnit = 1.0.0", etc.
		/// </remarks>
		Nupkg Fetch(PackageDependency dependency, string directory);

		/// <summary>Some sources let you push your own package to them.</summary>
		/// <remarks>
		/// Returns the Source's representation of the Package if successful, else null.
		/// </remarks>
		IPackage Push(Nupkg nupkg);

		/// <summary>Some sources let you delete packages from them;</summary>
		/// <remarks>Returns true if operation was successful, else false.</remarks>
		bool Yank(PackageDependency dependency);

		/// <summary>Some sources let you specify that you want to install a package and its dependencies to them.</summary>
		/// <remarks>
		/// Returns the Source's representation of the Package if successful, else null.
		/// </remarks>
		IPackage Install(PackageDependency dependency, params ISource[] sourcesForDependencies);

		/// <summary>Some sources let you uninstall a package (and, optionally, that package's dependencies)</summary>
		/// <remarks>Returns true if operation was successful, else false.</remarks>
		bool Uninstall(PackageDependency dependency, bool uninstallDependencies);
	}
}
