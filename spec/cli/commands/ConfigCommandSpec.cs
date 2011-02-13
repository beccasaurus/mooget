using System;
using System.Linq;
using System.Reflection;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class ConfigCommandSpec : Spec {

		[Test][Description("moo help config")]
		public void help() {
			moo("help config").ShouldContain("Usage: moo config");
		}

		[Test][Description("moo config")]
		public void prints_out_miscellaneous_Moo_configuration_settings() {
			var output = moo("config");
			output.ShouldMatch(@"MooDir:\s+" + PathToTemp("home"));
			output.ShouldMatch(@"Debug:\s+False");
			output.ShouldMatch(@"Verbose:\s+False");
			output.ShouldMatch(@"System HOME:\s+" + PathToTemp("home"));
			output.ShouldMatch(@"System TMP:\s+" + PathToTemp("tmp"));
		}
	}
}
