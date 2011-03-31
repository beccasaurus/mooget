using System;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo pack</summary>
	public class PackCommand : MooGetCommand {

		[Command("pack", "Pack a package from a remote source")]
		public static object Run(string[] args) { return new PackCommand(args).Run(); }

		public PackCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = true;
			// Options = new OptionSet() {
			// 	{ "s|source=",  v => Source  = v },
			// 	{ "v|version=", v => Version = v }
			// };
		}

		/// <summary>moo pack</summary>
		public override object RunDefault() {
			if (Args.Count == 1)
				return PackNuspec(Args.First());
			else
				return "Nothing yet ... you need to pass in a nuspec to pack ...";
		}

		public virtual string RandomXsdId {
			get {
				var id    = "";
				var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
				while (id.Length < 16) {
					id += chars[new Random((int) DateTime.Now.Ticks).Next(chars.Length)];
				}
				return "R" + id;
			}
		}

		// TODO - this whole method (except for the writelines) should be available on the Nuspec or Nupkg classes
		public virtual object PackNuspec(string nuspecPath) {
			var response = new StringBuilder();

			var nuspec = new Nuspec(Path.GetFullPath(nuspecPath));
			if (! nuspec.Exists())
				return string.Format("Nuspec not found: {0}\n", nuspecPath);

			var filename = nuspec.IdAndVersion() + ".nupkg";

			// Print out basic information about the package
			response.AppendFormat("Package Id: {0}\n", nuspec.Id);
			response.AppendFormat("Version:    {0}\n", nuspec.Version);
			response.AppendFormat("File:       {0}\n", filename);

			var zip = new Zip(Path.Combine(nuspec.DirName(), filename));
			if (zip.Exists())
				return string.Format("Package already exists: {0}\n", zip.FileName());

			// We always add the nuspec file to the nupkg
			zip.AddExisting(nuspec.FileName(), nuspec.Path);

			response.AppendFormat("Adding files:\n");

			// Add files specified in the <file> attributes of the Nuspec
			foreach (var fileSource in nuspec.FileSources) {
				foreach (var realFile in fileSource.GetFiles()) {
					var filePath = Path.GetFullPath(realFile);
					var relative = Path.GetFullPath(nuspec.DirName()).AsDir().Relative(filePath);
					if (! string.IsNullOrEmpty(fileSource.Target))
						relative = fileSource.Target.Replace("\\", "/") + "/" + Path.GetFileName(relative);
					response.AppendFormat("  {0}\n", relative.TrimStart(@"\/".ToCharArray()));
					zip.AddExisting(relative, filePath);
				}
			}

			// Ugh, need to do some ugly OpenXML bullshit ...

			using (var package = ZipPackage.Open(zip.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				// First, let's set some properties
				package.PackageProperties.Creator     = nuspec.AuthorsText;
				package.PackageProperties.Description = nuspec.Description;
				package.PackageProperties.Identifier  = nuspec.Id;
				package.PackageProperties.Version     = nuspec.VersionText;

				// Now, let's create an internal 'Relationship' to the nuspec file (for some reason)
				var nuspecUri           = new Uri("/" + nuspec.FileName(), UriKind.Relative);
				var someCrappyNamespace = "http://schemas.microsoft.com/packaging/2010/07/manifest";
				package.CreateRelationship(nuspecUri, TargetMode.Internal, someCrappyNamespace, RandomXsdId);

				// If there's a relationship with an (invalid) ID of 0, delete it and create it with a unique ID
                try {
				    var invalid = package.GetRelationship("0");
				    if (invalid != null) {
					    package.DeleteRelationship("0");
					    package.CreateRelationship(invalid.TargetUri, invalid.TargetMode, invalid.RelationshipType, RandomXsdId);
				    }
                } catch (Exception ex) {
                    if (! ex.Message.Contains("'0' ID is not a valid XSD ID")) // this is a safe exception to swallow
                        throw ex;
                }
			}

			return response;
		}
	}
}
