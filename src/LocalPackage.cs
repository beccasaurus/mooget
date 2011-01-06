using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a Package that is unpacked locally so we have full access to its files</summary>
	public class LocalPackage : Package {

		public string Path           { get; set; }
		public string NuspecFileName { get; set; }
		public string NuspecPath     { get { return System.IO.Path.Combine(Path, NuspecFileName); } }

		// What is this for?  A package is an *unpacked* package ...
		public string NupkgFileName  { get { return IdAndVersion + ".nupkg"; } }

		public LocalPackage(string nuspecPath) {
			Path           = System.IO.Path.GetDirectoryName(nuspecPath);
			NuspecFileName = System.IO.Path.GetFileName(nuspecPath);
			LoadSpec(nuspecPath);
		}

		public void MoveInto(string directory) {
			var newPath = System.IO.Path.Combine(directory, IdAndVersion);
			Directory.Move(Path, newPath);
			Path = newPath;
		}

		/// <summary></summary>
		public void Uninstall() {
			File.Delete(System.IO.Path.Combine(Moo.SpecDir, IdAndVersion + ".nuspec"));
			Directory.Delete(Path, true);
			Console.WriteLine("Uninstalled {0}", IdAndVersion);
		}

		public string LibDirectory     { get { return System.IO.Path.Combine(Path, "lib");     } }
		public string ToolsDirectory   { get { return System.IO.Path.Combine(Path, "tools");   } }
		public string ContentDirectory { get { return System.IO.Path.Combine(Path, "content"); } }

		/// <summary>Returns all of the .dll files found in this package's LibDirectory</summary>
		public string[] Libraries {
			get { return GetFiles(LibDirectory, "*.dll"); }
		}

		/// <summary>Returns all of the .exe files found in this package's ToolsDirectory</summary>
		public string[] Tools {
			get { return GetFiles(ToolsDirectory, "*.exe"); }
		}

		/// <summary>Returns all of the files found in this package's ContentDirectory</summary>
		public string[] Content {
			get { return GetFiles(ContentDirectory, "*"); }
		}

		/// <summary>Returns all of the files found in this package</summary>
		public string[] GetFiles() {
			return GetFiles("");
		}

		/// <summary>Returns all of the files found in the given subdirectory of this package</summary>
		public string[] GetFiles(string relativeDirectory) {
			return GetFiles(relativeDirectory, "*");
		}

		/// <summary>Returns all of the files found in the given subdirectory of this package that match a particular pattern, eg. "*.dll"</summary>
		public string[] GetFiles(string relativeDirectory, string pattern) {
			var dir = System.IO.Path.Combine(Path, relativeDirectory);
			if (Directory.Exists(dir))
				return Directory.GetFiles(dir, pattern, SearchOption.AllDirectories);
			else
				return new string[] {};
		}

		/// <summary>Get all of the LocalPackage unpacked in the given directory (does not search subdirectories)</summary>
		public static List<LocalPackage> FromDirectory(string directory) {
			return AllFromDirectory(directory);
		}

		public static List<LocalPackage> FromDirectories(params string[] directories) {
			return AllFromDirectories(directories);
		}

		public static List<LocalPackage> LatestFromDirectory(string directory) {
			return LatestFromDirectories(directory);
		}

		public static List<LocalPackage> LatestFromDirectories(params string[] directories) {
			var all = AllFromDirectories(directories);
			return all.Where(pkg => pkg.Version == HighestVersionAvailableOf(pkg.Id, all)).ToList();
		}

		public static List<LocalPackage> AllFromDirectory(string directory) {
			// TODO test it directory doesn't exist
			var packages = new List<LocalPackage>();
			foreach (var dirname in Directory.GetDirectories(directory)) {
				var nuspecs = Directory.GetFiles(dirname, "*.nuspec");
				if (nuspecs.Length > 0)
					packages.Add(new LocalPackage(nuspecs[0]));
			}
			return packages;
		}

		public static List<LocalPackage> AllFromDirectories(params string[] directories) {
			var packages = new List<LocalPackage>();
			foreach (var dir in directories)
				packages.AddRange(FromDirectory(dir));
			return packages;
		}

		public static List<PackageVersion> VersionsAvailableOf(string packageId, List<LocalPackage> packages) {
			return packages.Where(pkg => pkg.Id == packageId).Select(pkg => pkg.Version).ToList();
		}

		public static PackageVersion HighestVersionAvailableOf(string packageId, List<LocalPackage> packages) {
			return PackageVersion.HighestVersion(VersionsAvailableOf(packageId, packages).ToArray());
		}
	}
}
