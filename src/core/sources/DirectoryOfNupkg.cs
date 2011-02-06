using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>An ISource representing a directory with .nupkg files in it (Nupkg packages)</summary>
	public class DirectoryOfNupkg : Source, ISource {

		public DirectoryOfNupkg() : base() {}
		public DirectoryOfNupkg(string path) : this() {
			Path = path;
		}

		/// <summary>Whether or not this directory exists</summary>
		public virtual bool Exists { get { return Directory.Exists(Path); } }

		/// <summary>Returns all of the Nupkg in this directory</summary>
		public override List<IPackage> Packages {
			get { return new List<IPackage>().AddPackages(DirectoryOfNupkg.GetNupkgsInDirectory(Path)); }
		}

		public override Nupkg Fetch(PackageDependency dependency) {
			return null;
		}
		
		public override IPackage Push(Nupkg nupkg) {
			if (Exists && nupkg.Exists())
				nupkg.Copy(Path);
			return Get(nupkg.ToPackageDependency());
		}
		
		public override bool Yank(PackageDependency dependency) {
			return false;
		}
		
		public override IPackage Install(PackageDependency dependency, params ISource[] sourcesForDependencies) {
			return null;
		}

		public override bool Uninstall(PackageDependency dependency, bool uninstallDependencies) {
			return false;
		}

		public static List<Nupkg> GetNupkgsInDirectory(string directory) {
			if (! Directory.Exists(directory)) return null;
			var nupkgs = new List<Nupkg>();
			foreach (var file in Directory.GetFiles(directory, "*.nupkg", SearchOption.TopDirectoryOnly))
				nupkgs.Add(new Nupkg(file));
			return nupkgs;
		}
	}
}
