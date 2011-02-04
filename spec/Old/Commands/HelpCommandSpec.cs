using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture][Description("Command: moo help")]
	public class HelpCommandSpec : MooGetSpec {

		[Test][Description("moo help")]
		public void moo_help() {
			var output = moo("help");

			output.ShouldEqual(@"
MooGet is a sophisticated package manager for .NET.

  Usage:
    moo -h/--help
    moo -v/--version

  Examples:
    moo install rake
    moo list --local
    moo build package.nuspec
    moo help install

  Further help:
    moo commands                 list all 'moo' commands
    moo help examples            show some examples of usage
    moo help <COMMAND>           show help on COMMAND
                                   (e.g. 'moo help install')

  Futher information:
    http://mooget.net
      		".Trim());
		}

		[Test][Ignore][Description("moo help help")]
		public void moo_help_help() {
		}

		[Test][Ignore][Description("moo help commands")]
		public void moo_help_command__builtin() {
		}

		[Test][Ignore][Description("moo help idontexist")]
		public void moo_help_command__not_found() {
		}

		[Test][Ignore][Description("moo help customcommand")]
		public void moo_help_command__custom() {
		}
	}
}
