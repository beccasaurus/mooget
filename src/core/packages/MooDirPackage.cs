using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a package installed in a MooDir (ISource), eg. every package "installed" by MooGet</summary>
	/// <remarks>
	///	Because a package installed in a MooDir is really just an UnpackedPackage, we inherit from that.
	///
	///	A MooDirPackage, however, also knows about its cached Nupkg, its tools in the MooDir.BinDirectory, etc etc.
	/// </remarks>
	public class MooDirPackage: UnpackedPackage, IPackage, IDirectory {

		public MooDirPackage() : base() {}
		public MooDirPackage(string path) : this() {
			Path = path;
		}
		public MooDirPackage(string path, MooDir mooDir) : this(path) {
			MooDir = mooDir;
		}

		Nupkg _nupkg;

		/// <summary>The MooDir that this package is associated with</summary>
		public MooDir MooDir { get; set; }

		public override ISource Source { get { return MooDir; } }

		/// <summary>The UnpackedPackage in the MooDir that this MooDirPackage wraps</summary>
		public UnpackedPackage Unpacked { get { return this as UnpackedPackage; } }

		/// <summary>A cached .nupkg in the MooDir's CacheDirectory.  May be null, but typically shouldn't be.</summary>
		public Nupkg Nupkg {
			get {
				if (_nupkg == null) _nupkg = MooDir.Cache.Get(this.ToPackageDependency()) as Nupkg;
				return _nupkg;
			}
		}
	}
}
