using System;
using System.IO;
using System.Net;

namespace MooGet {

	/// <summary>Represents a Package available from a Source.  May or may not be installed locally.</summary>
	public class RemotePackage : Package {

		public string DownloadUrl { get; set; }

		public void Install() {
			var tempFile = Path.Combine(Util.TempDir, IdAndVersion + ".nupkg");
			Util.DownloadFile(DownloadUrl, tempFile);
			Moo.Install(tempFile);
		}
	}
}
