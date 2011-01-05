using System;
using System.Collections.Generic;

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

		public string[] Parts { get { return ToString().Split('.'); } }

		public int CompareTo(object o) {
			var a = this;
			var b = (PackageVersion) o;

			if (a.ToString() == b.ToString())
				return 0;

			var aParts = new List<string>(a.Parts);
			var bParts = new List<string>(b.Parts);

			while (true) {

				// if a and b are both out of parts, they must be equal
				if (aParts.Count == 0 && bParts.Count == 0)
					return 0;

				// if a is out of parts, but b isn't, a is greater
				if (aParts.Count == 0 && bParts.Count > 0)
					return -1;

				// if b is out of parts, but a isn't, b is greater
				if (aParts.Count > 0 && bParts.Count == 0)
					return 1;

				// compare the first part for each a and b
				var aNumber = int.Parse(aParts[0]);
				var bNumber = int.Parse(bParts[0]);

				if (aNumber > bNumber)
					return 1;
				else if (aNumber < bNumber)
					return -1;

				// if we didn't return, remove the first part from a and b
				aParts.RemoveAt(0);
				bParts.RemoveAt(0);
			}
		}

		public override string ToString() {
			return _version;
		}
	}
}
