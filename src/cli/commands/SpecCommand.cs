using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo spec</summary>
	public class SpecCommand : MooGetCommand {

		[Command("spec", "Generate and modify nuspec files")]
		public static object Run(string[] args) { return new SpecCommand(args).Run(); }

		public SpecCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = false;
			// Options = new OptionSet() {
			// 	{ "s|source=",  v => Source  = v },
			// 	{ "v|version=", v => Version = v }
			// };
		}

		/// <summary>moo spec</summary>
		public override object RunDefault() {
			var id     = Path.GetFileName(Directory.GetCurrentDirectory());
			var nuspec = new Nuspec(Path.Combine(Directory.GetCurrentDirectory(), id + ".nuspec"));

			if (nuspec.Exists()) {
				return string.Format("Nuspec already exists: {0}", nuspec.FileName());
			} else {
				nuspec.Id          = id;
				nuspec.VersionText = "1.0.0";
				nuspec.AuthorsText = "me <me@email.com>";
				nuspec.Description = "About " + id;
				nuspec.ProjectUrl  = "https://github.com/me/" + id;
				nuspec.Dependencies = new List<PackageDependency> {
					new PackageDependency("SomePackage >= 1.0")
				};
				nuspec.FileSources = new List<NuspecFileSource> {
					new NuspecFileSource { Source = @"bin\Release\*.dll", Target = "lib" },
					new NuspecFileSource { Source = @"bin\Release\*", Target = "tools"   },
					new NuspecFileSource { Source = @"content"                           },
					new NuspecFileSource { Source = @"src"                               }
				};
				nuspec.Save();
				return string.Format("Generated {0}", nuspec.FileName());
			}
		}
	}
}
