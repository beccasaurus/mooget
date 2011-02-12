using System;
using System.Linq;
using System.Reflection;
using MooGet;
using Mono.Options;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class FiltersCommandSpec : MooGetSpec {

		[Test][Description("moo help filters")]
		public void help() {
		}

		[Test][Description("moo filters")]
		public void lists_filters() {
			moo("filters").ShouldContain("SplashScreenFilter");
			moo("filters").ShouldContain("MooVersionFilter");
		}

		[Test]
		public void is_a_Debug_command() {
			moo("commands").ShouldNotContain("filters ");
			moo("-D commands").ShouldContain("filters ");
			moo("--debug commands").ShouldContain("filters ");
		}
	}
}
