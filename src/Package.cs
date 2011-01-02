using System;
using System.Xml;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a NuGet package</summary>
	/// <remarks>
	/// A MooGet.Package will typically be:
	///
	///  1. SourcePackage: has package information from a given Source.
	///  2. InstalledPackage: has information about a package actually installed on the local file system.
	///
	/// Package defines certain properties / functionality that 
	/// packages have, regardless of whether or not you've installed them yet.
	/// </remarks>
	public class Package {

		List<string>            _authors      = new List<string>();
		List<string>            _tags         = new List<string>();
		List<PackageDependency> _dependencies = new List<PackageDependency>();

		public string Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string Language { get; set; }

		public string LicenseUrl { get; set; }

		public PackageVersion Version { get; set; }

		public bool RequireLicenseAcceptance { get; set; }

		public List<string> Authors { get { return _authors; } }

		public List<string> Tags { get { return _tags; } }

		public List<PackageDependency> Dependencies { get { return _dependencies; } }

		public string IdAndVersion { get { return string.Format("{0}-{1}", Id, Version); } }

		public string VersionString {
			get {
				if (Version == null) return null;
				return Version.ToString(); 
			}
			set { Version = new PackageVersion(value); }
		}

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

		public override string ToString() {
			return string.Format("{0} ({1})", Id, Version);
		}

		public static Package FromSpec(string pathToNuspec) {
			var package = new Package();
			package.LoadSpec(pathToNuspec);
			return package;
		}

		public void LoadSpec(string pathToNuspec) {
			var doc = Util.GetXmlDocumentForFile(pathToNuspec);
			foreach (XmlNode metadata in doc.GetElementsByTagName("metadata")[0]) {
				var text = metadata.InnerText.Trim();
				switch (metadata.Name.ToLower()) {
					case "id":          Id            = text; break;
					case "version":     VersionString = text; break;
					case "description": Description   = text; break;
					case "language":    Language      = text; break;
					case "licenseurl":  LicenseUrl    = text; break;

					case "created":  Created  = DateTime.Parse(text); break;
					case "modified": Modified = DateTime.Parse(text); break;

					case "requirelicenseacceptance":
						RequireLicenseAcceptance = bool.Parse(text); break;

					case "authors":
						foreach (XmlNode authorNode in metadata.ChildNodes)
							Authors.Add(authorNode.InnerText);
						break;

					default:
						Console.WriteLine("Unknown <metadata> element: {0}", metadata.Name);
						break;
				}
			}
			foreach (XmlNode dependencyNode in doc.GetElementsByTagName("dependency")) {
				var dependency = new PackageDependency();
				foreach (XmlAttribute attr in dependencyNode.Attributes) {
					switch (attr.Name.ToLower()) {
						case "id":         dependency.Id               = attr.Value; break;
						case "version":    dependency.VersionString    = attr.Value; break;
						case "minversion": dependency.MinVersionString = attr.Value; break;
						case "maxversion": dependency.MaxVersionString = attr.Value; break;
						default:
							Console.WriteLine("Unknown <dependency> attribute: {0}", attr.Name);
							break;
					}
				}
				Dependencies.Add(dependency);
			}
		}
	}
}
