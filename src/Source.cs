using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Linq;
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
			get { return LatestPackages; }
		}

		public List<SourcePackage> LatestPackages {
			get { return RemoveAllVersionsButLatest(AllPackages); }
		}

		public List<SourcePackage> AllPackages {
			get { return Source.GetPackagesFromPath(Path).OrderBy(p => p.Title).ToList(); }
		}

		public List<SourcePackage> SearchByTitle(string query) {
			query = query.ToLower();
			return Packages.Where(p => p.Title.ToLower().Contains(query)).ToList();
		}

		public List<SourcePackage> SearchByDescription(string query) {
			query = query.ToLower();
			return Packages.Where(p => p.Description.ToLower().Contains(query)).ToList();
		}

		public List<SourcePackage> SearchByTitleOrDescription(string query) {
			query = query.ToLower();
			return Packages.Where(p => p.Title.ToLower().Contains(query) || p.Description.ToLower().Contains(query)).ToList();
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

		// If we use a normal XmlResolver, it will explode if it parses something that it thinks might be a URI but the URI is invalid
		class NonStupidXmlResolver : XmlResolver {
			public override Uri ResolveUri (Uri baseUri, string relativeUri){ return baseUri; }
			public override object GetEntity (Uri absoluteUri, string role, Type type){ return null; }
			public override ICredentials Credentials { set {} }
		}

		static SourcePackage PackageFromFeedEntry(XmlElement entry) {
			return new SourcePackage {
				Id            = entry.GetElementsByTagName("pkg:packageId")[0].InnerText,
				Title         = entry.GetElementsByTagName("title")[0].InnerText,
				Description   = entry.GetElementsByTagName("content")[0].InnerText,
				VersionString = entry.GetElementsByTagName("pkg:version")[0].InnerText  // TODO fix this ... dependencies can have versions too ...
			};
		}

		static List<SourcePackage> RemoveAllVersionsButLatest(List<SourcePackage> packages) {
			var latestPackages  = new List<SourcePackage>();
			var highestVersions = new Dictionary<string, PackageVersion>(); // packageId: highest version

			// grab the highest version of each package
			foreach (var package in packages)
				if (! highestVersions.ContainsKey(package.Id) || highestVersions[package.Id] < package.Version)
					highestVersions[package.Id] = package.Version;

			// now that we have the highest version for each package, put just those packages into our new list
			foreach (var package in packages)
				if (highestVersions[package.Id] == package.Version)
					latestPackages.Add(package);

			return latestPackages;
		}
		#endregion
	}
}
