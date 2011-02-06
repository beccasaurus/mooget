using System;
using System.IO;
using System.Linq;

namespace MooGet {

	/// <summary>Represents a single .nupkg file</summary>
	public class Nupkg : NewPackage, IPackage, IFile { // TODO : Package after we refactor ...

		public Nupkg() : base() {}
		public Nupkg(string path) : this() {
			Path = path;
		}

		string _path;
		Nuspec _nuspec;
		Zip _zip;

		/// <summary>The file system path to this .nupkg file</summary>
		public virtual string Path {
			get { return _path; }
			set { _path = value; _zip = null; _nuspec = null; }
		}

		/// <summary>Returns whether or not this Nupkg file exists (using the Path)</summary>
		public virtual bool Exists { get { return File.Exists(Path); } }

		/// <summary>This nupkg as a Zip file</summary>
		public virtual Zip Zip {
			get {
				if (_zip == null && Exists) _zip = new Zip(Path);
				return _zip;
			}
		}

		/// <summary>All of the files in this nupkg file (as a Zip file)</summary>
		public string[] Files { get { return Zip.Paths.ToArray(); } }

		/// <summary>This package's Nuspec, read out of the .nupkg zip file</summary>
		public virtual Nuspec Nuspec {
			get {
				if (_nuspec == null && Exists) _nuspec = new Nuspec { Xml = NuspecXml };
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

		#region Private
		string NuspecXml {
			get {
				if (Zip == null) return null;
				var path = Zip.Search("*.nuspec").FirstOrDefault();
				return (path == null) ? null : Zip.Read(path);
			}
		}
		#endregion
	}
}
