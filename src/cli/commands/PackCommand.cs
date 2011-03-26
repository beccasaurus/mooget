using System;
using System.IO;
using System.Linq;
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
			var response = "";

			var nuspec = new Nuspec(nuspecPath);
			if (! nuspec.Exists())
				return string.Format("Nuspec not found: {0}", nuspecPath);

			var zip = new Zip(Path.Combine(nuspec.DirName(), nuspec.IdAndVersion() + ".nupkg"));

			// We always add the nuspec file to the nupkg
			zip.AddExisting(nuspec.FileName(), nuspec.Path);

			// Add files specified in the <file> attributes of the Nuspec
			foreach (var fileSource in nuspec.FileSources) {
				foreach (var realFile in fileSource.GetFiles()) {
					var filePath = Path.GetFullPath(realFile);
					var relative = Path.GetFullPath(nuspec.DirName()).AsDir().Relative(filePath);
					zip.AddExisting(relative, filePath);
				}
			}

			return response;
		}
	}
}
