using System;
using System.IO;

namespace MooGet {

	/// <summary>Represents a single .nupkg file</summary>
	public class Nupkg : NewPackage, IPackage { // TODO : Package after we refactor ...

		public Nupkg() : base() {}
		public Nupkg(string path) : this() {
			Path = path;
		}

		public virtual string Path { get; set; }

		public virtual bool Exists { get { return File.Exists(Path); } }

		public virtual Nuspec Nuspec { get { return null; } }
	}
}
