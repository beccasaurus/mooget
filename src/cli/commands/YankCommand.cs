using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo yank</summary>
	public class YankCommand : MooGetCommand {

		[Command("yank", "Yank a package from a remote source")]
		public static object Run(string[] args) { return new YankCommand(args).Run(); }

		public YankCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = true;
			Options = new OptionSet() {
				{ "s|source=",  v => Source  = v },
				{ "v|version=", v => Version = v }
			};
		}

		/// <summary>A particular source to yank from</summary>
		public string Source { get; set; }

		/// <summary>The version of the given package to yank</summary>
		public string Version { get; set; }

		/// <summary>moo yank</summary>
		public override object RunDefault() {
			Console.WriteLine("YANK Source:{0} Version:{1} Args:{2}", Source, Version, Args.Join(", "));

			if (Source == null)  return Help;
			if (Version == null) return Help;
			if (Args.Count != 1) return Help;

			var source = MooGet.Source.GetSource(Source);
			if (source == null) return string.Format("Source not found: {0}", Source);

			var packageId  = Args.First();
			var dependency = new PackageDependency(string.Format("{0} {1}", packageId, Version));

			if (source.Yank(dependency))
				return string.Format("Yanked {0} from {1}", dependency, source.Path);
			else
				return string.Format("{0} could not be yanked from {1}", dependency, source.Path);
		}
	}
}
