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

		public SourcePackage Get(string id) {
			id = id.ToLower();
			return LatestPackages.FirstOrDefault(p => p.Id.ToLower() == id);
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
			var doc      = Util.GetXmlDocumentForString(xml);

			foreach (XmlElement entry in doc.GetElementsByTagName("entry"))
				packages.Add(PackageFromFeedEntry(entry));

			return packages;
		}

		static SourcePackage PackageFromFeedEntry(XmlElement entry) {
			var package = new SourcePackage();
			
			foreach (XmlNode node in entry.ChildNodes) {
				switch (node.Name.ToLower()) {
					
					// we don't use these elements at the moment, so we ignore them
					case "id":
					case "published":
					case "updated":
						break;

					case "pkg:packageid": package.Id            = node.InnerText; break;
					case "pkg:version":   package.VersionString = node.InnerText; break;
					case "pkg:language":  package.Language      = node.InnerText; break;
					case "title":         package.Title         = node.InnerText; break;
					case "content":       package.Description   = node.InnerText; break;
					case "author":        package.Authors.Add(node.InnerText);    break;

					case "category":
						var term = node.Attributes["term"].Value;
						if (! package.Tags.Contains(term))
							package.Tags.Add(term);
						break;

					case "pkg:requirelicenseacceptance":
						package.RequireLicenseAcceptance = bool.Parse(node.InnerText); break;

					case "pkg:keywords":
						// if there is 1 <string>, split it on spaces 
						// else if there are many, each element is a tag
						var tagNodes = node.ChildNodes;
						if (tagNodes.Count == 1) {
							foreach (var tag in tagNodes[0].InnerText.Split(' '))
								if (! package.Tags.Contains(tag.Trim()))
									package.Tags.Add(tag.Trim());
						} else {
							foreach (XmlNode tagString in tagNodes)
								if (! package.Tags.Contains(tagString.InnerText.Trim()))
									package.Tags.Add(tagString.InnerText.Trim());
						}
						break;

					case "link":
						switch (node.Attributes["rel"].Value) {
							case "enclosure":
								package.DownloadUrl = node.Attributes["href"].Value; break;
							case "license":
								package.LicenseUrl = node.Attributes["href"].Value; break;
							default:
								Console.WriteLine("Unsupported <link> rel: {0}", node.Attributes["rel"].Value); break;
						}
						break;

					case "pkg:dependencies":
						foreach (XmlNode dependencyNode in node.ChildNodes) {
							var dependency = new PackageDependency();
							foreach (XmlNode depNode in dependencyNode.ChildNodes) {
								switch (depNode.Name) {
									case "pkg:id":         dependency.Id               = depNode.InnerText; break;
									case "pkg:version":    dependency.VersionString    = depNode.InnerText; break;
									case "pkg:minVersion": dependency.MinVersionString = depNode.InnerText; break;
									case "pkg:maxVersion": dependency.MaxVersionString = depNode.InnerText; break;
									default:
										Console.WriteLine("Unknown dependency node: {0}", depNode.Name);
										break;
								}
							}
							package.Dependencies.Add(dependency);
						}
						break;

					default:
						Console.WriteLine("Unsupported <entry> element: {0} \"{1}\"", node.Name, node.InnerText);
						break;
				}
			}

			return package;
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
