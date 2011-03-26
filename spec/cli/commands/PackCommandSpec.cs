using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class PackCommandSpec : Spec {

		[SetUp]
		public void Before() {
			base.BeforeEach();
			
			// Copy spec/content/my_nuspecs into tmp
			PathToContent("my_nuspecs").AsDir().Copy(PathToTemp("working", "my_nuspecs"));
		}

		[Test][Description("moo help pack")]
		public void help() {
			var help = moo("help pack");
			help.ShouldContain("Usage: moo pack");
		}

		[Test][Description("<file src='README' />")]
		public void can_include_files_from_nuspec() {
			var output = moo("pack my_nuspecs/file-src.nuspec");

			var nupkg = new Nupkg(PathToTemp("working", "my_nuspecs", "file-src-1.0.nupkg"));
			nupkg.Exists().Should(Be.True);
			nupkg.Files.ShouldEqual(new List<string>{
				"file-src.nuspec",
				"README"
			});
		}

		[Test][Description(@"<file src='doc\*' />")][Ignore]
		public void can_include_files_from_nuspec_with_wildcards() {
		}

		[Test][Description(@"<file src='README' target='foo' />")][Ignore]
		public void can_include_files_from_nuspec_with_target() {
		}

		[Test][Description(@"<file src='doc\*' target='foo' />")][Ignore]
		public void can_include_files_from_nuspec_with_target_with_wildcards() {
		}

		[Test][Ignore]
		public void can_specify_directory_to_use_for_lib() {
		}

		[Test][Ignore]
		public void can_specify_directory_to_use_for_tools() {
		}

		[Test][Ignore]
		public void can_specify_directory_to_use_for_content() {
		}

		[Test][Ignore]
		public void can_specify_directory_to_use_for_src() {
		}

		[Test][Ignore]
		public void can_specify_that_you_want_to_use_all_defaults() {
		}
	}
}
