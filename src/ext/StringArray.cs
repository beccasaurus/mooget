using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {
	public static class StringArrayExtensions {

		public static string Combine(this string[] pathParts) {
			var list = new List<string>(pathParts);
			var path = list.First();
			list.RemoveAt(0);
			foreach (var part in list)
				path = Path.Combine(path, part);
			return Path.GetFullPath(path);
		}
	}
}
