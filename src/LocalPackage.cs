using System;
using System.IO;

namespace MooGet {

	/// <summary>Represents a Package that is unpacked locally so we have full access to its files</summary>
	public class LocalPackage : Package {

		public string Path           { get; set; }
		public string NuspecFileName { get; set; }
		public string NuspecPath { get { return System.IO.Path.Combine(Path, NuspecFileName); } }
		public string NupkgFileName { get { return IdAndVersion + ".nupkg"; } }

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

		public void Uninstall() {
			File.Delete(System.IO.Path.Combine(Moo.SpecDir, IdAndVersion + ".nuspec"));
			Directory.Delete(Path, true);
			Console.WriteLine("Uninstalled {0}", IdAndVersion);
		}
	}
}
