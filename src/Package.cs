using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;

// TODO this is going to get big.  be sure to refactor into a partial class or 
//      additional classes as soon as we hit 200 LOC or so ...
namespace MooGet {

	/// <summary>Represents a NuGet package</summary>
	public class Package {

		public string Name { get; set; }

		public static List<Package> ParseFeed(string xml) {
			var packages = new List<Package>();
			var doc      = GetXmlDocumentForString(xml);

			foreach (XmlElement entry in doc.GetElementsByTagName("entry"))
				packages.Add(Package.FromFeedEntry(entry));

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

		static Package FromFeedEntry(XmlElement entry) {
			return new Package {
				Name = entry.GetElementsByTagName("title")[0].InnerText
			};
		}
	}
}
