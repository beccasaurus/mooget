using System;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class SearchSpec : MooGetSpec {

		/*
		[TestFixture]
		public class API : SearchSpec {

			[Test][Ignore]
			public void can_search_for_packages_with_an_exact_id() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_a_matching_id() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_an_exact_title() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_a_matching_title() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_a_matching_description() {
			}

			[Test][Ignore]
			public void can_search_for_packages_that_have_a_certain_tag() {
			}

			[Test][Ignore]
			public void can_search_for_packages_that_have_a_certain_one_of_a_list_of_tags() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_a_matching_license_url() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_an_exact_language() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_an_exact_id_and_version() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_an_exact_id_and_a_minumum_version() {
			}

			[Test][Ignore]
			public void can_search_for_packages_with_an_exact_id_and_a_maximum_version() {
			}
		}
		*/

		[TestFixture]
		public class Integration : SearchSpec {

			[Test]
			public void can_search_a_source() {
				var result = moo("search nhibernate --source {0}", PathToContent("example-feed.xml"));
				result.ShouldContain("FluentNHibernate");
				result.ShouldContain("NHibernate.Core");
				result.ShouldContain("NHibernate.Linq");
				result.ShouldNotContain("NUnit");
			}

			[Test]
			public void shows_all_available_versions() {
				var result = moo("search castle --source {0}", PathToContent("example-feed.xml"));
				result.ShouldContain("Castle.Core (2.5.1, 1.2.0, 1.1.0)");
			}
		}
	}
}
