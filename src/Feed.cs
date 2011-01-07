using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Reflection;
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

		// TODO move this out into its own file ...
		public class EasyXmlWriter {
			public MemoryStream Stream { get; set; }
			public XmlWriter    Writer { get; set; }

			public EasyXmlWriter() {
				Stream = new MemoryStream();
				Writer = XmlWriter.Create(Stream, new XmlWriterSettings { Indent = true });
				Writer.WriteStartDocument();
			}

			public EasyXmlWriter WriteElement(string name) {
				return WriteElement(name, null, null);
			}
			public EasyXmlWriter WriteElement(string name, string innerText) {
				return WriteElement(name, innerText, null);
			}
			public EasyXmlWriter WriteElement(string name, object attributes) {
				return WriteElement(name, null, attributes);
			}
			public EasyXmlWriter WriteElement(string name, string innerText, object attributes) {
				StartElement(name, innerText, attributes);
				EndElement();
				return this;
			}

			public EasyXmlWriter StartElement(string name) {
				return StartElement(name, null, null);
			}
			public EasyXmlWriter StartElement(string name, string innerText) {
				return StartElement(name, innerText, null);
			}
			public EasyXmlWriter StartElement(string name, object attributes) {
				return StartElement(name, null, attributes);
			}
			public EasyXmlWriter StartElement(string name, string innerText, object attributes) {
				var attrs = ObjectToAttributes(attributes);

				if (attrs != null && attrs.ContainsKey("xmlns"))
					Writer.WriteStartElement(name, attrs["xmlns"]);
				else
					Writer.WriteStartElement(name);

				if (attrs != null) {
					foreach (var attr in attrs) {
						Writer.WriteStartAttribute(attr.Key.Replace("_", ":")); // foo_bar becomes foo:bar
						Writer.WriteString(attr.Value);
						Writer.WriteEndAttribute();
					}
				}

				if (innerText != null)
					Writer.WriteString(innerText);

				return this;
			}

			public EasyXmlWriter EndElement() {
				Writer.WriteEndElement();
				return this;
			}

			string _xml;
			public string ToString() {
				if (_xml == null) {
					Writer.WriteEndDocument();
					Writer.Flush();
					var buffer = Stream.ToArray();
					_xml = System.Text.Encoding.UTF8.GetString(buffer).Trim();
				}
				return _xml;
			}

			static IDictionary<string, string> ObjectToAttributes(object anonymousType) {
				if (anonymousType == null)
					return null;

				var attr = BindingFlags.Public | BindingFlags.Instance;
				var dict = new Dictionary<string, string>();
				foreach (var property in anonymousType.GetType().GetProperties(attr))
					if (property.CanRead)
						dict.Add(property.Name, property.GetValue(anonymousType, null).ToString());
				return dict;
			} 
		}

		public static string GenerateFeed(List<Package> packages) {
			var xml = new EasyXmlWriter();

			// TODO make it so you can specify these Atom feed metadata values
			var atomId          = "urn:uuid:" + Guid.NewGuid().ToString();
			var atomTitle       = "Generated MooGet Feed";
			var atomSubtitle    = "Enjoy!";
			var atomAuthorName  = "Moo Cow";
			var atomAuthorEmail = "moo.cow@mooget.net";
			var atomUpdatedAt   = DateTime.Now; // this should be calculated somehow ...

			// Atom <feed>
			xml.StartElement("feed", new { xml_lang = "en-us", xmlns = "http://www.w3.org/2005/Atom", xmlns_pkg = "http://schemas.microsoft.com/packaging/2010/07/" }).
				WriteElement("id",       atomId).
				WriteElement("title",    atomTitle).
				WriteElement("subtitle", atomSubtitle).
				WriteElement("updated",  DateTime.Now.ToString("s") + "Z").
				StartElement("author").
					WriteElement("name",  atomAuthorName).
					WriteElement("email", atomAuthorEmail).
				EndElement();

				foreach (var package in packages) {
					xml.StartElement("entry").
						WriteElement("pkg:packageId", package.Id).
						WriteElement("pkg:version",   package.VersionString).
						WriteElement("title",         (package.Title == null) ? package.Id : package.Title).
						WriteElement("content",       package.Description);

						// <pkg:keywords xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><string xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays">SNAP
						if (package.Tags.Count > 0) {
							xml.StartElement("pkg:keywords", new { xmlns_i = "http://www.w3.org/2001/XMLSchema-instance" });
							foreach (var tag in package.Tags) {
								xml.WriteElement("string", tag, new { xmlns = "http://schemas.microsoft.com/2003/10/Serialization/Arrays" });
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
