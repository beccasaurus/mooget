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

		[Test][Description("moo help list")]
		public void help() {
			var help = moo("help list");
			help.ShouldContain("Usage: moo list [STRING] [options]");
			help.ShouldMatch(@"Defaults:\s+--local");
		}

		[Test][Description("moo list")]
		public void lists_locally_installed_packaged_if_no_args_passed() {
			OutputMooCommands = true;
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

		[Test][Description("moo list -s /path/, moo list --source /path/")][Ignore]
		public void can_list_all_packages_in_a_source() {
		}

		[Test][Description("moo list -s Foo, moo list --source Foo")][Ignore]
		public void can_list_all_packages_in_a_source_referenced_by_name() {
		}

		[Test][Description("moo list nuni")][Ignore]
		public void lists_all_local_gem_names_that_start_with_query() {
		}

		[Test][Description("moo list nuni -s Url")][Ignore]
		public void lists_all_gem_names_in_source_that_start_with_query() {
		}
	}
}
