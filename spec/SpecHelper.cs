using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Requestoring;

namespace MooGet.Specs {

	[SetUpFixture]
	public class MooSpecsSetup {

		[SetUp]
		public void BeforeAll() {
			MooGetSpec.ClearTempDirectory();

			Environment.SetEnvironmentVariable("HOME", MooGetSpec.PathToTemp("home")); // fake 'home' directory
			Environment.SetEnvironmentVariable("TMP",  MooGetSpec.PathToTemp("tmp"));  // fake 'tmp' directory
			MooGetSpec.ResetTempDirectory();
		}

		[TearDown]
		public void AfterAll() {
			// ... nothing ...
		}
	}

	// Old - alias
	public class MooGetSpec : Spec {
	}

	/// <summary>Base class for MooGet specs.  Provides helper methods.</summary>
	public class Spec {

		[SetUp]
		public void BeforeEach() {
			ResetTempDirectory();
			ResetMooWorkingDirectory();
			Requestor.Global.FakeResponses.Clear();
			Requestor.Global.AllowRealRequests = false;
			Requestor.Global.Verbose           = false;
		}

		[TearDown]
		public void AfterEach() {
			// ... nothing yet ...
		}

		public bool OutputMooCommands = false;

		public string MooWorkingDirectory;

		public void ResetMooWorkingDirectory() {
			MooWorkingDirectory = PathToTemp("working");
		}

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
			Directory.CreateDirectory(MooGetSpec.PathToTemp("tmp"));
		}

		public static void ResetTempDirectory() {
			ClearTempDirectory();
			SetupTempDirectory();
		}

		public void cd(string relativePath) {
			MooWorkingDirectory = Path.GetFullPath(Path.Combine(MooWorkingDirectory, relativePath));
		}

		public string run(string commandAndArguments) {
			return Util.RunCommand(commandAndArguments, MooWorkingDirectory);
		}

		public string moo() {
			return moo(null);
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

			if (OutputMooCommands)
				Console.WriteLine("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

            process.StartInfo.UseShellExecute                 = false;
            process.StartInfo.RedirectStandardOutput          = true;
            process.StartInfo.CreateNoWindow                  = true;
            process.StartInfo.WorkingDirectory                = MooWorkingDirectory;
			process.StartInfo.EnvironmentVariables["HOME"]    = PathToTemp("home"); // fake the home directory
			process.StartInfo.EnvironmentVariables["TMP"]     = PathToTemp("tmp");  // fake the tmp directory
			process.StartInfo.EnvironmentVariables["MOO_DIR"] = null;               // ignore out system's MOO_DIR!
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

			if (OutputMooCommands)
				Console.WriteLine("{0}\n", stdout);

            return stdout.TrimEnd('\n');
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

		public static string PathToTempHome(params string[] parts) {
			return FullCombinedPath(Path.Combine(TempDirectory, "home"), parts);
		}

		public static string ReadTemp(string filename) {
			return MooGet.Util.ReadFile(PathToTemp(filename));
		}
		#endregion

		public string NuGetServiceRoot = "http://packages.nuget.org/v1/FeedService.svc/";

		/// <summary>Tells Requestor to fake the given response (in spec/content/nuget_odata_responses)</summary>
		public void FakeResponse(string getUrl, string savedResponse) {
			var filePath = PathToContent("nuget_odata_responses", savedResponse);
			if (! File.Exists(filePath))
				throw new Exception(string.Format("Couldn't find saved response: {0}", filePath));
			
			Requestor.Global.FakeResponse("GET", getUrl, Response.FromHttpResponse(File.ReadAllText(filePath)));
		}
	}
}
