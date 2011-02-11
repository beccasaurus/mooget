using System;

namespace MooGet {

	/// <summary>Represents an IPackage as a file (eg. a nupkg or a zip file or SOME kind of local file)</summary>
	/// <remarks>
	///	The most used IPackageFile implementation is Nupkg.
	///
	///	Why do we have an interface?
	///
	///	Well, we might want to eventually support OpenWrap or Nu or ... ? ... packages.
	/// </remarks>
	public interface IPackageFile : IPackage, IFile {

		/// <summary>An IPackageFile can be unpacked into an IUnpackedPackage (eg. .nupkg -> directory)</summary>
		IUnpackedPackage Unpack(string directory);
	}
}
