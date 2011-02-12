using System;
using System.Linq;
using System.Reflection;
using MooGet;
using MooGet.Options;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class HelpCommandSpec : MooGetSpec {

		[Test][Description("moo help")]
		public void prints_out_the_main_moo_help_screen_if_called_without_argumetns() {
			moo("help").ShouldContain("MooGet is a sophisticated package manager for .NET.");
		}

		[Test][Description("moo help help")]
		public void help() {
			moo("help help").ShouldContain("Usage: moo help COMMAND");
		}

		[Test][Description("moo help commands")]
		public void prints_out_help_for_the_given_command() {
			moo("help commands").ShouldContain("Usage: moo commands");
		}

		[Test][Description("moo help idontexist")]
		public void help_for_unknown_command() {
			moo("help idontexist").ShouldEqual("Command not found: idontexist");
		}
	}
}
