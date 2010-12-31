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

		public static bool IsWindows {
			get { return Environment.OSVersion.Platform.ToString().Contains("Win"); }
		}

		public static string ENV(string environmentVariable) {
			return Environment.GetEnvironmentVariable(environmentVariable);
		}

		public static string HomeDirectory {
			get {
				if (ENV("HOME") != null)
					return ENV("HOME");
				else if (ENV("HOMEDRIVE") != null && ENV("HOMEPATH") != null)
					return string.Format("{0}{1}", ENV("HOMEDRIVE"), ENV("HOMEPATH"));
				else if (ENV("USERPROFILE") != null)
					return ENV("USERPROFILE");
				else
					throw new Exception("Could not determine where your home directory is");
			}
		}
	}
}
