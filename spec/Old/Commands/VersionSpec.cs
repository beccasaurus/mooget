using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.Commands {

	[TestFixture]
	public class VersionSpec : MooGetSpec {

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
