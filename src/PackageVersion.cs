using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a version, eg. 1.0 or 2.1.15.6439</summary>
	/// <remarks>
	/// It's important for MooGet to be able to easily compare versions and figure out 
	/// which version is higher than another version, etc, so we have PackageVersion 
	/// to help us with this because the comparison logic could get a bit hairy.
	///
	/// PackageVersion represents a particular version, eg. 1.0
	///
	/// If you need to see if a version matches a dependency, eg. "&gt;= 1.0.0" 
	/// then you should look at PackageDependency
	/// </remarks>
	public class PackageVersion : IComparable {

		string _version;

		public PackageVersion(string version) {
			_version = version;
		}

		public static bool operator <  (PackageVersion a, PackageVersion b) { return a.CompareTo(b) == -1;  }
		public static bool operator >  (PackageVersion a, PackageVersion b) { return a.CompareTo(b) == 1;   }
		public static bool operator != (PackageVersion a, PackageVersion b) { return (a == b) == false;     }
		public static bool operator <= (PackageVersion a, PackageVersion b) { return (a == b || a < b);     }
		public static bool operator >= (PackageVersion a, PackageVersion b) { return (a == b || a > b);     }

		public static bool operator == (PackageVersion a, PackageVersion b) {
			if ((object)b == null)
				return (object)a == null;
			else
				return a.CompareTo(b) == 0;
		}

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

		/// <summary>
		/// 1.0.1 is sorta greater than 1.0.0 but 1.1 is NOT because it is TOO much greater than it (first/second parts cannot be bigger)
		/// </summary>
		public bool SortaGreaterThan(PackageVersion b) {
			// if this is not greater than or equal to b, it can never be "sorta" greater than!
			if (! (this >= b)) return false;

			// ok, so now that we know this IS greater than or equal to b, let's look at the first 2 parts of the version
			var aParts = new List<string>(this.Parts);
			var bParts = new List<string>(b.Parts);

			// add .0 to the version numbers to give us atleast 2 parts for each, so 1 becomes 1.0
			while (aParts.Count < 2) aParts.Add("0");
			while (bParts.Count < 2) bParts.Add("0");
			
			// so, is "a" sorta greater than "b"?  check the first 2 parts.
			// if either of the first 2 parts are greater than b's first 2 parts, we are NOT sorta greater than!
			var aMajorVersion = int.Parse(aParts[0]);
			var aMinorVersion = int.Parse(aParts[1]);
			var bMajorVersion = int.Parse(bParts[0]);
			var bMinorVersion = int.Parse(bParts[1]);

			if (aMajorVersion > bMajorVersion || aMinorVersion > bMinorVersion)
				return false;
			else
				return true; // looks like we're good!
		}

		// TODO DRY this and SortaGreaterThan?  They're basically copy/pasted!
		/// <summary>
		/// 1.1.0 is sorta less than 1.1.5 but 1.0.0 is NOT because it is TOO much less than it (first/second parts cannot be bigger)
		/// </summary>
		/// <remarks>
		/// SortaGreaterThan is very useful for dependencies.  I think SortaLessThan is pretty useless but we might as well support it 
		/// if we support SortaGreaterThan.
		/// </remarks>
		public bool SortaLessThan(PackageVersion b) {
			// if this is not less than or equal to b, it can never be "sorta" less than!
			if (! (this <= b)) return false;

			// ok, so now that we know this IS greater than or equal to b, let's look at the first 2 parts of the version
			var aParts = new List<string>(this.Parts);
			var bParts = new List<string>(b.Parts);

			// add .0 to the version numbers to give us atleast 2 parts for each, so 1 becomes 1.0
			while (aParts.Count < 2) aParts.Add("0");
			while (bParts.Count < 2) bParts.Add("0");
			
			// so, is "a" sorta less than "b"?  check the first 2 parts.
			// if either of the first 2 parts are less than b's first 2 parts, we are NOT sorta less than!
			var aMajorVersion = int.Parse(aParts[0]);
			var aMinorVersion = int.Parse(aParts[1]);
			var bMajorVersion = int.Parse(bParts[0]);
			var bMinorVersion = int.Parse(bParts[1]);

			if (aMajorVersion < bMajorVersion || aMinorVersion < bMinorVersion)
				return false;
			else
				return true; // looks like we're good!
		}

		public override string ToString() {
			return _version;
		}

		public static PackageVersion HighestVersion(params string[] versions) {
			return HighestVersion(versions.Select(str => new PackageVersion(str)).ToArray());
		}

		public static PackageVersion HighestVersion(params PackageVersion[] versions) {
			return new List<PackageVersion>(versions).Max();
		}
	}
}
