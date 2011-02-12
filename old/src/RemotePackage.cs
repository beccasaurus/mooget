using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a Package available from a Source.  May or may not be installed locally.</summary>
	public class RemotePackage : Package {

		public string DownloadUrl { get; set; }

		public override string DetailString {
			get {
				return base.DetailString + string.Format("\nDownloadUrl: {0}", DownloadUrl);
			}
		}

		public string Nupkg {
			get { return IdAndVersion + ".nupkg"; }
		}

		public void Fetch() {
			Util.DownloadFile(DownloadUrl, Nupkg);
		}

		public bool IsInstalled {
			get { return Moo.Packages.Any(pkg => pkg.IdAndVersion == IdAndVersion); }
		}

		public void Install() {
			var tempFile = Path.Combine(Util.TempDir, Nupkg);
			Util.DownloadFile(DownloadUrl, tempFile);
			Moo.Install(tempFile);
		}

		public static List<RemotePackage> LatestPackages {
			get {
				var packages = new List<RemotePackage>();
				foreach (var source in Moo.Sources)
					packages.AddRange(source.LatestPackages);
				return packages;
			}
		}

		public static List<PackageVersion> VersionsAvailableOf(string packageId) {
			return LatestPackages.Where(pkg => pkg.Id == packageId).Select(pkg => pkg.Version).ToList();
		}

		public static PackageVersion HighestVersionAvailableOf(string packageId) {
			return PackageVersion.HighestVersion(VersionsAvailableOf(packageId).ToArray());
		}

		public static RemotePackage FindLatestPackageByName(string name) {
			return LatestPackages.FirstOrDefault(pkg => pkg.Id == name && pkg.Version == HighestVersionAvailableOf(name));
		}
	}
}
