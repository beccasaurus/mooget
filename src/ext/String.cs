using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Extension methods for System.String</summary>
	public static class StringExtensions {

		/// <summary>Split a string with a char separator and return the results as a List</summary>
		public static List<string> ToList(this string self, char separator) {
			if (self == null) return new List<string>();
			return new List<string>(self.Split(separator));
		}

		/// <summary>Split a string with a char separator, trim each of the parts, and return the results as a List</summary>
		public static List<string> ToTrimmedList(this string self, char separator) {
			if (self == null) return new List<string>();
			return self.ToList(separator).Select(str => str.Trim()).ToList();
		}
	}
}
