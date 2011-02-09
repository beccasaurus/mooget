using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>An ISource where packages are installed to locally by MooGet.</summary>
	public class MooDir : Source, ISource, IDirectory {

		public MooDir() : base() {}
		public MooDir(string path) : this() {
			Path = path;
		}

		/// <summary>Whether or not this directory exists</summary>
		public virtual bool Exists { get { return this.Exists(); } }

		public MooDir Initialize() {
			Path.AsDir().Create();
			PackageDirectory.AsDir().Create();
			CacheDirectory.AsDir().Create();
			BinDirectory.AsDir().Create();
			return this;
		}

		public string PackageDirectory { get { return System.IO.Path.Combine(Path, "packages"); } }
		public string CacheDirectory   { get { return System.IO.Path.Combine(Path, "cache");    } }
		public string BinDirectory     { get { return System.IO.Path.Combine(Path, "bin");      } }

		// TODO cache this ... for now, we return a new one every time we call this ...
		public DirectoryOfNupkg Cache { get { return new DirectoryOfNupkg(CacheDirectory); } }

		public override List<IPackage> Packages {
			get { return PackageDirectory.AsDir().SubDirs().Select(dir => new MooDirPackage(dir.Path, this) as IPackage).ToList(); }
		}

		public override Nupkg Fetch(PackageDependency dependency, string directory){ return null; }

		public override IPackage Push(Nupkg nupkg){
			return null;

			// Install it to our cache
			var cached = Cache.Push(nupkg) as Nupkg;
			
			// Unzip to our packages
			
			// Return our MooDirPackage
		}
		
		public override bool Yank(PackageDependency dependency){ return false; }
		
		public override IPackage Install(PackageDependency dependency, params ISource[] sourcesForDependencies){ return null; }
		
		public override bool Uninstall(PackageDependency dependency, bool uninstallDependencies){ return false; }
	}
}
