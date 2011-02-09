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

		public override Nupkg Fetch(PackageDependency dependency, string directory) {
			var package = Get(dependency) as Nupkg;
			if (package != null)
				package.Copy(directory);
			return package;
		}
		
		public override IPackage Push(Nupkg nupkg) {
			if (Exists && nupkg.Exists())
				nupkg.Copy(System.IO.Path.Combine(Path, nupkg.IdAndVersion() + ".nupkg"));
			return Get(nupkg.ToPackageDependency());
		}
		
		public override bool Yank(PackageDependency dependency) {
			var package = Get(dependency) as Nupkg;
			if (package != null) {
				package.Delete();
				return true;
			}
			return false;
		}
		
		public override IPackage Install(PackageDependency dependency, params ISource[] sourcesForDependencies) {
			Console.WriteLine("DirectoryOfNupkg.Install({0})", dependency);

			var latestPackage = sourcesForDependencies.GetLatest(dependency);
			if (latestPackage == null) throw new PackageNotFoundException(dependency);

			var allPackages = latestPackage.FindDependencies(sourcesForDependencies);
			Console.WriteLine("Found Dependencies: {0}", allPackages.Select(pkg => pkg.IdAndVersion()).ToList().Join(", "));

			allPackages.Add(latestPackage);

			foreach (var package in allPackages)
				if (Get(package.ToPackageDependency()) != null)
					Console.WriteLine("Already installed: {0}", package.IdAndVersion());
				else
					Push(package as Nupkg);

			return Get(latestPackage.ToPackageDependency());
		}

		public override bool Uninstall(PackageDependency dependency, bool uninstallDependencies) {
			var nupkg = Get(dependency) as Nupkg;
			if (nupkg == null) return false;
				
			if (uninstallDependencies)
				foreach (var dep in nupkg.Details.Dependencies)
					Uninstall(dep, true);

			nupkg.Delete();
			return true;
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
