using System;
using System.IO;
using System.Text;
using System.Linq;
using System.IO.Packaging;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {

	/// <summary>Represents a single .zip file (or .nupkg file, or any valid Zip package file)</summary>
	public class Zip : IFile {

		public Zip(){}
		public Zip(string path) : this() {
			Path = path;
		}

		string _path;
		List<string> _paths;

		/// <summary>The file system Path to this .zip file</summary>
		public virtual string Path {
			get { return _path; }
			set { _path = value; _paths = null; }
		}

		public virtual bool Exists { get { return File.Exists(Path); } }

		/// <summary>Returns a list of paths matching the given string (case insensitive)</summary>
		public virtual List<string> Search(string matcher) { return Search(matcher, true); }

		/// <summary>Returns a list of paths matching the given string, eg "*.zip" or "lib\**\*.dll"</summary>
		/// <remarks>
		/// Supports wildcards:
		///  - ** means anything
		///  - * means anything that's NOT a slash
		/// </remarks>
		public virtual List<string> Search(string matcher, bool ignoreCase) {
			// We have to initially substitude ** out for something besides a single * because then we replace single *'s.
			matcher   = "^" + matcher.Replace("\\", "/").Replace(".", "\\.").Replace("**", ".REAL_REGEX_STAR").Replace("*", @"[^\/]+").Replace("REAL_REGEX_STAR", "*") + "$";
			var regex = ignoreCase ? new Regex(matcher, RegexOptions.IgnoreCase) : new Regex(matcher);
			return Search(regex);
		}

		/// <summary>Returns a list of paths matching the given regular express</summary>
		public virtual List<string> Search(Regex regex) {
			return Paths.Where(path => regex.IsMatch(path)).ToList();
		}

		/// <summary>Returns all of the paths inside of this zip file</summary>
		public virtual List<string> Paths {
			get {
				if (_paths == null && Exists) _paths = Zip.GetPaths(this);
				return _paths;
			}
		}

		/// <summary>Reads the content of the given path in the zip file and returns it as a string</summary>
		public virtual string Read(string path) {
			if (Paths == null || ! Paths.Contains(path)) return null;
			using (var package = System.IO.Packaging.Package.Open(Path, FileMode.Open, FileAccess.Read))
				foreach (var part in package.GetParts())
					if (part.Uri.OriginalString.Substring(1) == path)
						using (var reader = new StreamReader(part.GetStream()))
							return reader.ReadToEnd();
			return null;
		}

		/// <summary>Adds a new item to this zip file, given the provided content.  MimeType will be guessed using the file extension.</summary>
		public virtual void AddNew(string path, string content) {
			AddNew(path, content, Util.MimeTypeFor(path));
		}

		/// <summary>Adds a new item to this zip file, given the provided content and mime type</summary>
		/// <remarks>Content is UTF8 encoded.  To encode yourself, use AddNew(path, byte[], mimeType)</remarks>
		public virtual void AddNew(string path, string content, string mimeType) {
			AddNew(path, new UTF8Encoding().GetBytes(content), mimeType);
		}

		/// <summary>Adds a new item to this zip file, given the provided content and mime type</summary>
		public virtual void AddNew(string path, byte[] content, string mimeType) {
			using (var package = ZipPackage.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				var uri  = new Uri("/" + path.TrimStart(@"\/".ToCharArray()), UriKind.Relative);
				var part = package.CreatePart(uri, mimeType, CompressionOption.Maximum);
				part.GetStream().Write(content, 0, content.Length);
			}
		}

		/// <summary>Adds an existing file to this zip file, given the file's path.  MimeType will be guessed using the file extension.</summary>
		/// <remarks>We use the existing file path to determine the MimeType.  Pass MimeType manually to override.</remarks>
		public virtual void AddExisting(string path, string existingFilePath) {
			AddExisting(path, existingFilePath, Util.MimeTypeFor(existingFilePath));
		}

		/// <summary>Adds an existing file to this zip file, given the file's path and mime type</summary>
		public virtual void AddExisting(string path, string existingFilePath, string mimeType) {
			AddNew(path, File.ReadAllBytes(existingFilePath), mimeType);
		}

		/// <summary>Adds every file in the given directory to the root of this archive</summary>
		public virtual void AddExistingDirectory(string existingDirectory) {
			AddExistingDirectory("", existingDirectory);
		}

		/// <summary>Adds every file in the given directory to this archive inside of the given path</summary>
		public virtual void AddExistingDirectory(string path, string existingDirectory) {
			foreach (var filePath in existingDirectory.AsDir().Files().Paths()) {
				var relative = filePath.Replace(existingDirectory, "").TrimStart('/');
				if (! IsMetaFile(relative)) {
					AddExisting(relative, filePath);
				}
			}
		}

		/// <summary>Extracts this zip file to the target directory</summary>
		public virtual void Extract(string targetDirectory) {
			if (Directory.Exists(targetDirectory)) {
				// make a subdirectory using this Zip file's name (without extension)
				var dir = System.IO.Path.Combine(targetDirectory, System.IO.Path.GetFileNameWithoutExtension(Path));
				Directory.CreateDirectory(dir);
				ExtractInto(dir);
			} else {
				// make the given directory and extract into it
				Directory.CreateDirectory(targetDirectory);
				ExtractInto(targetDirectory);
			}
		}

		/// <summary>Extracts this Zip's files *into* the target directory without making a new directory</summary>
		public virtual void ExtractInto(string targetDirectory) {
			using (var zip = System.IO.Packaging.Package.Open(Path, FileMode.Open, FileAccess.Read)) {
				foreach (var part in zip.GetParts()) {
					var path = part.Uri.OriginalString.Substring(1);

					// skip these stupid files
					if (path == "[Content_Types].xml" || path == "_rels/.rels" || path.EndsWith(".psmdcp"))
						continue;

					using (var stream = part.GetStream(FileMode.Open, FileAccess.Read)) {

						// Create a path and make sure the directory for it gets created
						var filepath = System.IO.Path.Combine(targetDirectory, path);
						Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filepath));

						// Copy the part Stream into a file Stream
						using (var file = File.OpenWrite(filepath)) {
							var buffer = new byte[8 * 1024];
							int len;
							while ( (len = stream.Read(buffer, 0, buffer.Length)) > 0)
								file.Write(buffer, 0, len);
						}
					}
				}
			}
		}

		public static List<string> GetPaths(Zip zip) { return GetPaths(zip, true); }
		public static List<string> GetPaths(Zip zip, bool ignoreMetaFiles) {
			var paths = new List<string>();
			if (! zip.Exists) return paths;

			// Right now, we're using the System.IO.Packaging namespace which comes with .NET 3.5.
			// We *might* compile something like SharpZipLib into our project for greater compatibility.
			// Nomatter what, we should *create* Nupkg files using System.IO.Packaging so the official 
			// NuGet tools can read them.  But we might support *reading* any kind of zip file?
			using (var package = System.IO.Packaging.Package.Open(zip.Path, FileMode.Open, FileAccess.Read))
				foreach (var part in package.GetParts())
					paths.Add(part.Uri.OriginalString.Substring(1)); // removes the beginning slash from the uri

			return ignoreMetaFiles ? Zip.RemoveMetaFiles(paths) : paths;
		}

		/// <summary>Removes the stupid meta files that System.IO.Packaging Packages have, eg. "_rels/.rels'</summary>
		public static List<string> RemoveMetaFiles(List<string> paths) {
			var clean = new List<string>();
			foreach (var path in paths) {
				if (IsMetaFile(path)) continue;
				clean.Add(path);
			}
			return clean;
		}

        /// <summary>Returns whether or not the given path is for one of the meta files that Systen.IO.Packaging Packages have</summary>
        public static bool IsMetaFile(string path) {
			if (path == "_rels/.rels") return true;
			if (path == "[Content_Types].xml") return true;
			if (path.StartsWith("package/services/metadata/core-properties") && path.EndsWith(".psmdcp")) return true;
			return false;
        }  

		/// <summary>Zips up the given directory into a Zip file, returning the Zip instance represeting the created file.</summary>
		public static Zip Archive(string directory, string zipFileName) {
			var zip = new Zip(zipFileName);
			zip.AddExistingDirectory(directory);
			return zip;
		}
	}
}
