using System;
using System.IO;
using System.Linq;
using System.Text;
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

		public virtual object PackNuspec(string nuspecPath) {
			var response = new StringBuilder();

			var nuspec = new Nuspec(Path.GetFullPath(nuspecPath));
			if (! nuspec.Exists())
				return string.Format("Nuspec not found: {0}", nuspecPath);

			var filename = nuspec.IdAndVersion() + ".nupkg";

			// Print out basic information about the package
			response.AppendFormat("Package Id: {0}\n", nuspec.Id);
			response.AppendFormat("Version:    {0}\n", nuspec.Version);
			response.AppendFormat("File:       {0}\n", filename);

			var zip = new Zip(Path.Combine(nuspec.DirName(), filename));

			// We always add the nuspec file to the nupkg
			zip.AddExisting(nuspec.FileName(), nuspec.Path);

			response.AppendFormat("Adding files:\n");

			// Add files specified in the <file> attributes of the Nuspec
			foreach (var fileSource in nuspec.FileSources) {
				foreach (var realFile in fileSource.GetFiles()) {
					var filePath = Path.GetFullPath(realFile);
					var relative = Path.GetFullPath(nuspec.DirName()).AsDir().Relative(filePath);
					if (! string.IsNullOrEmpty(fileSource.Target))
						relative = fileSource.Target + "/" + Path.GetFileName(relative);
					response.AppendFormat("  {0}\n", relative.TrimStart(@"\/".ToCharArray()));
					zip.AddExisting(relative, filePath);
				}
			}

			return response;
		}
	}
}
