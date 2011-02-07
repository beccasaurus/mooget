using System;
using System.IO;

namespace MooGet {

	/// <summary>Represents a file.  Any class that has a public string Path property can be an IFile</summary>
	/// <remarks>Why?  So you can use the IFileExtensions with your IFile class to Copy(), Move(), Delete(), etc the file.</remarks>
	public interface IFile {
		string Path { get; set; }
	}

	/// <summary>Concrete implementation of IFile</summary>
	public class RealFile : IFile {
		public RealFile(){}
		public RealFile(string path){ Path = path; }
		public string Path { get; set; }
		public override string ToString(){ return Path; }
	}

	/// <summary>The actual methods for an IFile</summary>
	public static class IFileExtensions {

		public static bool   Exists(this IFile file)        { return File.Exists(file.Path); }
		public static string FileName(this IFile file)      { return Path.GetFileName(file.Path); }
		public static string DirectoryName(this IFile file) { return Path.GetDirectoryName(file.Path); }
		public static string DirName(this IFile file)       { return file.DirectoryName(); }
		public static void   Delete(this IFile file)        { File.Delete(file.Path); }

		public static IFile Touch(this IFile file) {
			if (file.Exists())
				File.SetLastWriteTimeUtc(file.Path, DateTime.UtcNow);
			else
				using (var writer = new StreamWriter(file.Path)) writer.Write("");

			return file;
		}

		public static IFile Copy(this IFile file, params string[] destinationParts) {
			var destination = destinationParts.Combine();
			if (Directory.Exists(destination))
				File.Copy(file.Path, Path.Combine(destination, file.FileName()));
			else
				File.Copy(file.Path, destination);
			return file;
		}

		public static IFile Move(this IFile file, params string[] destinationParts) {
			var destination = destinationParts.Combine();
			if (Directory.Exists(destination)) {
				// move INTO the existing directory
				var newPath = Path.Combine(destination, file.FileName());
				File.Move(file.Path, newPath);
				file.Path = newPath;
			} else {
				// move to the exact destination
				File.Move(file.Path, destination);
				file.Path = destination;
			}
			return file;
		}

		public static string ToString(this IFile file) { return file.Path; }
	}
}
