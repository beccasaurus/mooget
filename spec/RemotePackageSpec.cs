using System;
using System.IO;
using System.Linq;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class RemotePackageSpec : MooGetSpec {

		// to make these tests easier to write, we should be able to generate a feed, given abunchof packages

		[Test][Ignore]
		public void can_get_all_remote_packages_from_a_source() {
			//var packages = RemotePackage.FromSource(new Source("..."));
		}

		[Test][Ignore]
		public void can_get_all_remote_packages_from_a_list_of_sources() {
			//var packages = RemotePackage.FromSource(new Source("..."), new Source("..."));
		}

		[Test][Ignore]
		public void by_default_all_packages_come_from_Moo_Sources() {
		}

		[Test][Ignore]
		public void can_fetch_a_remote_package() {
		}

		// for more Search-related specs, see SearchSpec
		[Test][Ignore]
		public void can_search_packages() {
		}

		[Test][Ignore]
		public void can_get_all_versions_of_the_packages_returned() {
		}

		[Test][Ignore]
		public void can_get_only_the_latest_versions_of_the_packages_returned() {
		}
	}
}
