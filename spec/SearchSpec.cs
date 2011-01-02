using System;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class SearchSpec : MooGetSpec {

		[TestFixture]
		public class Integration : MooGetSpec {

			[Test]
			public void can_search_a_source() {
				var result = moo("search nhibernate --source {0}", PathToContent("example-feed.xml"));
				result.ShouldContain("FluentNHibernate");
				result.ShouldContain("NHibernate.Core");
				result.ShouldContain("NHibernate.Linq");
				result.ShouldNotContain("NUnit");
			}

		}
	}
}
