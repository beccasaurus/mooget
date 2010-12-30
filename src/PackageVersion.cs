using System;

namespace MooGet {

	/// <summary>Represents a version, eg. 1.0 or 2.1.15.6439</summary>
	/// <remarks>
	/// It's important for MooGet to be able to easily compare versions and figure out 
	/// which version is higher than another version, etc, so we have PackageVersion 
	/// to help us with this because the comparison logic could get a bit hairy.
	/// </remarks>
	public class PackageVersion : IComparable {

		string _version;

		public PackageVersion(string version) {
			_version = version;
		}

		public static bool operator < (PackageVersion a, PackageVersion b) { return a.CompareTo(b) == -1; }
		public static bool operator > (PackageVersion a, PackageVersion b) { return a.CompareTo(b) == 1; }

		public int CompareTo(object o) {
			var anotherVersion = (PackageVersion) o;
			return 0; // always return equal to ... just cause ... until we write some simple specs for PackageVersion
		}

		public override string ToString() {
			return _version;
		}
	}
}
