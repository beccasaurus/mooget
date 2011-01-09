using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a NuGet package</summary>
	/// <remarks>
	/// A MooGet.Package will typically be:
	///
	///  1. RemotePackage: has package information from a given Source.
	///  2. LocalPackage (or InstalledPackage ?): has information about a package actually installed on the local file system.
	///
	/// Package defines certain properties / functionality that 
	/// packages have, regardless of whether or not you've installed them yet.
	/// </remarks>
	public class Package {

		List<string>            _authors      = new List<string>();
		List<string>            _owners       = new List<string>();
		List<string>            _tags         = new List<string>();
		List<PackageDependency> _dependencies = new List<PackageDependency>();

		public string Id                     { get; set; }
		public string Title                  { get; set; }
		public string Description            { get; set; }
		public string Language               { get; set; }
		public string LicenseUrl             { get; set; }
		public string ProjectUrl             { get; set; }
		public string IconUrl                { get; set; }
		public DateTime Created              { get; set; }
		public DateTime Modified             { get; set; }
		public PackageVersion Version        { get; set; }
		public bool RequireLicenseAcceptance { get; set; }

		public List<string> Authors {
			get { return _authors;  }
			set { _authors = value; }
		}

		public List<string> Owners {
			get { return _owners;  }
			set { _owners = value; }
		}

		public List<string> Tags {
			get { return _tags;  }
			set { _tags = value; }
		}

		public List<PackageDependency> Dependencies {
			get { return _dependencies;  }
			set { _dependencies = value; }
		}

		public string IdAndVersion { get { return string.Format("{0}-{1}", Id, Version); } }

		public string VersionString {
			get {
				if (Version == null) return null;
				return Version.ToString(); 
			}
			set { Version = new PackageVersion(value); }
		}

		public override string ToString() {
			return string.Format("{0} ({1})", Id, Version);
		}

		public List<RemotePackage> FindDependencies(params List<RemotePackage>[] packages) {
			return Package.FindDependencies(this, packages);
		}

		public static List<RemotePackage> FindDependencies(Package package, params List<RemotePackage>[] listsOfPackages) {
			var found = new List<RemotePackage>();
			Console.WriteLine("FindDependencies for {0}", package);

			foreach (var dependency in package.Dependencies) {
				Console.WriteLine("Looking for dependency: {0}", dependency);
				RemotePackage dependencyPackage = null;
				foreach (var packages in listsOfPackages) {
					var match = packages.Where(pkg => pkg.Id == dependency.Id && dependency.Matches(pkg.Version)).
						OrderBy(pkg => pkg.Version).Reverse().FirstOrDefault();

					Console.WriteLine("match: {0}", match);

					if (dependencyPackage == null || dependencyPackage.Version < match.Version)
						dependencyPackage = match;
				}
				if (dependencyPackage != null) {
					found.Add(dependencyPackage);
					if (dependencyPackage.Dependencies.Any())
						found.AddRange(dependencyPackage.FindDependencies(listsOfPackages));
				}
				else
					Console.WriteLine("Count not find dependency: {0}", dependency.Id);
			}

			return found.Distinct().ToList();
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
					case "title":       Title         = text; break;
					case "description": Description   = text; break;
					case "language":    Language      = text; break;
					case "licenseurl":  LicenseUrl    = text; break;
					case "projecturl":  ProjectUrl    = text; break;
					case "iconurl":     IconUrl       = text; break;

					case "created":  Created  = DateTime.Parse(text); break;
					case "modified": Modified = DateTime.Parse(text); break;

					case "requirelicenseacceptance":
						RequireLicenseAcceptance = bool.Parse(text); break;

					case "authors":
						if (text.Contains(","))
							foreach (var author in text.Split(','))
								Authors.Add(author.Trim());
						else
							foreach (XmlNode authorNode in metadata.ChildNodes)
								Authors.Add(authorNode.InnerText);
						break;

					case "owners":
						if (text.Contains(","))
							foreach (var owner in text.Split(','))
								Owners.Add(owner.Trim());
						else
							foreach (XmlNode ownerNode in metadata.ChildNodes)
								Owners.Add(ownerNode.InnerText);
						break;

					case "tags":
						char separator = metadata.InnerText.Contains(",") ? ',' : ' ';
						foreach (var tag in metadata.InnerText.Trim().Split(separator))
							Tags.Add(tag.Trim());
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
