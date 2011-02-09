using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {
	public static class ListOfStringExtensions {

		/// <summary>Assuming we have a list of strings representing file paths, return a List of IFile</summary>
		public static List<IFile> AsFiles(this List<string> paths) {
			return paths.Select(path => path.AsFile()).ToList();
		}

		/// <summary>Assuming we have a list of strings representing directory paths, return a List of IDirectory</summary>
		public static List<IDirectory> AsDirs(this List<string> paths) {
			return paths.Select(path => path.AsDir()).ToList();
		}
	}
}
