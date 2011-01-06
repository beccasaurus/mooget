using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	public class Feed {

		// TODO make RemotePackage!
		public static List<RemotePackage> ParseFeed(string feedSourceXml) {
			var packages = new List<RemotePackage>();
			var doc      = Util.GetXmlDocumentForString(feedSourceXml);

			foreach (XmlElement entry in doc.GetElementsByTagName("entry"))
				packages.Add(PackageFromFeedEntry(entry));

			return packages;
		}

		public static string GenerateFeed(List<Package> packages) {
			var doc = new XmlDocument();

			// TODO make it so you can specify these Atom feed metadata values
			var atomId          = "urn:uuid:" + Guid.NewGuid().ToString();
			var atomTitle       = "Generated MooGet Feed";
			var atomSubtitle    = "Enjoy!";
			var atomAuthorName  = "Moo Cow";
			var atomAuthorEmail = "moo.cow@mooget.net";
			var atomUpdatedAt   = DateTime.Now; // this should be calculated somehow ...

			// <?xml version="1.0" encoding="utf-8"?>
			doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

			// Atom <feed>
			var feed = doc.CreateElement("feed");
			feed.SetAttribute("xml:lang",  "en-us");
			feed.SetAttribute("xmlns",     "http://www.w3.org/2005/Atom");
			feed.SetAttribute("xmlns:pkg", "http://schemas.microsoft.com/packaging/2010/07/");

			// <feed> metadata ... id, title, subtitle, links, id, updated, author ...
			var id = doc.CreateElement("id");
			id.InnerText = atomId;
			feed.AppendChild(id);

			var title = doc.CreateElement("title");
			title.InnerText = atomTitle;
			feed.AppendChild(title);

			var subtitle = doc.CreateElement("subtitle");
			subtitle.InnerText = atomSubtitle;
			feed.AppendChild(subtitle);

			var updated = doc.CreateElement("updated");
			updated.InnerText = DateTime.Now.ToString("s") + "Z";
			feed.AppendChild(updated);

			var author = doc.CreateElement("author");
				var authorName = doc.CreateElement("name");
				authorName.InnerText = atomAuthorName;
				author.AppendChild(authorName);

				var authorEmail = doc.CreateElement("email");
				authorEmail.InnerText = atomAuthorEmail;
				author.AppendChild(authorEmail);
			feed.AppendChild(author);

			// Add <entry> elements ...
			foreach (var package in packages) {
				var entry = doc.CreateElement("entry");

				// TODO ONCE GREEN, refactor each of these down to 1 line

				var packageId = doc.CreateElement("pkg", "packageId", null);
				packageId.Prefix = "PREFIX";
				packageId.InnerText = package.Id;
				entry.AppendChild(packageId);

				var packageVersion = doc.CreateElement("pkg", "version", null);
				packageVersion.InnerText = package.VersionString;
				entry.AppendChild(packageVersion);

				var packageTitle = doc.CreateElement("title");
				packageTitle.InnerText = (package.Title == null) ? package.Id : package.Title;
				entry.AppendChild(packageTitle);

				var packageDescription = doc.CreateElement("foo:content");
				packageDescription.InnerText = "djsklfjdsjkfsdl " + package.Description;
				entry.AppendChild(packageDescription);

				feed.AppendChild(entry);
			}

			doc.AppendChild(feed);

			// or doc.OuterXml (not indented)
			var xml = new StringWriter();

			using (var writer = new XmlTextWriter(xml)) {
				writer.Formatting = Formatting.Indented;
				writer.Namespaces = false;
				doc.WriteTo(writer);
			}

			Console.WriteLine("Generated Feed\n{0}", xml.ToString());
			return xml.ToString();
		}

		static RemotePackage PackageFromFeedEntry(XmlElement entry) {
			var package = new RemotePackage();
			
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
	}
}
