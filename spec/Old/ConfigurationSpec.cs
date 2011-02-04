using System;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class ConfigurationSpec : MooGetSpec {

		[TestFixture]
		public class API : MooGetSpec {
			
		}

		[TestFixture]
		public class Integration : MooGetSpec {

			[Test]
			public void mooDir_default_to_inside_fake_home_directory_for_specs() {
				moo("config").ShouldContain("mooDir: " + PathToTemp("home"));
			}

			[Test][Ignore]
			public void mooDir_can_be_overriden_from_the_commandline(){}

			[Test][Ignore]
			public void mooDir_can_be_overriden_from_a_local_moorc(){}
		}
	}
}
