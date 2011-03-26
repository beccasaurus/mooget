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
			return "Doesn't work yet"; // TODO - need to update the Nuspec spec to make Nuspecs from scratch!

			// Right now, ALL this does is generate a stub ...

			/*
			var id     = Path.GetFileName(Directory.GetCurrentDirectory());
			var nuspec = new Nuspec(Path.Combine(Directory.GetCurrentDirectory(), id + ".nuspec"));

			Console.WriteLine("The xml is {0}", nuspec.Xml);
			
			nuspec.Id = id;

			nuspec.Save();
			*/
		}
	}
}
