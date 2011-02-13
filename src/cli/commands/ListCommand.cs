using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo list</summary>
	public class ListCommand : MooGetCommand {

		[Command("list", "Display all packages installed or from source")]
		public static object Run(string[] args) { return new ListCommand(args).Run(); }

		public ListCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = false;
			Options = new OptionSet() {
				{ "s|source=", v => Source      = v         },
				{ "l|local",   v => QueryLocal  = v != null },
				{ "r|remote",  v => QueryRemote = v != null }
			};
		}

		/// <summary>A particular source to query against</summary>
		public string Source { get; set; }

		/// <summary>Whether or not we should query the local (installed) packages</summary>
		public bool? QueryLocal { get; set; }

		/// <summary>Whether or not we should query ALL remote sources (added via 'moo source')</summary>
		public bool? QueryRemote { get; set; }

		/// <summary>moo source</summary>
		public override object RunDefault() {
			return string.Format("Running ... args passed: \n{0}", DebugArguments());
		}

		string DebugArguments() {
			return string.Format(@"
QueryLocal:  {0}
QueryRemote: {1}
Source:      {2}
",
QueryLocal, QueryRemote, Source).TrimStart();
		}
	}
}
