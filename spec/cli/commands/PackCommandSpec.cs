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

		[Test][Description("moo help pack")]
		public void help() {
			var help = moo("help pack");
			help.ShouldContain("Usage: moo pack");
		}

		[Test][Ignore]
		public void can_include_files_from_nuspec() {
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
