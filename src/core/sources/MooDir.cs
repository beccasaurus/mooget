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

		public override IPackageFile Fetch(PackageDependency dependency, string directory){
			var package = Get(dependency) as MooDirPackage;
			if (package == null) throw new PackageNotFoundException(dependency);

			var nupkg = package.Nupkg;
			if (nupkg == null) nupkg = package.CreateNupkg();

			var newFile = nupkg.Copy(directory);
			return new Nupkg(newFile.Path);
		}

		public override IPackage Push(IPackageFile file){
			// Install it to our cache
			var cached = Cache.Push(file) as Nupkg;
			
			// Unzip to our packages
			var unpacked = cached.Unpack(PackageDirectory);

			var package = Get(unpacked.ToPackageDependency()) as MooDirPackage;

			// If this package has tool and this is the highest version of this package, install its tools
			if (unpacked.Tools.Count > 0)
				if (this.HighestVersionAvailableOf(package.Id) == package.Version)
					SetupBinariesFor(package);
			
			// Return our MooDirPackage
			return package;
		}
		
		public override bool Yank(PackageDependency dependency){
			var package = Get(dependency) as MooDirPackage;
			if (package == null) return false;
			var unpacked = package.Unpacked;

			// If this has tools and was the highest version of this package, delete the tools
			if (unpacked.Tools.Count > 0)
				if (this.HighestVersionAvailableOf(package.Id) == package.Version)
					DeleteBinariesFor(package);

			// Delete our UnpackedPackage directory
			unpacked.Delete();

			return true;
		}
		
		public virtual void SetupBinariesFor(MooDirPackage package) {
			foreach (var exe in package.Tools) {
				SetupUnixShellScriptForTool(exe,    BinDirectory);
				SetupWindowsBatchScriptForTool(exe, BinDirectory);
			}
		}

		public virtual void SetupUnixShellScriptForTool(string exePath, string binDirectory) {
			var name = System.IO.Path.GetFileNameWithoutExtension(exePath);
			var path = System.IO.Path.Combine(binDirectory, name);

			// #! /bin/bash
			// "/home/user/.moo/packages/Foo-1.2-3/tools/MyTool.exe" "$@"
			var script = string.Format("#! /bin/sh\n\"{0}\" \"$@\"", exePath.Replace(" ", "\\ "));

			using (var writer = new StreamWriter(path)) writer.Write(script);
		}

		public virtual void SetupWindowsBatchScriptForTool(string exePath, string binDirectory) {
			var name = System.IO.Path.GetFileNameWithoutExtension(exePath);
			var path = System.IO.Path.Combine(binDirectory, name + ".bat");

			// @ECHO OFF
			// IF NOT "%~f0" == "~f0" GOTO :WinNT
			// @"C:/Users/rtaylor/.moo/packages/Foo-1.2-3/tools/MyTool.exe" %1 %2 %3 %4 %5 %6 %7 %8 %9
			// GOTO :EOF
			// :WinNT
			// @"C:/Users/rtaylor/.moo/packages/Foo-1.2-3/tools/MyTool.exe" %*
			var script = string.Format(@"@ECHO OFF
IF NOT ""%~f0"" == ""~f0"" GOTO :WinNT
@""{0}"" %1 %2 %3 %4 %5 %6 %7 %8 %9
GOTO :EOF
:WinNT
@""{0}"" %*", path);	

			using (var writer = new StreamWriter(path)) writer.Write(script);
		}

		public virtual void DeleteBinariesFor(MooDirPackage package) {
			foreach (var exe in package.Tools) {
				var name = System.IO.Path.GetFileNameWithoutExtension(exe);
				var unix = System.IO.Path.Combine(BinDirectory, name);
				var bat  = System.IO.Path.Combine(BinDirectory, name + ".bat");
				unix.AsFile().Delete();
				bat.AsFile().Delete();
			}
		}
	}
}
