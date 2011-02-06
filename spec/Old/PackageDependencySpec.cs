using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class PackageDependencySpec : MooGetSpec {

		// Helper method to get a PackageDependency using the given VersionText
		PackageDependency Dep(string versionString) {
			return new PackageDependency { VersionText = versionString };
		}

		[Test]
		public void constructor_parses_package_name_and_versions() {
			new PackageDependency("MyPackage"            ).ToString().ShouldEqual("MyPackage");
			new PackageDependency("MyPackage = 1.0"      ).ToString().ShouldEqual("MyPackage = 1.0");
			new PackageDependency("MyPackage >1.0 <2.0"  ).ToString().ShouldEqual("MyPackage > 1.0 < 2.0");
			new PackageDependency("MyPackage > 1.0 < 2.0").ToString().ShouldEqual("MyPackage > 1.0 < 2.0");
			new PackageDependency("MyPackage 1.0"        ).ToString().ShouldEqual("MyPackage = 1.0");
		}

		[Test]
		public void can_compare_2_PackageDependency() {
			new PackageDependency("MyPackage 1.0" ).ShouldNotEqual(new PackageDependency("MyPackage 1.1"));
			new PackageDependency("MyPackage 1.0" ).ShouldEqual(   new PackageDependency("MyPackage 1.0"));
			new PackageDependency("MyPackage 1.0" ).ShouldEqual(   new PackageDependency("MyPackage = 1.0"));
			new PackageDependency("MyPackage 1.0" ).ShouldNotEqual(new PackageDependency("MyPackage > 1.0"));
			new PackageDependency("MyPackage >1.0").ShouldEqual(   new PackageDependency("MyPackage > 1.0"));

			// same should all be true with the == operator
			(new PackageDependency("MyPackage 1.0" ) == new PackageDependency("MyPackage 1.1")).ShouldBeFalse();
			(new PackageDependency("MyPackage 1.0" ) == new PackageDependency("MyPackage 1.0")).ShouldBeTrue();
			(new PackageDependency("MyPackage 1.0" ) == new PackageDependency("MyPackage = 1.0")).ShouldBeTrue();
			(new PackageDependency("MyPackage 1.0" ) == new PackageDependency("MyPackage > 1.0")).ShouldBeFalse();
			(new PackageDependency("MyPackage >1.0") == new PackageDependency("MyPackage > 1.0")).ShouldBeTrue();
		}

		[TestFixture]
		public class parsing : PackageDependencySpec {

			[Test]
			public void operator_parsing() {
				PackageDependency.ParseOperator(null  ).ShouldEqual(PackageDependency.Operators.EqualTo); // Default is EqualTo
				PackageDependency.ParseOperator(""    ).ShouldEqual(PackageDependency.Operators.EqualTo); //
				PackageDependency.ParseOperator("="   ).ShouldEqual(PackageDependency.Operators.EqualTo);
				PackageDependency.ParseOperator(" ="  ).ShouldEqual(PackageDependency.Operators.EqualTo);
				PackageDependency.ParseOperator(" = " ).ShouldEqual(PackageDependency.Operators.EqualTo);
				PackageDependency.ParseOperator(" ==" ).ShouldEqual(PackageDependency.Operators.EqualTo);
				PackageDependency.ParseOperator(" == ").ShouldEqual(PackageDependency.Operators.EqualTo);
				PackageDependency.ParseOperator(">"   ).ShouldEqual(PackageDependency.Operators.GreaterThan);
				PackageDependency.ParseOperator(">="  ).ShouldEqual(PackageDependency.Operators.GreaterThanOrEqualTo);
				PackageDependency.ParseOperator("=>"  ).ShouldEqual(PackageDependency.Operators.GreaterThanOrEqualTo);
				PackageDependency.ParseOperator("<"   ).ShouldEqual(PackageDependency.Operators.LessThan);
				PackageDependency.ParseOperator("<="  ).ShouldEqual(PackageDependency.Operators.LessThanOrEqualTo);
				PackageDependency.ParseOperator("=<"  ).ShouldEqual(PackageDependency.Operators.LessThanOrEqualTo);
				PackageDependency.ParseOperator(" =<" ).ShouldEqual(PackageDependency.Operators.LessThanOrEqualTo);
				PackageDependency.ParseOperator(" =< ").ShouldEqual(PackageDependency.Operators.LessThanOrEqualTo);
				PackageDependency.ParseOperator("~>"  ).ShouldEqual(PackageDependency.Operators.SortaGreaterThan);
				PackageDependency.ParseOperator(">~"  ).ShouldEqual(PackageDependency.Operators.SortaGreaterThan);
				PackageDependency.ParseOperator(" ~> ").ShouldEqual(PackageDependency.Operators.SortaGreaterThan);
				PackageDependency.ParseOperator("~<"  ).ShouldEqual(PackageDependency.Operators.SortaLessThan);
				PackageDependency.ParseOperator("<~"  ).ShouldEqual(PackageDependency.Operators.SortaLessThan);
				PackageDependency.ParseOperator(" ~< ").ShouldEqual(PackageDependency.Operators.SortaLessThan);

				PackageDependency.GetOperatorString(PackageDependency.Operators.EqualTo             ).ShouldEqual("=");
				PackageDependency.GetOperatorString(PackageDependency.Operators.GreaterThan         ).ShouldEqual(">");
				PackageDependency.GetOperatorString(PackageDependency.Operators.GreaterThanOrEqualTo).ShouldEqual(">=");
				PackageDependency.GetOperatorString(PackageDependency.Operators.SortaGreaterThan    ).ShouldEqual("~>");
				PackageDependency.GetOperatorString(PackageDependency.Operators.LessThan            ).ShouldEqual("<");
				PackageDependency.GetOperatorString(PackageDependency.Operators.LessThanOrEqualTo   ).ShouldEqual("<=");
				PackageDependency.GetOperatorString(PackageDependency.Operators.SortaLessThan       ).ShouldEqual("<~");
			}

			[Test]
			public void specific_version() {
				Dep("1.2"    ).ToString().ShouldEqual("= 1.2");
				Dep("= 1.1.0").ToString().ShouldEqual("= 1.1.0");
				Dep("=1.2"   ).ToString().ShouldEqual("= 1.2");
				Dep("==0.9"  ).ToString().ShouldEqual("= 0.9");

				new PackageDependency { PackageId = "MyPackage", VersionText = "9.0.123" }.ToString().ShouldEqual("MyPackage = 9.0.123");

				Dep("= 1.2.3.4").Versions[0].Version.ToString().ShouldEqual("1.2.3.4");
				Dep("= 1.2.3.4").Versions[0].Operator.ShouldEqual(PackageDependency.Operators.EqualTo);
			}

			[Test]
			public void just_minversion() {
				Dep(">= 1.2"   ).ToString().ShouldEqual(">= 1.2");
				Dep(">=0.5.0.1").ToString().ShouldEqual(">= 0.5.0.1");
				Dep("=>1.1"    ).ToString().ShouldEqual(">= 1.1");

				Dep(">= 1.2.3.4").Versions[0].Version.ToString().ShouldEqual("1.2.3.4");
				Dep(">= 1.2.3.4").Versions[0].Operator.ShouldEqual(PackageDependency.Operators.GreaterThanOrEqualTo);
			}

			// "greater than or equal to 1.2.3 but not 1.3 or higher"
			[Test]
			public void tilde_minversion() {
				Dep("~> 1.2").ToString().ShouldEqual("~> 1.2");
				Dep("~>1.5" ).ToString().ShouldEqual("~> 1.5");
				Dep(">~1.9" ).ToString().ShouldEqual("~> 1.9");
			}

			[Test]
			public void just_maxversion() {
				Dep("<= 1.2"   ).ToString().ShouldEqual("<= 1.2");
				Dep("<=0.5.0.1").ToString().ShouldEqual("<= 0.5.0.1");
				Dep("=<1.1"    ).ToString().ShouldEqual("<= 1.1");

				Dep("<= 1.2.3.4").Versions[0].Version.ToString().ShouldEqual("1.2.3.4");
				Dep("<= 1.2.3.4").Versions[0].Operator.ShouldEqual(PackageDependency.Operators.LessThanOrEqualTo);
			}

			[Test]
			public void min_and_maxversion() {
				Dep(">=1.0 <1.9.0").ToString().ShouldEqual(">= 1.0 < 1.9.0");

				Dep(">=1.0 <1.9.0").Versions[0].ToString().ShouldEqual(">= 1.0");
				Dep(">=1.0 <1.9.0").Versions[1].ToString().ShouldEqual("< 1.9.0");
			}
		}

		[TestFixture]
		public class matching_against_package_versions : PackageDependencySpec {

			[Test]
			public void specific_version() {
				Dep("= 1.2.0").Matches("1.2.0" ).ShouldBeTrue();
				Dep("= 1.2.0").Matches("1.2"   ).ShouldBeFalse();
				Dep("= 1.2.0").Matches("0.2.0" ).ShouldBeFalse();
			}

			[Test]
			public void just_minversion() {
				Dep("<= 1.0.1").Matches("0.9"  ).ShouldBeTrue();
				Dep("<= 1.0.1").Matches("1.0"  ).ShouldBeTrue();
				Dep("<= 1.0.1").Matches("1.0.0").ShouldBeTrue();
				Dep("<= 1.0.1").Matches("1.0.1").ShouldBeTrue();
				Dep("<= 1.0.1").Matches("1.0.2").ShouldBeFalse();
				Dep("<= 1.0.1").Matches("2"    ).ShouldBeFalse();
			}
			
			[Test]
			public void less_than() {
				Dep("< 1.0.1").Matches("0.9"  ).ShouldBeTrue();
				Dep("< 1.0.1").Matches("1.0"  ).ShouldBeTrue();
				Dep("< 1.0.1").Matches("1.0.0").ShouldBeTrue();
				Dep("< 1.0.1").Matches("1.0.1").ShouldBeFalse();
				Dep("< 1.0.1").Matches("1.0.2").ShouldBeFalse();
				Dep("< 1.0.1").Matches("2"    ).ShouldBeFalse();
			}

			[Test]
			public void sorta_less_than() {
				Dep("<~ 0.9"  ).Matches("1.0.1").ShouldBeFalse();
				Dep("<~ 1.0"  ).Matches("1.0.1").ShouldBeFalse();
				Dep("<~ 1.0.0").Matches("1.0.1").ShouldBeFalse();
				Dep("<~ 1.0.1").Matches("1.0.1").ShouldBeTrue();
				Dep("<~ 1.0.2").Matches("1.0.1").ShouldBeTrue();
				Dep("<~ 1.0.9").Matches("1.0.1").ShouldBeTrue();
				Dep("<~ 1.1"  ).Matches("1.0.1").ShouldBeFalse(); // major/minor version numbers can't be greater
				Dep("<~ 2"    ).Matches("1.0.1").ShouldBeFalse();
			}

			[Test]
			public void just_maxversion() {
				Dep(">= 1.0.1").Matches("0.9"  ).ShouldBeFalse();
				Dep(">= 1.0.1").Matches("1.0"  ).ShouldBeFalse();
				Dep(">= 1.0.1").Matches("1.0.0").ShouldBeFalse();
				Dep(">= 1.0.1").Matches("1.0.1").ShouldBeTrue();
				Dep(">= 1.0.1").Matches("1.0.2").ShouldBeTrue();
				Dep(">= 1.0.1").Matches("2"    ).ShouldBeTrue();
			}

			[Test]
			public void greater_than() {
				Dep("> 1.0.1").Matches("0.9"  ).ShouldBeFalse();
				Dep("> 1.0.1").Matches("1.0"  ).ShouldBeFalse();
				Dep("> 1.0.1").Matches("1.0.0").ShouldBeFalse();
				Dep("> 1.0.1").Matches("1.0.1").ShouldBeFalse();
				Dep("> 1.0.1").Matches("1.0.2").ShouldBeTrue();
				Dep("> 1.0.1").Matches("2"    ).ShouldBeTrue();
			}

			[Test]
			public void sorta_greater_than() {
				Dep("~> 1.0.1").Matches("0.9"  ).ShouldBeFalse();
				Dep("~> 1.0.1").Matches("1.0"  ).ShouldBeFalse();
				Dep("~> 1.0.1").Matches("1.0.0").ShouldBeFalse();
				Dep("~> 1.0.1").Matches("1.0.1").ShouldBeTrue();
				Dep("~> 1.0.1").Matches("1.0.2").ShouldBeTrue();
				Dep("~> 1.0.1").Matches("1.0.9").ShouldBeTrue();
				Dep("~> 1.0.1").Matches("1.1"  ).ShouldBeFalse(); // major/minor version numbers can't be greater
				Dep("~> 1.0.1").Matches("2"    ).ShouldBeFalse();
			}

			[Test]
			public void min_and_maxversion() {
				Dep("> 1.0   < 2.0").Matches("1.0").ShouldBeFalse();
				Dep("> 1.0   < 2.0").Matches("1.0.0").ShouldBeTrue();
				Dep("> 1.0   < 2.0").Matches("1.0.1").ShouldBeTrue();
				Dep("> 1.0   < 2.0").Matches("1.1").ShouldBeTrue();
				Dep("> 1.0   < 2.0").Matches("1.9").ShouldBeTrue();
				Dep("> 1.0   < 2.0").Matches("2.0").ShouldBeFalse();
				Dep("> 1.0   < 2.0").Matches("2.0.0.0").ShouldBeFalse();
			}

			[Test]
			public void matching_against_multiple_dependencies() {
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.5")).ShouldBeTrue();
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.6")).ShouldBeFalse();
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.5"), Dep("< 2.0")).ShouldBeTrue();
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.4"), Dep("< 1.5")).ShouldBeFalse();
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.4"), Dep("= 1.5")).ShouldBeFalse();
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.4"), Dep("= 1.5.0")).ShouldBeTrue();
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.4"), Dep("< 1.6"), Dep("~> 1.5")).ShouldBeTrue();
				PackageDependency.MatchesAll("1.5.0", Dep(">= 1.4"), Dep("< 1.7"), Dep("~> 1.5")).ShouldBeTrue();
				PackageDependency.MatchesAll("1.6.0", Dep(">= 1.4"), Dep("< 1.7"), Dep("~> 1.5")).ShouldBeFalse();
			}
		}

		[TestFixture]
		public class helper_methods_for_setting_versions {

			[Test]
			public void Version_sets_exact_version() {
				var dependency = new PackageDependency { VersionText = "1.2.1" };

				dependency.VersionText.ShouldEqual("1.2.1");
				dependency.Version.ToString().ShouldEqual("1.2.1");

				dependency.Versions.Count.ShouldEqual(1);
				dependency.Versions[0].Version.ToString().ShouldEqual("1.2.1");
				dependency.Versions[0].Operator.ShouldEqual(PackageDependency.Operators.EqualTo);
			}

			[Test]
			public void MinVersion_sets_greater_than_or_equal_to() {
				var dependency = new PackageDependency { MinVersionText = "1.2.1" };

				dependency.MinVersionText.ShouldEqual("1.2.1");
				dependency.MinVersion.ToString().ShouldEqual("1.2.1");

				dependency.Versions.Count.ShouldEqual(1);
				dependency.Versions[0].Version.ToString().ShouldEqual("1.2.1");
				dependency.Versions[0].Operator.ShouldEqual(PackageDependency.Operators.GreaterThanOrEqualTo);
			}

			[Test]
			public void MaxVersion_sets_less_than_or_equal_to() {
				var dependency = new PackageDependency { MaxVersionText = "1.2.1" };

				dependency.MaxVersionText.ShouldEqual("1.2.1");
				dependency.MaxVersion.ToString().ShouldEqual("1.2.1");

				dependency.Versions.Count.ShouldEqual(1);
				dependency.Versions[0].Version.ToString().ShouldEqual("1.2.1");
				dependency.Versions[0].Operator.ShouldEqual(PackageDependency.Operators.LessThanOrEqualTo);
			}

			[Test]
			public void SortaMinVersion_sets_sorta_greater_than() {
				var dependency = new PackageDependency { SortaMinVersionText = "1.2.1" };

				dependency.SortaMinVersionText.ShouldEqual("1.2.1");
				dependency.SortaMinVersion.ToString().ShouldEqual("1.2.1");

				dependency.Versions.Count.ShouldEqual(1);
				dependency.Versions[0].Version.ToString().ShouldEqual("1.2.1");
				dependency.Versions[0].Operator.ShouldEqual(PackageDependency.Operators.SortaGreaterThan);
			}
		}
	}
}
