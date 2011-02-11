using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>An ISource representing a directory with .nupkg files in it (Nupkg packages)</summary>
	public class DirectoryOfNupkg : Source, ISource, IDirectory {

		public DirectoryOfNupkg() : base() {}
		public DirectoryOfNupkg(string path) : this() {
			Path = path;
		}

		/// <summary>Whether or not this directory exists</summary>
		public virtual bool Exists { get { return this.Exists(); } }

		/// <summary>Returns all of the Nupkg in this directory</summary>
		public override List<IPackage> Packages {
			get {
				var packages = new List<IPackage>().AddPackages(DirectoryOfNupkg.GetNupkgsInDirectory(Path));
				packages.ForEach(p => (p as Nupkg).Source = this);
				return packages;
			}
		}

		public override IPackageFile Fetch(PackageDependency dependency, string directory) {
			var package = Get(dependency) as IPackageFile;
			if (package != null)
				package.Copy(directory);
			return package;
		}
		
		public override IPackage Push(IPackageFile file) {
			if (Exists && file.Exists())
				file.Copy(System.IO.Path.Combine(Path, file.IdAndVersion() + ".nupkg"));

			return Get(file.ToPackageDependency());
		}
		
		public override bool Yank(PackageDependency dependency) {
			var package = Get(dependency) as Nupkg;
			if (package != null) {
				package.Delete();
				return true;
			}
			return false;
		}

		public static List<Nupkg> GetNupkgsInDirectory(string directory) {
			if (! Directory.Exists(directory)) return null;
			var nupkgs = new List<Nupkg>();
			foreach (var file in Directory.GetFiles(directory, "*.nupkg", SearchOption.TopDirectoryOnly))
				nupkgs.Add(new Nupkg(file));
			return nupkgs;
		}

		/// <summary>Returns true if the given path looks like a valid DirectoryOfNupkg, else false</summary>
		public static bool IsValidPath(string path) {
			return Directory.Exists(path);
		}
	}
}
