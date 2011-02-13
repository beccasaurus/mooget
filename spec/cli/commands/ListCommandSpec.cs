using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class ListCommandSpec : Spec {

		MooDir mooDir            = new MooDir(PathToTemp("home", ".moo"));
		DirectoryOfNupkg source1 = new DirectoryOfNupkg(PathToContent("packages")); // <--- relative
		DirectoryOfNupkg source2 = new DirectoryOfNupkg(PathToContent("more_packages"));

		[SetUp]
		public void Before() {
			base.BeforeEach();
			OutputMooCommands = true;
		}

		[Test][Description("moo help list")]
		public void help() {
			var help = moo("help list");
			help.ShouldContain("Usage: moo list [STRING] [options]");
			help.ShouldMatch(@"Defaults:\s+--local");
		}

		[Test][Description("moo list")]
		public void lists_locally_installed_packaged_if_no_args_passed() {
			mooDir.Packages.Count.ShouldEqual(0);

			var output = moo("list");
			output.ShouldEqual("No packages");
			output.ShouldNotContain("MarkdownSharp");

			mooDir.Install(new PackageDependency("MarkdownSharp"), source1);
			mooDir.Packages.Count.ShouldEqual(1);

			output = moo("list");
			output.ShouldContain("MarkdownSharp");
			output.ShouldContain("1.13.0.0"); // version
		}

		[Test][Description("moo list -s /path/, moo list --source /path/")]
		public void can_list_all_packages_in_a_source() {
			mooDir.Packages.Count.ShouldEqual(0);

			// uninstalled source, by url
			var output = moo("list --source {0}", source1.Path);
			output.ShouldContainAll("FluentNHibernate (1.1.0.694)", "NUnit (2.5.7.10213)", "Ninject (2.1.0.76)");

			// no name ...
			moo("list --source Foo").ShouldContain("Source not found: Foo");

			// "installed" source, by name
			moo("source add Foo {0}", source1.Path);
			moo("list --source Foo").ShouldContainAll("FluentNHibernate (1.1.0.694)", "NUnit (2.5.7.10213)", "Ninject (2.1.0.76)");

			mooDir.Packages.Count.ShouldEqual(0);
		}

		[Test][Description("moo list nuni")][Ignore]
		public void lists_all_local_gem_names_that_start_with_query() {
		}

		[Test][Description("moo list nuni -s Url")]
		public void lists_all_gem_names_in_source_that_start_with_query() {
			moo("list N --source Foo").ShouldContain("Source not found: Foo");

			var output = moo("list N --source {0}", source1.Path);
			output.ShouldContain("NUnit");
			output.ShouldContain("Ninject");
			output.ShouldNotContain("FluentNHibernate");
			output.ShouldNotContain("MarkdownSharp");

			output = moo("list --source {0} nin", source1.Path);
			output.ShouldContain("Ninject");
			output.ShouldNotContain("NUnit");
			output.ShouldNotContain("FluentNHibernate");
			output.ShouldNotContain("MarkdownSharp");

			output = moo("list --source {0} m", source1.Path);
			output.ShouldContain("MarkdownSharp");
			output.ShouldNotContain("Ninject");
			output.ShouldNotContain("NUnit");
			output.ShouldNotContain("FluentNHibernate");
		}
	}
}
