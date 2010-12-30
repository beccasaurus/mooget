using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a standard XML with Package information</summary>
	public class Source {

		/// <summary>URL or local filesystem path to this source's XML feed</summary>
		public string Path { get; set; }

		public Source(){}
		public Source(string path) {
			Path = path;
		}

		public List<SourcePackage> Packages {
			get { return Source.GetPackagesFromPath(Path); }
		}

		#region Private
		static List<SourcePackage> GetPackagesFromPath(string path) {
			if (File.Exists(path))
				return GetPackagesFromXml(MooGet.Util.ReadFile(path));
			else
				throw new NotImplementedException("Haven't implemented getting packages from anything but a local file yet");
		}

		static List<SourcePackage> GetPackagesFromXml(string xml) {
			var packages = new List<SourcePackage>();
			var doc      = GetXmlDocumentForString(xml);

			foreach (XmlElement entry in doc.GetElementsByTagName("entry"))
				packages.Add(PackageFromFeedEntry(entry));

			return packages;
		}

		static XmlDocument GetXmlDocumentForString(string xml) {
			var doc            = new XmlDocument();
			var reader         = new XmlTextReader(new StringReader(xml));
			reader.XmlResolver = new NonStupidXmlResolver();
			doc.Load(reader);
			return doc;
		}

		class NonStupidXmlResolver : XmlResolver {
			public override Uri ResolveUri (Uri baseUri, string relativeUri){ return baseUri; }
			public override object GetEntity (Uri absoluteUri, string role, Type type){ return null; }
			public override ICredentials Credentials { set {} }
		}

		static SourcePackage PackageFromFeedEntry(XmlElement entry) {
			return new SourcePackage {
				Name = entry.GetElementsByTagName("title")[0].InnerText
			};
		}
		#endregion
	}
}
