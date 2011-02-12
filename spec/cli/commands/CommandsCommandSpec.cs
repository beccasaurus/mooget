using System;
using System.Linq;
using System.Reflection;
using MooGet;
using Mono.Options;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class CommandsCommandSpec : MooGetSpec {

		[Test][Description("moo help commands")]
		public void help() {
			var output = moo("help commands");
			output.ShouldContain("Usage: moo commands");
		}

		[Test][Description("moo commands")]
		public void lists_commands() {
			var output = moo("commands");
			output.ShouldMatch(@"commands\s+List all available Moo commands");
		}
	}
}
