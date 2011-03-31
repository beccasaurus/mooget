using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo fetch</summary>
	public class FetchCommand : MooGetCommand {

		[Command("fetch", "Fetch a package from a remote source")]
		public static object Run(string[] args) { return new FetchCommand(args).Run(); }

		public FetchCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = true;
			Options = new OptionSet() {
				{ "s|source=",  v => Source  = v },
				{ "v|version=", v => Version = v }
			};
		}

		/// <summary>A particular source to fetch from</summary>
		public string Source { get; set; }

		/// <summary>The version of the given package to fetch (optional)</summary>
		public string Version { get; set; }

		/// <summary>moo fetch</summary>
		public override object RunDefault() {
			if (Source == null)  return Help;
			if (Args.Count != 1) return Help;

			var source = MooGet.Source.GetSource(Source);
			if (source == null) return string.Format("Source not found: {0}", Source);

			var packageId  = Args.First();
			var dependency = new PackageDependency(string.Format("{0} {1}", packageId, Version));
			var fetched    = source.Fetch(dependency, Directory.GetCurrentDirectory());

			if (fetched != null)
				return string.Format("Fetched {0}\n", fetched.Name());
			else
				return string.Format("{0} could not be fetched from {1}\n", dependency, source.Path);
		}
	}
}
