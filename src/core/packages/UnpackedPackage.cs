using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

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
		public string[] Files { get { return this.Files().Select(file => file.Path).ToArray(); } }

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
	}
}
