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
			output.ShouldContain("MooDir: " + PathToTemp("home"));
			output.ShouldContain("Debug: False");
			output.ShouldContain("Verbose: False");
			output.ShouldContain("System HOME: " + PathToTemp("home"));
			output.ShouldContain("System TMP: " + PathToTemp("tmp"));
		}
	}
}
