using System;
using System.IO;
using System.Linq;

namespace MooGet {

	/// <summary>Represents a single .nupkg file</summary>
	public class Nupkg : NewPackage, IPackage { // TODO : Package after we refactor ...

		public Nupkg() : base() {}
		public Nupkg(string path) : this() {
			Path = path;
		}

		string _path;
		Nuspec _nuspec;
		Zip _zip;

		public virtual string Path {
			get { return _path; }
			set { _path = value; _zip = null; _nuspec = null; }
		}

		public virtual bool Exists { get { return File.Exists(Path); } }

		public virtual Zip Zip {
			get {
				if (_zip == null && Exists) _zip = new Zip(Path);
				return _zip;
			}
		}

		public virtual Nuspec Nuspec {
			get {
				if (_nuspec == null && Exists) _nuspec = new Nuspec(NuspecXml);
				return _nuspec;
			}
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
