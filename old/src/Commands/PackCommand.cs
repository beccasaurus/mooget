using System;
using System.IO;
using System.Text;
using System.IO.Packaging;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet.Commands {

	///<summary></summary>
	public class PackCommand {

		[Command(Name = "pack", Description = "Generates a nupkg from a nuspec file")]
		public static object Run(string[] args) {
			var response  = new StringBuilder();
			var nuspec    = args[0];
			var package   = Package.FromSpec(nuspec);
			var directory = Path.GetDirectoryName(Path.GetFullPath(nuspec));
			var nupkg     = Path.Combine(Directory.GetCurrentDirectory(), package.IdAndVersion + ".nupkg");

			// todo we'll just get tools, lib, and content!  unless otherwise specified by the nuspec.  we should use the Package class to help us get the paths to files ... LocalPackage?
			var allFiles = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

			// TODO use Nupkg class!  (which should use Zip class, internally)
			response.AppendFormat("Packaging {0}\n", package);
			using (var zip = ZipPackage.Open(nupkg, FileMode.Create)) {
				foreach (var filePath in allFiles) {
					var filename = Path.GetFileName(filePath);
					var relative = Path.GetFullPath(filePath).Replace(directory, "").Replace("\\", "/").Replace(" ", "_");
					var uri  = new Uri(relative, UriKind.Relative);

					// We ONLY support the tools directory right now
					var parts = relative.Split('/');
					if (parts.Length > 2) {
						if (! new List<string>{ "lib", "tools" }.Contains(parts[1].ToLower()))
							continue;
					}
					
					response.AppendFormat("\t{0}\n", relative);

					var part = zip.CreatePart(uri, Util.MimeTypeFor(filename), CompressionOption.Maximum);
					byte[] fileContent = File.ReadAllBytes(filePath);
					part.GetStream().Write(fileContent, 0, fileContent.Length);
				}
			}

			response.AppendFormat("Created {0}\n", Path.GetFileName(nupkg));
			return response;
		}
	}
}
