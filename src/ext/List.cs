using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Extension methods for System.Collections.Generic.List</summary>
	public static class ListExtensions {

		/// <summary>Join a list by calling ToString() on each of its elements and joining them with the given separator</summary>
		public static string Join<T>(this List<T> self, string separator) {
			return string.Join(separator, self.Select(item => item.ToString()).ToArray());
		}

		public static List<string> ToStrings<T>(this List<T> self) {
			return self.Select(o => o.SafeString()).ToList();
		}
	}
}
