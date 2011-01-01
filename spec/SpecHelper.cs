using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace MooGet.Specs {

	[SetUpFixture]
	public class MooSpecsSetup {

		[SetUp]
		public void RunBeforeAllSpecs() {
			MooGetSpec.ClearTempDirectory();
			MooGetSpec.SetupTempDirectory();
		}

		[TearDown]
		public void RunAfterAllSpecs() {
			MooGetSpec.ClearTempDirectory();
		}
	}

	/// <summary>Base class for MooGet specs.  Provides helper methods.</summary>
	public class MooGetSpec {

		public static string ToJSON(object o) {
			return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(o);
		}

		public static void ClearTempDirectory() {
			if (Directory.Exists(MooGetSpec.TempDirectory))
				Directory.Delete(MooGetSpec.TempDirectory, true);
		}

		public static void SetupTempDirectory() {
			Directory.CreateDirectory(MooGetSpec.TempDirectory);
			Directory.CreateDirectory(MooGetSpec.PathToTemp("home"));
			Directory.CreateDirectory(MooGetSpec.PathToTemp("working"));
		}

		public string moo(string arguments, params object[] formatting) {
			return moo(string.Format(arguments, formatting));
		}

		// runs the moo command from ./spec/tmp/working with the home directory faked as ./spec/tmp/home
		public string moo(string arguments) {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = FullCombinedPath(SpecDirectory, "..", "bin", "Debug", "moo.exe");
            if (arguments != null)
                process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute              = false;
            process.StartInfo.RedirectStandardOutput       = true;
            process.StartInfo.CreateNoWindow               = true;
            process.StartInfo.WorkingDirectory             = PathToTemp("working");
			process.StartInfo.EnvironmentVariables["HOME"] = PathToTemp("home"); // fake the home directory
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return stdout.Trim();
		}

		#region Path Helpers

		public static string FullCombinedPath(string part1, params string[] parts) {
			var list = new List<string>(parts);
			list.Insert(0, part1);
			return FullCombinedPath(list.ToArray());
		}
		public static string FullCombinedPath(params string[] parts) {
			var list = new List<string>(parts);
			var path = list.First();
			list.RemoveAt(0);
			foreach (var part in list)
				path = Path.Combine(path, part);
			return Path.GetFullPath(path);
		}

		// spec dir
		public static string SpecDirectory {
			get { return FullCombinedPath(Directory.GetCurrentDirectory(), "../../spec"); }
		}

		// content
		public static string ContentDirectory {
			get{ return FullCombinedPath(SpecDirectory, "content"); }
		}

		public static string PathToContent(params string[] parts) {
			return FullCombinedPath(ContentDirectory, parts);
		}

		public static string ReadContent(string filename) {
			return MooGet.Util.ReadFile(PathToContent(filename));
		}

		// temp
		public static string TempDirectory {
			get{ return FullCombinedPath(SpecDirectory, "tmp"); }
		}

		public static string PathToTemp(params string[] parts) {
			return FullCombinedPath(TempDirectory, parts);
		}

		public static string ReadTemp(string filename) {
			return MooGet.Util.ReadFile(PathToTemp(filename));
		}
		#endregion
	}
}
