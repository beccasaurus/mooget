using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

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

		public static void Unzip(string zipFile, string directoryToUnzipInto) {
			Directory.CreateDirectory(directoryToUnzipInto);
			using (var zip = System.IO.Packaging.Package.Open(zipFile, FileMode.Open, FileAccess.Read)) {
				foreach (var part in zip.GetParts()) {
					using (var reader = new StreamReader(part.GetStream(FileMode.Open, FileAccess.Read))) {
						var filepath = Path.Combine(directoryToUnzipInto, part.Uri.OriginalString.Substring(1));
						Directory.CreateDirectory(Path.GetDirectoryName(filepath));
						using (var writer = new FileStream(filepath, FileMode.Create, FileAccess.Write)) {
							var buffer = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
							writer.Write(buffer, 0, buffer.Length);
						}
					}
				}
			}
		}

		public static XmlDocument GetXmlDocumentForFile(string filename) {
			return GetXmlDocumentForString(ReadFile(filename));
		}

		public static XmlDocument GetXmlDocumentForString(string xml) {
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
	}
}
