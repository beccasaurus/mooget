using System;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>
	/// Represents any IPackage that is capable of inspecting 
	/// its files to get libraries, tools, content, etc
	/// </summary>
	public interface IPackageWithFiles : IPackage {

		// TODO Add Search() for searching this package's files ...?  Altho IDirectory already provides this ...

		/// <summary>All of the files in this package</summary>
		List<string> Files { get; }
		
		/// <summary>
		/// All of the executable "tools" in this package (.exe files) that are meant to be exposed 
		/// so they can easily be executed.
		/// </summary>
		List<string> Tools { get; }
		
		/// <summary>All of this package's miscellaneous "content" files, eg. templates</summary>
		List<string> Content { get; }
		
		/// <summary>
		/// All of this package's source files.  For packages that include source, this is 
		/// intended to only include the files that someone would want to reference to "include" 
		/// this package in their own.  It should not contain the source of test files, etc.
		/// </summary>
		List<string> SourceFiles { get; }

		/// <summary>All of this package's .NET assemblies.  This should return *all* of these assemblies.</summary>
		List<string> Libraries { get; }

		/// <summary>
		/// A subset of Libraries.  These are the global libraries that support any version of 
		/// the .NET framework, or the user simple didn't specify a version of the framework.
		/// </summary>
		List<string> GlobalLibraries { get; }
		
		/// <summary>
		/// A subset of Libraries.  These are the libraries that should be used for a particular 
		/// version of the .NET framework.  This should include the GlobalLibraries, so it provides 
		/// an easy way to get *all* of the libraries that should be used for this version of the framework.
		/// </summary>
		List<string> LibrariesFor(string frameworkName);

		/// <summary>
		/// A list of FrameworkName objects representing the different versions of the .NET 
		/// framework that this package has custom libraries for.
		/// </summary>
		List<FrameworkName> LibraryFrameworkNames { get; }
	}
}
