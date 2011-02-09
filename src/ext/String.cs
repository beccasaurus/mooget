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

		/// <summary>Assuming that this string represents the path to a file, returns an IFile</summary>
		public static IFile AsFile(this string self) {
			return new RealFile(self);
		}

		/// <summary>Assuming that this string represents the path to a directory, returns an IDirectory</summary>
		public static IDirectory AsDir(this string self) {
			return new RealDirectory(self);
		}
	}
}
