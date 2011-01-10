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

		public List<PackageDependency> FindPackageDependencies(params List<RemotePackage>[] packages) {
			return Package.FindPackageDependencies(this, packages);
		}

		public static List<PackageDependency> FindPackageDependencies(Package package, params List<RemotePackage>[] listsOfPackages) {
			return FindPackageDependencies(new Package[]{ package }, listsOfPackages);
		}

		/// <summary>
		/// Returns all of the PackageDependency dependencies that these packages require, given these RemotePackages (from sources)
		/// </summary>
		public static List<PackageDependency> FindPackageDependencies(Package[] packages, params List<RemotePackage>[] listsOfPackages) {
			throw new NotImplementedException("Not implemented yet");
			// TODO this needs to create a dependency tree and figure out the best dependencies?
		}

		public List<RemotePackage> FindDependencies(params List<RemotePackage>[] packages) {
			return Package.FindDependencies(this, packages);
		}

		public static List<RemotePackage> FindDependencies(Package package, params List<RemotePackage>[] listsOfPackages) {
			return FindDependencies(new Package[]{ package }, listsOfPackages);
		}

		/// <summary>
		/// Returns all of the RemotePackage that can be installed to satisfy dependencies for these packages, given these RemotePackages (from sources)
		/// </summary>
		public static List<RemotePackage> FindDependencies(Package[] packages, params List<RemotePackage>[] listsOfPackages) {
			return Package.FindDependencies(null, packages, listsOfPackages);
		}

		static List<RemotePackage> FindDependencies(IDictionary<string, List<PackageDependency>> discoveredDependencies, Package[] packages, params List<RemotePackage>[] listsOfPackages) {
			var found           = new List<RemotePackage>();
			var packageIds      = packages.Select(p => p.Id);
			bool throwIfMissing = (discoveredDependencies == null); // if this is null, then we're not recursing

			// TODO this should be pulled out into its own method that JUST returns a list of PackageDependency for us to find
			// get ALL of the dependencies for these packages, grouped by package Id
			// eg. { "log4net" => ["log4net > 2.0", "log4net < 2.5"] }
			var allDependencies = new Dictionary<string, List<PackageDependency>>();
			foreach (var package in packages) {
				foreach (var packageDependency in package.Dependencies) {
					if (packageIds.Contains(packageDependency.PackageId)) continue;
					if (! allDependencies.ContainsKey(packageDependency.PackageId)) 
						allDependencies[packageDependency.PackageId] = new List<PackageDependency>();
					if (! allDependencies[packageDependency.PackageId].Contains(packageDependency))
						allDependencies[packageDependency.PackageId].Add(packageDependency);
				}
			}

			// add these packages' dependencies into discoveredDependencies.
			// we track these to know whether or not we're missing any dependencies for any of the packages found.
			if (discoveredDependencies == null)
				discoveredDependencies = new Dictionary<string, List<PackageDependency>>();
			foreach (var packageDependency in allDependencies) {
				var dependencyId = packageDependency.Key;
				var dependencies = packageDependency.Value.ToArray();
				if (! discoveredDependencies.ContainsKey(dependencyId))
					discoveredDependencies[dependencyId] = new List<PackageDependency>();
				foreach (var dependency in dependencies)
					if (! discoveredDependencies[dependencyId].Contains(dependency))
						discoveredDependencies[dependencyId].Add(dependency);
			}

			foreach (var packageDependency in allDependencies) {
				var dependencyId = packageDependency.Key;
				var dependencies = packageDependency.Value.ToArray();

				// go through all sources and get the *latest* version of this dependency (that matches)
				RemotePackage dependencyPackage = null;
				foreach (var sourcePackages in listsOfPackages) {
					var match = sourcePackages.Where(pkg => pkg.Id == dependencyId && PackageDependency.MatchesAll(pkg.Version, dependencies)).OrderBy(pkg => pkg.Version).Reverse().FirstOrDefault();

					if (match != null)
						if (dependencyPackage == null || dependencyPackage.Version < match.Version)
							dependencyPackage = match;
				}

				if (dependencyPackage != null) {
					found.Add(dependencyPackage);
					if (dependencyPackage.Dependencies.Any())
						found.AddRange(Package.FindDependencies(discoveredDependencies, new Package[]{ dependencyPackage }, listsOfPackages)); // <--- recurse!
				}
				else
					Console.WriteLine("Count not find dependency: {0}", dependencyId);
			}

			// throw a MissingDependencyException if any of the discovered dependencies were not found
			if (throwIfMissing) {
				var foundIds = found.Select(pkg => pkg.Id);
				var missing  = new List<PackageDependency>();

				foreach (var dependencyPackage in discoveredDependencies)
					if (! foundIds.Contains(dependencyPackage.Key))
						missing.AddRange(dependencyPackage.Value);

				if (missing.Count > 0)
					throw new MissingDependencyException(missing);
			}

			// TODO instead of just doing a Distinct(), we need to actually inspect the dependencies ...

			// do not include any of the packages that were passed in as dependencies
			return found.Where(pkg => ! packageIds.Contains(pkg.Id)).Distinct().ToList();
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
