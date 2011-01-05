using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class PackageVersionSpec : MooGetSpec {

		[Test]
		public void can_compare_a_version_number_with_another_version_number() {
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

		[Test][Ignore]
		public void can_get_the_highest_version_number_given_a_list_of_version_numbers() {
		}
	}
}
