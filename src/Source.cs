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

		public List<RemotePackage> Packages {
			get { return LatestPackages; }
		}

		public List<RemotePackage> LatestPackages {
			get { return RemoveAllVersionsButLatest(AllPackages); }
		}

		// TODO deprecated ... see how LocalPackage does this.  we're going to implement something similar in RemotePackage.  A Source doesn't need to have this implementation ...
		static List<RemotePackage> RemoveAllVersionsButLatest(List<RemotePackage> packages) {
			var latestPackages  = new List<RemotePackage>();
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

		public List<RemotePackage> AllPackages {
			get { return Source.GetPackagesFromPath(Path).OrderBy(p => p.Title).ToList(); }
		}

		public RemotePackage Get(string id) {
			id = id.ToLower();
			return LatestPackages.FirstOrDefault(p => p.Id.ToLower() == id);
		}

		public List<RemotePackage> SearchByTitle(string query) {
			query = query.ToLower();
			return Packages.Where(p => p.Title.ToLower().Contains(query)).ToList();
		}

		public List<RemotePackage> SearchByDescription(string query) {
			query = query.ToLower();
			return Packages.Where(p => p.Description.ToLower().Contains(query)).ToList();
		}

		public List<RemotePackage> SearchByTitleOrDescription(string query) {
			query = query.ToLower();
			return Packages.Where(p => p.Title.ToLower().Contains(query) || p.Description.ToLower().Contains(query)).ToList();
		}

		static List<RemotePackage> GetPackagesFromPath(string path) {
			if (File.Exists(path))
				return Feed.ParseFeed(MooGet.Util.ReadFile(path));
			else
				return Feed.ParseFeed(MooGet.Util.ReadUrl(path));
		}
	}
}
