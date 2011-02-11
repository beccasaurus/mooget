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

		[Test][Description("moo -v")]
		public void moo_v() {
			moo("-v").ShouldEqual(Moo.Version);
		}

		[Test][Description("moo --version")]
		public void moo_version() {
			moo("--version").ShouldEqual(Moo.Version);
		}
	}
}
