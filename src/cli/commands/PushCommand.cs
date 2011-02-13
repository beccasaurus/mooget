using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo push</summary>
	public class PushCommand : MooGetCommand {

		[Command("push", "Push a package to a remote source")]
		public static object Run(string[] args) { return new PushCommand(args).Run(); }

		public PushCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = true;
			Options = new OptionSet() {
				{ "s|source=", v => Source = v }
			};
		}

		/// <summary>A particular source to push to</summary>
		public string Source { get; set; }

		/// <summary>moo push</summary>
		public override object RunDefault() {
			if (Source == null)  return Help;
			if (Args.Count != 1) return Help;

			var path    = Args.First();
			var package = NewPackage.FromFile(path);
			if (package == null) return string.Format("Package file not found: {0}", path);

			var source = MooGet.Source.GetSource(Source);
			if (source == null) return string.Format("Source not found: {0}", Source);

			var remotePackage = source.Push(package);
			if (remotePackage == null)
				return string.Format("{0} could not be pushed to {1}", package, source);
			else
				return string.Format("Pushed {0} to {1}", remotePackage.ToString(), source.Path);
		}
	}
}
