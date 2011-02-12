using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/*
	 * OBSOLETE 
	 *
	 * Going to port any required code from here into the new Source ...
	 *
	 */
	public class OldSource {

		List<RemotePackage> _allPackages;

		/// <summary>URL or local filesystem path to this source's XML feed</summary>
		public string Path { get; set; }

		public OldSource(){}
		public OldSource(string path) {
			Path = path;
		}

		public List<RemotePackage> Packages {
			get { return LatestPackages; }
		}

		public List<RemotePackage> LatestPackages {
			get { return RemoveAllVersionsButLatest(AllPackages); }
		}

		public List<RemotePackage> AllPackages {
			get {
				if (_allPackages == null)
					return OldSource.GetPackagesFromPath(Path).OrderBy(p => p.Title).ToList();
				else
					return _allPackages;
			}
			set { _allPackages = value; }
		}

		// TODO obsolete this!  it doesn't make any sense.  you can Get(id and version) but not just ID!  ID should return a LIST of packages!
		public RemotePackage Get(string id) {
			id = id.ToLower();
			return LatestPackages.FirstOrDefault(p => p.Id.ToLower() == id);
		}

		// TODO this uses AllPackages, but others don't?  omg that's icky ... clean this up!  I don't like any of these Search() methods ...
		public List<RemotePackage> SearchByTitle(string query) {
			query = query.ToLower();
			return AllPackages.Where(p => p.Title.ToLower().Contains(query)).ToList();
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

		public static List<OldSource> GetSources() {
			if (File.Exists(Moo.SourceFile))
				return FromSourceFile(Moo.SourceFile);
			else
				return Moo.DefaultSources;
		}

		// each line in ~/.moo/sources.list has a source url
		public static List<OldSource> FromSourceFile(string path) {
			var sources = new List<OldSource>();
			foreach (var line in Util.ReadFile(path).Split('\n'))
				if (line.Trim().Length > 0)
					sources.Add(new OldSource(line.Trim()));
			return sources;
		}

		public static void AddSource(string path) {
			Moo.InitializeMooDir();

			// if the SourceFile doesn't exist yet, 
			// write all existing sources to the file
			if (! File.Exists(Moo.SourceFile))
				foreach (var source in Moo.Sources)
					Util.AppendToFile(Moo.SourceFile, source.Path + "\n");

			Util.AppendToFile(Moo.SourceFile, path + "\n");
		}

		public static void RemoveSource(string path) {
			Moo.InitializeMooDir();

			var sourcePaths = GetSources().Select(source => source.Path).ToList();
			sourcePaths.Remove(path);
			var sourceContent = string.Join("\n", sourcePaths.ToArray()) + "\n";

			Util.WriteFile(Moo.SourceFile, sourceContent);
		}

		public void LoadXml(string xml) {
			AllPackages = Feed.ParseFeed(xml);
		}

		public static OldSource FromXml(string xml) {
			var source = new OldSource();
			source.LoadXml(xml);
			return source;
		}

		// TODO deprecated ... see how LocalPackage does this.  we're going to implement something similar in RemotePackage.  A Source doesn't need to have this implementation ...
		public static List<RemotePackage> RemoveAllVersionsButLatest(List<RemotePackage> packages) {
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
	}
}
