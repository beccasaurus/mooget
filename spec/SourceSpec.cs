using System;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class SourceSpec : MooGetSpec {

		// see ./spec/content/example-feed.xml to see the feed that these specs use
		static Source              source   = new Source(PathToContent("example-feed.xml"));
		static List<SourcePackage> packages = source.Packages;

		[Test]
		public void can_get_all_packages_from_a_source() {
			Assert.That( packages.Count,     Is.EqualTo(151));
			Assert.That( packages[0].Name,   Is.EqualTo("Adam.JSGenerator"));
			Assert.That( packages[150].Name, Is.EqualTo("xmlbuilder"));
		}

		[Test]
		public void can_search_for_package_by_name() {}

		[Test]
		public void can_search_for_package_by_description() {}

		[Test]
		public void can_search_for_package_by_name_or_description() {}
	}
}
