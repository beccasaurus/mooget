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
			moo("config"        ).ShouldMatch(@"Debug:\s+False");;
			moo("-D config"     ).ShouldMatch(@"Debug:\s+True");;
			moo("--debug config").ShouldMatch(@"Debug:\s+True");;
		}

		[Test][Description("moo --verbose, moo -V")]
		public void moo_verbose() {
			moo("config"          ).ShouldMatch(@"Verbose:\s+False");;
			moo("-V config"       ).ShouldMatch(@"Verbose:\s+True");;
			moo("--verbose config").ShouldMatch(@"Verbose:\s+True");;
		}
	}
}
