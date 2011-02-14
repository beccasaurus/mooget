using System;
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
			// ...
			return "PACK";
		}
	}
}
