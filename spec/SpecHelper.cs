using System;
using System.IO;
using NUnit.Framework;

namespace MooGet.Specs {

	/// <summary>Base class for MooGet specs.  Provides helper methods.</summary>
	public class MooGetSpec {

		public static string SpecDirectory {
			get { return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../spec")); }
		}

		public static string ContentDirectory {
			get { return Path.GetFullPath(Path.Combine(SpecDirectory, "content")); }
		}

		public static string PathToContent(string filename) {
			return Path.GetFullPath(Path.Combine(ContentDirectory, filename));
		}

		public static string ReadContent(string filename) {
			string content = "";
			using (StreamReader reader = new StreamReader(PathToContent(filename)))
				content = reader.ReadToEnd();
			return content;
		}
	}
}
