using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MooGet {

	public class Feed {

		public static string Domain = "mooget.net"; // default domain for use in generating parts of the Atom feed

		public static List<RemotePackage> ParseFeed(string feedSourceXml) {
			var packages = new List<RemotePackage>();
			var doc      = Util.GetXmlDocumentForString(feedSourceXml);

			foreach (XmlElement entry in doc.GetElementsByTagName("entry"))
				packages.Add(PackageFromFeedEntry(entry));

			return packages;
		}

		// Format date for Atom feed ... not sure if this is right or not.  It doesn't end in 'Z' as expected ...
		public static string Format(DateTime date) {
			// return date.ToString("s");
			return XmlConvert.ToString(date, XmlDateTimeSerializationMode.Local);
		}

		public static string IdForPackage(Package package) {
			return IdForPackage(package, Feed.Domain);
		}

		public static string IdForPackage(Package package, string domain) {
			return string.Format("tag:{0},{1}:{2}.nupkg", domain, package.Modified.ToString("yyyy-MM-dd"), package.IdAndVersion);
		}

		public static string GenerateFeed(List<Package> packages) {
			var xml = new XmlBuilder();

			// TODO make it so you can specify these Atom feed metadata values
			var atomId          = string.Format("tag:{0},{1}:/", Domain, "2010-12-15"); // date MooGet was created
			var atomTitle       = "Generated MooGet Feed";
			var atomSubtitle    = "Enjoy!";
			var atomAuthorName  = "Moo Cow";
			var atomAuthorEmail = "moo.cow@mooget.net";
			var atomUpdatedAt   = DateTime.Now; // this should be calculated somehow ...

			// Atom <feed>
			xml.StartElement("feed", new { xml_lang = "en-us", xmlns = "http://www.w3.org/2005/Atom", 
							               xmlns_pkg = "http://schemas.microsoft.com/packaging/2010/07/" }).

				// Standard Atom feed metadata
				WriteElement("id",       atomId).
				WriteElement("title",    atomTitle).
				WriteElement("subtitle", atomSubtitle).
				WriteElement("updated",  Format(DateTime.Now)).
				StartElement("author").
					WriteElement("name",  atomAuthorName).
					WriteElement("email", atomAuthorEmail).
				EndElement();

				// <entry> elements for each package
				foreach (var package in packages) {
					xml.StartElement("entry").
						WriteElement("id",            IdForPackage(package)).
						WriteElement("title",         (package.Title == null) ? package.Id : package.Title).
						WriteElement("updated",       Format(package.Modified)).
						WriteElement("published",     Format(package.Created)).
						WriteElement("pkg:packageId", package.Id).
						WriteElement("pkg:version",   package.VersionString).
						WriteElement("content",       package.Description);

						foreach (var author in package.Authors)
							xml.StartElement("author").
								WriteElement("name", author).
							EndElement();

						if (package.Tags.Any()) {
							xml.StartElement("pkg:keywords", new { xmlns_i = "http://www.w3.org/2001/XMLSchema-instance" });
							foreach (var tag in package.Tags) {
								xml.WriteElement("string", tag, new { xmlns = "http://schemas.microsoft.com/2003/10/Serialization/Arrays" });
							}
							xml.EndElement();
						}

						if (package.Dependencies.Any()) {
							xml.StartElement("pkg:dependencies", new { xmlns_i = "http://www.w3.org/2001/XMLSchema-instance" });
							foreach (var dependency in package.Dependencies) {
								xml.StartElement("pkg:dependency");
									xml.WriteElement("pkg:id", dependency.Id);
									if (dependency.Version != null)
										xml.WriteElement("pkg:version", dependency.VersionString);
									else if (dependency.MinVersion != null)
										xml.WriteElement("pkg:minVersion", dependency.MinVersionString);
									else if (dependency.MaxVersion != null)
										xml.WriteElement("pkg:maxVersion", dependency.MaxVersionString);
								xml.EndElement();
							}
							xml.EndElement();
						}

					xml.EndElement(); // </entry>
				}

			xml.EndElement(); // </feed>

			return xml.ToString();
		}

		static RemotePackage PackageFromFeedEntry(XmlElement entry) {
			var package = new RemotePackage();
			
			foreach (XmlNode node in entry.ChildNodes) {
				switch (node.Name.ToLower()) {
					
					case "id": break;

					case "pkg:packageid": package.Id            = node.InnerText; break;
					case "pkg:version":   package.VersionString = node.InnerText; break;
					case "pkg:language":  package.Language      = node.InnerText; break;
					case "title":         package.Title         = node.InnerText; break;
					case "content":       package.Description   = node.InnerText; break;
					case "author":        package.Authors.Add(node.InnerText);    break;

					case "published":     package.Created  = DateTime.Parse(node.InnerText); break;
					case "updated":       package.Modified = DateTime.Parse(node.InnerText); break;

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
	}
}
