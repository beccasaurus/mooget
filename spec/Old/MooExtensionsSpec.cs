using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class MooExtensionsSpec : MooGetSpec {

		[Test]
		public void commands_are_loaded_from_installed_packages() {
			moo("extensiontest").ShouldContain("Command not found: extensiontest");
			moo("commands").ShouldNotContain("command test");

			moo("install {0}", PathToContent("package_working_directories", "just-a-library-1.0.0.0.nupkg"));

			moo("extensiontest").ShouldNotContain("Command not found: extensiontest");
			var output = moo("commands");
			output.ShouldContain("command test");
			output.ShouldContain("extensiontest");
		}

		[Test]
		public void command_filters_are_loaded_from_installed_packages() {
			moo("commands --with-header").ShouldNotContain("HEADER\n------\n");
			moo("filters").ShouldNotContain("ExtensionTestCommand");

			moo("install {0}", PathToContent("package_working_directories", "just-a-library-1.0.0.0.nupkg"));

			moo("commands --with-header").ShouldContain("HEADER\n------\n");
			moo("filters").ShouldContain("ExtensionTestCommand");
		}
	}
}
