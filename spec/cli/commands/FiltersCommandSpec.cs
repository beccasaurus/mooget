using System;
using System.Linq;
using System.Reflection;
using MooGet;
using MooGet.Options;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class FiltersCommandSpec : MooGetSpec {

		[Test]
		public void is_a_Debug_command() {
			moo("commands").ShouldNotContain("filters ");
			moo("-D commands").ShouldContain("filters ");
			moo("--debug commands").ShouldContain("filters ");
		}

		[Test]
		public void lists_filters() {
			moo("filters").ShouldContain("SplashScreenFilter");
			moo("filters").ShouldContain("MooVersionFilter");
		}
	}
}
