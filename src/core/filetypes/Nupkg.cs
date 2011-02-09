using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a single .nupkg file</summary>
	public class Nupkg : NewPackage, IPackage, IFile { // TODO : Package after we refactor ...

		public Nupkg() : base() {}
		public Nupkg(string path) : this() {
			Path = path;
		}

		ISource _source;
		string _path;
		Nuspec _nuspec;
		Zip _zip;

		// TODO why do we need to reset the zip and spec when we change the path?  I set it when we SET it ...
		/// <summary>The file system path to this .nupkg file</summary>
		public virtual string Path {
			get { return _path; }
			set { _path = value; _zip = null; _nuspec = null; }
		}

		public override ISource Source {
			get { return _source;  }
			set { _source = value; }
		}

		/// <summary>Returns whether or not this Nupkg file exists (using the Path)</summary>
		public virtual bool Exists { get { return this.Exists(); } }

		/// <summary>This nupkg as a Zip file</summary>
		public virtual Zip Zip {
			get {
				if (_zip == null && Exists) _zip = new Zip(Path);
				return _zip;
			}
		}

		/// <summary>All of the files in this nupkg file (as a Zip file)</summary>
		public List<string> Files { get { return Zip.Paths; } }

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

		/// <summary>Extracts this .nupkg (as a Zip file) to the target directory, returning an UnpackedPackage representing this directory</summary>
		public UnpackedPackage Unpack(string targetDirectory) {
			if (Directory.Exists(targetDirectory)) {
				// make a subdirectory using this Zip file's name (without extension)
				var dir = System.IO.Path.Combine(targetDirectory, this.IdAndVersion());
				Directory.CreateDirectory(dir);
				return UnpackInto(dir);
			} else {
				// make the given directory and extract into it
				Directory.CreateDirectory(targetDirectory);
				return UnpackInto(targetDirectory);
			}
		}

		/// <summary>Unpacks this Nupkg *into* the target directory without making a new directory</summary>
		public UnpackedPackage UnpackInto(string targetDirectory) {
			Zip.ExtractInto(targetDirectory);
			return new UnpackedPackage(targetDirectory);
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
