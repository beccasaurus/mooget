using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class MoofileDependencySpec : MooGetSpec {

		[Test]
		public void has_reference_back_to_Moofile() {
			var moofile = new Moofile(PathToContent("moofile_examples", "Moofile1"));
			moofile.GlobalDependencies.First().Text.ShouldEqual("log4net");
			moofile.GlobalDependencies.First().Moofile.ShouldEqual(moofile);
		}

		[Test][Ignore]
		public void has_reference_back_to_Group_if_no_global() {
		}

		[Test][Ignore]
		public void has_reference_back_to_GlobalGroup_if_global() {
		}

		[Test][Ignore]
		public void knows_whether_its_referencing_a_dll() {
		}

		[Test][Ignore]
		public void knows_whether_its_referencing_a_nupkg() {
		}

		[Test][Ignore]
		public void knows_whether_its_referencing_a_directory() {
		}

		[Test][Ignore]
		public void knows_whether_its_referencing_a_csproj() {
		}

		[Test][Ignore]
		public void knows_whether_its_referencing_a_project_defined_in_a_sln_file() {
		}
	}
}
