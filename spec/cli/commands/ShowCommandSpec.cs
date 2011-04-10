using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class ShowCommandSpec : Spec {

		DirectoryOfNupkg remoteSource = new DirectoryOfNupkg(PathToContent("more_packages"));

		[SetUp]
		public void Before() {
			base.BeforeEach();	

			// Copy content\moo_dir_for_moo_show to our fake ~/.moo to fake abunchof installed packages
			PathToContent("moo_dir_for_moo_show").AsDir().Copy(PathToTempHome(".moo"));
		}

		[Test][Description("moo help show")]
		public void moo_help_show() {
			moo("help show").ShouldContain("Usage: moo show");
		}

		[Test][Description("moo show")]
		public void moo_show() {
			moo("help show").ShouldContain("Usage: moo show");
		}

		[Test][Description("moo show NoExist")]
		public void moo_show_NoExist() {
			moo("show log4net").ShouldEqual(""); // this is not installed (but it is in the remote source)
		}

		[Test][Description("moo show Package1")]
		public void moo_show_Package1() {
			var output = moo("show SpecFlow");
			output.ShouldMatch(@"Id:\s+SpecFlow");
			output.ShouldMatch(@"Version:\s+1.5.0");
			output.ShouldNotMatch(@"Version:\s+1.4.0"); // doesn't show the old version
		}

		[Test][Description("moo show \"Package1 1.0\"")]
		public void moo_show_Package1_specific_version() {
			var output = moo("show 'SpecFlow 1.4.0'");
			output.ShouldMatch(@"Id:\s+SpecFlow");
			output.ShouldMatch(@"Version:\s+1.4.0");
			output.ShouldNotMatch(@"Version:\s+1.5.0"); // doesn't show the latest version
		}

		[Test][Description("moo show Package1 --source RemoteSource")]
		public void moo_show_Package1_remote_source() {
			var output = moo("show log4net --source {0}", remoteSource.Path);
			output.ShouldMatch(@"Id:\s+log4net");
			output.ShouldMatch(@"Version:\s+1.2.10");
		}

		[Test][Description("moo show Package1 Package2")]
		public void moo_show_Package1_Package2() {
			var output = moo("show NUnit 'SpecFlow 1.4.0' TaskMan");
			output.ShouldMatch(@"Id:\s+NUnit");
			output.ShouldMatch(@"Version:\s+2.5.9.10348");

			output.ShouldMatch(@"Id:\s+SpecFlow");
			output.ShouldMatch(@"Version:\s+1.4.0");
			
			output.ShouldMatch(@"Id:\s+TaskMan");
			output.ShouldMatch(@"Version:\s+1.1.0.0");
		}

		[Test][Description("moo show libraries")][Ignore]
		public void moo_show_libraries() {
			// show help docs for 'show libraries' ?
		}

		[Test][Description("moo show libraries Package1")][Ignore]
		public void moo_show_libraries_Package1() {
			var output = moo("show libraries SpecFlow");
			output.ShouldContain(PathToTempHome(".moo", "packages", "SpecFlow-1.5.0", "lib", "NET35", "TechTalk.SpecFlow.dll"));
			output.ShouldContain(PathToTempHome(".moo", "packages", "SpecFlow-1.5.0", "lib", "SL3", "TechTalk.SpecFlow.Silverlight.dll"));

			output = moo("show libraries NUnit");
			output.ShouldContain(PathToTempHome(".moo", "packages", "NUnit-2.5.9.10348", "lib", "nunit.framework.dll"));
			output.ShouldContain(PathToTempHome(".moo", "packages", "NUnit-2.5.9.10348", "lib", "nunit.mocks.dll"));
			output.ShouldContain(PathToTempHome(".moo", "packages", "NUnit-2.5.9.10348", "lib", "pnunit.framework.dll"));
		}

		[Test][Description("moo show libraries Package1 --framework NET20")]
		public void moo_show_libraries_Package1_specific_framework() {
			var output = moo("show libraries NUnit.Should");
			output.ShouldContain("NET20");
			output.ShouldContain("NET30");

			output = moo("show libraries NUnit.Should --framework NET20");
			output.ShouldContain("NET20");
			output.ShouldNotContain("NET30");

			output = moo("show libraries NUnit.Should --framework 30");
			output.ShouldNotContain("NET20");
			output.ShouldContain("NET30");
		}

		[Test][Description("moo show libraries Package1 Package2")]
		public void moo_show_libraries_Package1_Package2() {
			var output = moo("show libraries SpecFlow NUnit");
			output.ShouldContain(PathToTempHome(".moo", "packages", "SpecFlow-1.5.0", "lib", "NET35", "TechTalk.SpecFlow.dll"));
			output.ShouldContain(PathToTempHome(".moo", "packages", "SpecFlow-1.5.0", "lib", "SL3", "TechTalk.SpecFlow.Silverlight.dll"));
			output.ShouldContain(PathToTempHome(".moo", "packages", "NUnit-2.5.9.10348", "lib", "nunit.framework.dll"));
			output.ShouldContain(PathToTempHome(".moo", "packages", "NUnit-2.5.9.10348", "lib", "nunit.mocks.dll"));
			output.ShouldContain(PathToTempHome(".moo", "packages", "NUnit-2.5.9.10348", "lib", "pnunit.framework.dll"));
		}

		[Test][Description("moo show libraries Package1 --relative")]
		public void moo_show_libraries_Package1_relative() {
			var output = moo("show libraries NUnit --relative");
			output.ShouldContain("packages/NUnit-2.5.9.10348/lib/nunit.framework.dll");
			output.ShouldNotContain(PathToTempHome()); // doesn't have the root path at all
		}

		[Test][Description("moo show libraries Package1 --env=SomeVariable")]
		public void moo_show_libraries_Package1P_env() {
			moo("show libraries Requestor --env FOO_BAR").ShouldEqual("$(FOO_BAR)/packages/Requestor-1.0.2.2/lib/Requestor.dll");
		}

		[Test][Description("moo show libraries Package1 --moopath")]
		public void moo_show_libraries_Package1_moopath() {
			moo("show libraries Requestor -m").ShouldEqual("$(MOO_DIR)/packages/Requestor-1.0.2.2/lib/Requestor.dll");
		}

		[Test][Description("moo show libraries Package1 --moopath --dependencies")]
		public void moo_show_libraries_Package1_moopath_dependencies() {
			moo("show libraries NUnit.Should").ShouldNotContain("nunit.framework.dll");
			moo("show libraries NUnit.Should --dependencies").ShouldContain("nunit.framework.dll");
		}
	}
}
