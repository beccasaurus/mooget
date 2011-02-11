using System;

namespace MooGet {

	/// <summary>Represents an IPackage as a directory with all of its files extracted</summary>
	/// <remarks>
	/// The most used IUnpackedPackage implementation is UnpackedNupkg.
	///
	/// Why do we have an interface?
	///
	///	Well, we might want to eventually support OpenWrap or Nu or ... ? ... packages.
	///
	///	If you implement IUnpackedPackage, you MUST implement IPackageWithFiles. 
	///	We assume that, if your package is unpacked, you can inspect the files.
	/// </remarks>
	public interface IUnpackedPackage : IPackage, IPackageWithFiles, IDirectory {

		/// <summary>An IUnpackedPackage can be packed into an IPackageFile (eg. directory -> .nupkg)</summary>
		IPackageFile Pack(string file);
	}
}
