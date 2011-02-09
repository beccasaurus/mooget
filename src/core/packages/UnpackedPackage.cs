using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>A .nupkg that has been extracted to a directory.  This represents the directory.</summary>
	public class UnpackedPackage: NewPackage, IPackage, IDirectory {

		public UnpackedPackage() : base() {}
		public UnpackedPackage(string path) : this() {
			Path = path;
		}

		Nuspec _nuspec;

		/// <summary>The file system path to this directory</summary>
		public virtual string Path { get; set; }

		/// <summary>Whether or not this directory exists</summary>
		public virtual bool Exists { get { return this.Exists(); } }

		/// <summary>Returns the path to this package's .nuspec file or null, if not found.</summary>
		public virtual string NuspecPath {
			get {
				if (! Exists)       return null;
				if (_nuspec != null) return _nuspec.Path;
				var file = this.Search("*.nuspec").FirstOrDefault();
				return (file == null) ? null : file.Path;
			}
		}

		/// <summary>All of the files in thie directory</summary>
		public virtual List<string> Files { get { return this.Files().Select(file => file.Path).ToList(); } }

		/// <summary>This UnpackedPackage's Nuspec, read from the .nuspec file</summary>
		public virtual Nuspec Nuspec {
			get {
				if (_nuspec == null && Exists && NuspecPath != null) _nuspec = new Nuspec(NuspecPath);
				return _nuspec;
			}
		}

		public override PackageDetails Details { get { return Nuspec; } }

		public override string Id {
			get { return Nuspec.Id;  }
			set { Nuspec.Id = value; }
		}

		public override PackageVersion Version {
			get { return Nuspec.Version;  }
			set { Nuspec.Version = value; }
		}

		public virtual string LibrariesDirectory { get { return RootCaseInsensitiveDir("lib");     } }
		public virtual string ToolsDirectory     { get { return RootCaseInsensitiveDir("tools");   } }
		public virtual string ContentDirectory   { get { return RootCaseInsensitiveDir("content"); } }
		public virtual string SourceDirectory    { get { return RootCaseInsensitiveDir("src");     } }

		/// <summary>Returns any FrameworkName that this package has specific libraries for</summary>
		public virtual List<FrameworkName> LibraryFrameworkNames {
			get { return LibrariesDirectory.AsDir().SubDirs().Select(dir => FrameworkName.Parse(dir.Name())).ToList(); }
		}

		/// <summary>Returns the path to the lib directory for the given framework name, eg. "Net20".  Or null if there isn't one.</summary>
		public virtual string LibraryDirectoryFor(string frameworkName) {
			var framework = FrameworkName.Parse(frameworkName);
			return LibrariesDirectory.AsDir().SubDirs().FirstOrDefault(dir => FrameworkName.Parse(dir.Name()) == framework).Path();
		}

		/// <summary>Returns the paths to all of this package's DLLs for the given framework name, eg. "Net20".  Includes global DLLs too.</summary>
		public virtual List<string> LibrariesFor(string frameworkName) {
			var libraries = GlobalLibraries;
			libraries.AddRange(JustLibrariesFor(frameworkName));
			return libraries;
		}

		/// <summary>Returns the paths to all of this package's DLLs for the given framework name, eg. "Net20".  Does not include global DLLs.</summary>
		public virtual List<string> JustLibrariesFor(string frameworkName) {
			var dir = LibraryDirectoryFor(frameworkName);
			return (dir == null) ? new List<string>() : dir.AsDir().Search("**.dll").Paths();
		}

		/// <summary>Returns just the DLLs in the root of the LibrariesDirectory (not categorized by framework version)</summary>
		public virtual List<string> GlobalLibraries { get { return LibrariesDirectory.AsDir().Search("*.dll").Paths(); } }

		/// <summary>Returns ALL DLLs for this package.  All DLLs in the LibrariesDirectory, regardless of framework version</summary>
		public virtual List<string> Libraries { get { return (LibrariesDirectory == null) ? new List<string>() : LibrariesDirectory.AsDir().Search("**.dll").Paths(); } }

		/// <summary>Returns ALL EXEs for this package founr in the tools directory.</summary>
		public virtual List<string> Tools { get { return (ToolsDirectory == null) ? new List<string>() : ToolsDirectory.AsDir().Search("**.exe").Paths(); } }

		/// <summary>Returns ALL content files for this package.  Everything that's in this package's content directory (if anything).</summary>
		public virtual List<string> Content { get { return (ContentDirectory == null) ? new List<string>() : ContentDirectory.AsDir().Files().Paths(); } }

		/// <summary>Returns ALL source files for this package.  Everythign that's in this package's src directory (if anything).</summary>
		public virtual List<string> SourceFiles { get { return (SourceDirectory == null) ? new List<string>() : SourceDirectory.AsDir().Files().Paths(); } }

		/// <summary>Creates a .nupkg file from this UnpackedPackage directory and saves it to the given directory or filename</summary>
		public virtual Nupkg CreateNupkg(string directoryOrFilename) {
			if (Directory.Exists(directoryOrFilename))
				return CreateNupkgFile(System.IO.Path.Combine(directoryOrFilename, this.IdAndVersion() + ".nupkg"));
			else
				return CreateNupkgFile(directoryOrFilename);
		}

		/// <summary>Creates a .nupkg file from this UnpackedPackage and saves it to the exact filename that is passed in</summary>
		public virtual Nupkg CreateNupkgFile(string filename) {
			var zip = Zip.Archive(Path, filename);
			return new Nupkg(zip.Path);
		}

		#region Private
		string RootCaseInsensitiveDir(string name) {
			return this.SubDirs().FirstOrDefault(dir => dir.Name().ToLower() == name.ToLower()).Path();
		}
		#endregion
	}
}
