using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class PackageVersionSpec : MooGetSpec {

		[TestFixture]
		public class comparing : PackageVersionSpec {

			[Test]
			public void equal_to() {
				(new PackageVersion("1.0.0.0") == new PackageVersion("1.0.0.0")).ShouldBeTrue();
				(new PackageVersion("1.1")     == new PackageVersion("1.1"    )).ShouldBeTrue();
				(new PackageVersion("1.0")     == new PackageVersion("1.0.0"  )).ShouldBeFalse();
				(new PackageVersion("1.0.1")   == new PackageVersion("1.1.0"  )).ShouldBeFalse();
				(new PackageVersion("1.1.0")   == new PackageVersion("1.1"    )).ShouldBeFalse();
			}

			[Test]
			public void greater_than() {
				(new PackageVersion("1.0.0.0") > new PackageVersion("0.0.0.0")).ShouldBeTrue();
				(new PackageVersion("1.0")     > new PackageVersion("0.9"    )).ShouldBeTrue();
				(new PackageVersion("1.0")     > new PackageVersion("0.1.0"  )).ShouldBeTrue();
				(new PackageVersion("1.1")     > new PackageVersion("1.0"    )).ShouldBeTrue();
				(new PackageVersion("1.1")     > new PackageVersion("1.0.1"  )).ShouldBeTrue();
				(new PackageVersion("1.1")     > new PackageVersion("1.0.9.9")).ShouldBeTrue();
				(new PackageVersion("2.0")     > new PackageVersion("1.9"    )).ShouldBeTrue();
				(new PackageVersion("2.0.1")   > new PackageVersion("2.0.0"  )).ShouldBeTrue();
				(new PackageVersion("2.0.1")   > new PackageVersion("2.0"    )).ShouldBeTrue();
				(new PackageVersion("2.0.10")  > new PackageVersion("2.0.9"  )).ShouldBeTrue();
			}

			[Test]
			public void greater_than_or_equal_to() {
				(new PackageVersion("1.0.0") >= new PackageVersion("1.0"  )).ShouldBeTrue();
				(new PackageVersion("1.0.0") >= new PackageVersion("1.0.0")).ShouldBeTrue();
				(new PackageVersion("1.0.1") >= new PackageVersion("1.0.0")).ShouldBeTrue();
				(new PackageVersion("1.0.0") >= new PackageVersion("1.0.1")).ShouldBeFalse();
				(new PackageVersion("1.0.0") >= new PackageVersion("2"    )).ShouldBeFalse();
			}

			[Test]
			public void sorta_greater_than() {
				new PackageVersion("1.0.0").SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeTrue();
				new PackageVersion("1.0.1").SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeTrue();
				new PackageVersion("1.0.9").SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeTrue();
				new PackageVersion("1.1.9").SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeFalse();
				new PackageVersion("1.1.0").SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeFalse();
				new PackageVersion("1.1"  ).SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeFalse();
				new PackageVersion("2"    ).SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeFalse();
				new PackageVersion("1"    ).SortaGreaterThan(new PackageVersion("1.0.0")).ShouldBeFalse(); // 1 is not >= 1.0.0
				new PackageVersion("1.0"  ).SortaGreaterThan(new PackageVersion("1.0"  )).ShouldBeTrue();
				new PackageVersion("1"    ).SortaGreaterThan(new PackageVersion("1"    )).ShouldBeTrue();
				new PackageVersion("1.0"  ).SortaGreaterThan(new PackageVersion("1"    )).ShouldBeTrue();
				new PackageVersion("1.1"  ).SortaGreaterThan(new PackageVersion("1"    )).ShouldBeFalse();
			}

			[Test]
			public void less_than() {
				(new PackageVersion("1.0.0.0") < new PackageVersion("0.0.0.0")).ShouldBeFalse();
				(new PackageVersion("1.0")     < new PackageVersion("0.9"    )).ShouldBeFalse();
				(new PackageVersion("1.0")     < new PackageVersion("0.1.0"  )).ShouldBeFalse();
				(new PackageVersion("1.1")     < new PackageVersion("1.0"    )).ShouldBeFalse();
				(new PackageVersion("1.1")     < new PackageVersion("1.0.1"  )).ShouldBeFalse();
				(new PackageVersion("1.1")     < new PackageVersion("1.0.9.9")).ShouldBeFalse();
				(new PackageVersion("2.0")     < new PackageVersion("1.9"    )).ShouldBeFalse();
				(new PackageVersion("2.0.1")   < new PackageVersion("2.0.0"  )).ShouldBeFalse();
				(new PackageVersion("2.0.1")   < new PackageVersion("2.0"    )).ShouldBeFalse();
				(new PackageVersion("2.0.10")  < new PackageVersion("2.0.9"  )).ShouldBeFalse();
			}

			[Test]
			public void less_than_or_equal_to() {
				(new PackageVersion("1.0.0") <= new PackageVersion("1.0"  )).ShouldBeFalse();
				(new PackageVersion("1.0.0") <= new PackageVersion("1.0.0")).ShouldBeTrue();
				(new PackageVersion("1.0.1") <= new PackageVersion("1.0.0")).ShouldBeFalse();
				(new PackageVersion("1.0.0") <= new PackageVersion("1.0.1")).ShouldBeTrue();
				(new PackageVersion("1.0.0") <= new PackageVersion("2"    )).ShouldBeTrue();
			}

			[Test]
			public void sorta_less_than() {
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("1.0.0")).ShouldBeTrue();
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("1.0.1")).ShouldBeTrue();
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("1.0.9")).ShouldBeTrue();
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("1.1.9")).ShouldBeFalse();
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("1.1.0")).ShouldBeFalse();
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("1.1"  )).ShouldBeFalse();
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("2"    )).ShouldBeFalse();
				new PackageVersion("1.0.0").SortaLessThan(new PackageVersion("1"    )).ShouldBeFalse(); // 1 is not >= 1.0.0
				new PackageVersion("1.0"  ).SortaLessThan(new PackageVersion("1.0"  )).ShouldBeTrue();
				new PackageVersion("1"    ).SortaLessThan(new PackageVersion("1"    )).ShouldBeTrue();
				new PackageVersion("1"    ).SortaLessThan(new PackageVersion("1.0"  )).ShouldBeTrue();
				new PackageVersion("1"    ).SortaLessThan(new PackageVersion("1.1"  )).ShouldBeFalse();
			}
		}

		[Test]
		public void can_get_the_highest_version_number_given_a_list_of_version_numbers() {
			PackageVersion.HighestVersion("0.1.0", "1.0.1.0", "0.9", "0.9.9.9.9").ToString().ShouldEqual("1.0.1.0");
			PackageVersion.HighestVersion("4.10", "1.0", "4.1.34", "3.99.999").ToString().ShouldEqual("4.10");
		}
	}
}
