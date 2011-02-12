using System;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	/// <summary>Once this spec gets too large because we have too many filters, we should split it up into many specs</summary>
	[TestFixture]
	public class FiltersSpec : Spec {

		[Test][Description("moo")]
		public void SplashScreenFilter() {
			moo().ShouldEqual(@"
 ___________________________________
< NuGet + Super Cow Powers = MooGet >
 -----------------------------------
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||

Run moo help for help documentation".TrimStart('\n'));
		}

		[Test][Description("moo --version, -v")]
		public void moo_version() {
			moo("-v").ShouldEqual(Moo.Version);
			moo("--version").ShouldEqual(Moo.Version);
		}

		[Test][Description("moo --debug, moo -D")]
		public void moo_debug() {
			moo("config"        ).ShouldContain("Debug: False");;
			moo("-D config"     ).ShouldContain("Debug: True");;
			moo("--debug config").ShouldContain("Debug: True");;
		}

		[Test][Description("moo --verbose, moo -V")]
		public void moo_verbose() {
			moo("config"          ).ShouldContain("Verbose: False");;
			moo("-V config"       ).ShouldContain("Verbose: True");;
			moo("--verbose config").ShouldContain("Verbose: True");;
		}
	}
}
