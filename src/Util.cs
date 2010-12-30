using System;
using System.IO;

namespace MooGet {

	/// <summary>Back-o-utility methods for MooGet</summary>
	public static class Util {

		public static string ReadFile(string filename) {
			string content = "";
			using (StreamReader reader = new StreamReader(filename))
				content = reader.ReadToEnd();
			return content;
		}
	}
}
