using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MooGet {

	// TODO this is really finally starting to get long!  we should split this up into mutliple files using a partial class.
	//      later, if this gets even bigger, we could separate out into different classes;

	/// <summary>Back-o-utility methods for MooGet</summary>
	public static class Util {

		// helper methods for getting spaces ... useful for commands ...
		public static string Spaces(string str, int numSpaces) {
			string spaces = "";
			for (int i = 0; i < numSpaces - str.Length; i++)
				spaces += " ";
			return spaces;
		}

		public static string HelpForCommand(string command) {
			return ReadStringResource("help." + command);
		}

		public static string ReadStringResource(string resourceName) {
			// foreach (string name in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
			string text = null;
			using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("MooGet.resources." + resourceName)))
				text = reader.ReadToEnd();
			return text;
		}

		public static string RunCommand(string commandAndArguments, string workingDirectory) {
			var cmdAndArgs = new List<string>(commandAndArguments.Split(' '));
			var command    = cmdAndArgs.First();
			cmdAndArgs.RemoveAt(0);
			var arguments  = string.Join(" ", cmdAndArgs.ToArray()).Trim();

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command;
            if (arguments.Length > 0)
                process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute              = false;
            process.StartInfo.RedirectStandardOutput       = true;
            process.StartInfo.CreateNoWindow               = true;
            process.StartInfo.WorkingDirectory             = workingDirectory;
			//process.StartInfo.EnvironmentVariables["HOME"] = PathToTemp("home"); // fake the home directory
			//process.StartInfo.EnvironmentVariables["TMP"]  = PathToTemp("tmp");  // fake the tmp directory
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return stdout.Trim();
		}

		public static string ReadFile(string filename) {
			if (filename == null)        return null;
			if (! File.Exists(filename)) return null;
			return File.ReadAllText(filename);
		}

		public static void WriteFile(string filename, string content) {
			using (var writer = new StreamWriter(filename, false))
				writer.Write(content);
		}

		public static void AppendToFile(string filename, string content) {
			using (var writer = new StreamWriter(filename, true))
				writer.Write(content);
		}

		public static WebClient WebClientWithUserAgent {
			get {
				var client = new WebClient();
				client.Headers.Add("user-agent", Moo.UserAgent);
				return client;
			}
		}

		public static string ReadUrl(string url) {
			return WebClientWithUserAgent.DownloadString(url);
		}

		public static void DownloadFile(string url, string path) {
			WebClientWithUserAgent.DownloadFile(url, path);	
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

		// OBSOLETE ... Use Zip class
		// TODO move this into Nuspec?  This isn't very ... Util-esque
		public static string ReadNuspecInNupkg(string pathToNupkg) {
			// TODO handle .nuspec files in subdirectories, eg. content ... just get .nuspec in root
			string nuspec = null;

			using (var zip = System.IO.Packaging.Package.Open(pathToNupkg, FileMode.Open, FileAccess.Read))
				foreach (var part in zip.GetParts())
					if (part.Uri.OriginalString.EndsWith(".nuspec"))
						using (var reader = new StreamReader(part.GetStream()))
							nuspec = reader.ReadToEnd();

			return nuspec;
		}

		// OBSOLETE ... Use Zip class
		public static string[] PathsInZip(string zipFile) {
			var parts = new List<string>();
			using (var zip = System.IO.Packaging.Package.Open(zipFile, FileMode.Open, FileAccess.Read))
				foreach (var part in zip.GetParts())
					parts.Add(part.Uri.OriginalString.Substring(1)); // removes the beginning slash from the uri
			return parts.ToArray();
		}

		// unzips a .zip file ... it knows it's dealing with nuget packages because we ignore certain files
		public static void Unzip(string zipFile, string directoryToUnzipInto) {
			Directory.CreateDirectory(directoryToUnzipInto);
			using (var zip = System.IO.Packaging.Package.Open(zipFile, FileMode.Open, FileAccess.Read)) {
				foreach (var part in zip.GetParts()) {
					var path = part.Uri.OriginalString.Substring(1);

					// skip these stupid files
					if (path == "[Content_Types].xml" || path == "_rels/.rels" || path.EndsWith(".psmdcp"))
						continue;

					using (var stream = part.GetStream(FileMode.Open, FileAccess.Read)) {

						// Create a path and make sure the directory for it gets created
						var filepath = Path.Combine(directoryToUnzipInto, path);
						Directory.CreateDirectory(Path.GetDirectoryName(filepath));

						// Copy the part Stream into a file Stream
						using (var file = File.OpenWrite(filepath)) {
							var buffer = new byte[8 * 1024];
							int len;
							while ( (len = stream.Read(buffer, 0, buffer.Length)) > 0)
								file.Write(buffer, 0, len);
						}
					}
				}
			}
		}

		public static string TempDirectory {
			get { return Environment.GetEnvironmentVariable("TMP") ?? Path.GetTempPath(); }
		}

		public static XmlDocument GetXmlDocumentForFile(string filename) {
			return GetXmlDocumentForString(ReadFile(filename));
		}

		public static XmlDocument GetXmlDocumentForString(string xml) {
			if (xml == null) return null;
			var doc            = new XmlDocument();
			var reader         = new XmlTextReader(new StringReader(xml));
			reader.XmlResolver = new NonStupidXmlResolver();
			doc.Load(reader);
			return doc;
		}

		// If we use a normal XmlResolver, it will explode if it parses something that it thinks might be a URI but the URI is invalid
		class NonStupidXmlResolver : XmlResolver {
			public override Uri ResolveUri (Uri baseUri, string relativeUri){ return baseUri; }
			public override object GetEntity (Uri absoluteUri, string role, Type type){ return null; }
			public override ICredentials Credentials { set {} }
		}

		public static string MimeTypeFor(string extension) {
			extension = (extension.Contains(".")) ? Path.GetExtension(extension).Replace(".","") : extension;

			switch (extension.ToLower().Trim()) {
				case "txt":
				case "cs":
				case "boo":
				case "vb":
					return MediaTypeNames.Text.Plain;
				
				case "xml":
				case "config":
				case "nuspec":
					return MediaTypeNames.Text.Xml;

				case "htm":
				case "html":
					return MediaTypeNames.Text.Html;

				case "rtf":
					return MediaTypeNames.Text.RichText;

				case "jpg":
				case "jpeg":
					return MediaTypeNames.Image.Jpeg;

				case "gif":
					return MediaTypeNames.Image.Gif;

				case "png":
					return "image/png";

				default:
					return MediaTypeNames.Application.Octet;
			}
		}
	}
}
