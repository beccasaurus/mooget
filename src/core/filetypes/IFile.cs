using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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

		/// <summary>returns the IFile.Path, if it exists, else null</summary>
		public static string Path(this IFile file) {
			return (file == null) ? null : file.Path;
		}

		public static bool   Exists(this IFile file)        { return File.Exists(file.Path); }
		public static string Name(this IFile file)          { return file.FileName(); }
		public static string FileName(this IFile file)      { return System.IO.Path.GetFileName(file.Path); }
		public static string DirectoryName(this IFile file) { return System.IO.Path.GetDirectoryName(file.Path); }
		public static string DirName(this IFile file)       { return file.DirectoryName(); }
		public static void   Delete(this IFile file)        { File.Delete(file.Path); }

		public static string Read(this IFile file) {
			return File.ReadAllText(file.Path);
		}

		public static List<string> Lines(this IFile file) {
			return file.Read().Split('\n').ToList();
		}

		public static void Append(this IFile file, string text) {
			using (var writer = new StreamWriter(file.Path, true)) writer.Write(text);
		}

		public static void AppendLine(this IFile file, string text) {
			using (var writer = new StreamWriter(file.Path, true)) writer.WriteLine(text);
		}

		public static void Write(this IFile file, string text) {
			using (var writer = new StreamWriter(file.Path)) writer.Write(text);
		}

		public static void WriteLine(this IFile file, string text) {
			using (var writer = new StreamWriter(file.Path)) writer.WriteLine(text);
		}

		/// <summary>If the file doesn't exist, it'll initialize it with this text, else it won't do anything</summary>
		public static IFile Initialize(this IFile file, string text) {
			if (! file.Exists())
				file.Write(text);
			return file;
		}

		public static IFile Touch(this IFile file) {
			if (file.Exists())
				File.SetLastWriteTimeUtc(file.Path, DateTime.UtcNow);
			else
				using (var writer = new StreamWriter(file.Path)) writer.Write("");

			return file;
		}

		public static IFile Copy(this IFile file, params string[] destinationParts) {
			var destination = destinationParts.Combine();
			if (Directory.Exists(destination)) {
				var newPath = System.IO.Path.Combine(destination, file.FileName());
				File.Copy(file.Path, newPath, true);
				return newPath.AsFile();
			} else {
				File.Copy(file.Path, destination, true);
				return destination.AsFile();
			}
		}

		public static IFile Move(this IFile file, params string[] destinationParts) {
			var destination = destinationParts.Combine();
			if (Directory.Exists(destination)) {
				// move INTO the existing directory
				var newPath = System.IO.Path.Combine(destination, file.FileName());
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

	public static class ListOfIFileExtensions {
		public static List<string> Paths(this List<IFile> files) {
			return files.Select(file => file.Path).ToList();
		}
	}
}
