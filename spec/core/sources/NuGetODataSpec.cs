using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using EasyOData;
using Requestoring;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class NuGetODataSpec : Spec {

		NuGetOData source;

		[SetUp]
		public void Before() {
			base.BeforeEach();
			FakeResponse(NuGetServiceRoot,                                                       "root.xml");
			FakeResponse(NuGetServiceRoot + "$metadata",                                         "metadata.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$top=1",                                   "Packages_top_1.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$top=3",                                   "Packages_top_3.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$top=3&$skip=2",                           "Packages_top_3_skip_2.xml");
			FakeResponse(NuGetServiceRoot + "Packages(Id='NUnit',Version='2.5.7.10213')",        "Packages_NUnit.xml");
			FakeResponse(NuGetServiceRoot + "Packages(Id='NUnit',Version='2.5.7.12345')",        "Packages_NUnit_wrongVersion.xml");
			FakeResponse(NuGetServiceRoot + "Packages",                                          "Packages.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='combres.mvc','2.2.1.2'",       "Packages_page_2.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='ImpromptuInterface','1.2.1'",  "Packages_page_3.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='MvcContrib.WatiN','2.0.96.0'", "Packages_page_4.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='postal','0.2.0'",              "Packages_page_5.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='StructureMap-MVC3','1.0.2'",   "Packages_page_6.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$filter=Id%20eq%20%27T4MVC%27",            "Packages_WithId_T4MVC.xml");
			FakeResponse(NuGetServiceRoot + "Packages(Id='T4MVC',Version='2.6.31')",             "Packages_T4MVC_2631.xml");
			FakeResponse(NuGetServiceRoot + "Packages(Id='T4MVC',Version='2.6.45')",             "Packages_T4MVC_2645.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$filter=Id%20eq%20%27DoesntExist%27",      "Packages_WithId_DoesntExist.xml");
			FakeResponse(NuGetServiceRoot + "Packages(Id='DoesntExist',Version='1.0')",          "Packages_DoesntExist_10.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$filter=startswith(Id%2c%20%27Cra%27)%20eq%20true", "Packages_startswith_Cra.xml");

			source = new NuGetOData(NuGetServiceRoot);
		}

		[Test]
		public void can_get_all_packages() {
			var packages = source.Packages;

			packages.Count.ShouldEqual(564);

			packages.First().IdAndVersion().ShouldEqual("51Degrees.mobi-0.1.11.10");
			packages.Last().IdAndVersion().ShouldEqual("YUICompressor.NET-1.5.0.0");
		}

		[Test]
		public void can_get_all_packages_that_start_with_a_string() {
			var packages = source.GetPackagesWithIdStartingWith("Cra");

			packages.Count.ShouldEqual(23);
			packages.ToStrings().ShouldEqual(new List<string>{
				"Crack-0.1.0.0", "CraigsUtilityLibrary-2.1", "CraigsUtilityLibrary-Cisco-2.1", "CraigsUtilityLibrary-Classifier-2.1", "CraigsUtilityLibrary-Compression-2.1", "CraigsUtilityLibrary-DataTypes-2.1", "CraigsUtilityLibrary-Encryption-2.1", "CraigsUtilityLibrary-Environment-2.1", "CraigsUtilityLibrary-Error-2.1", "CraigsUtilityLibrary-Events-2.1", "CraigsUtilityLibrary-Exchange-2.1", "CraigsUtilityLibrary-FileFormats-2.1", "CraigsUtilityLibrary-IO-2.1", "CraigsUtilityLibrary-LDAP-2.1", "CraigsUtilityLibrary-Math-2.1", "CraigsUtilityLibrary-Media-2.1", "CraigsUtilityLibrary-Multithreading-2.1", "CraigsUtilityLibrary-Profiler-2.1", "CraigsUtilityLibrary-Random-2.1", "CraigsUtilityLibrary-Reflection-2.1", "CraigsUtilityLibrary-SQL-2.1", "CraigsUtilityLibrary-Web-2.1", "CraigUtilityLibrary-2.0"
			});
		}

		[Test]
		public void can_get_a_package_with_an_exact_id_and_version() {
			// exists
			source.Get(new PackageDependency("NUnit 2.5.7.10213")).ToString().ShouldEqual("NUnit-2.5.7.10213");

			// wrong version
			source.Get(new PackageDependency("NUnit 2.5.7.12345")).Should(Be.Null);

			// nothing with this Id exists at all
			source.Get(new PackageDependency("DoesntExist 1.0")).Should(Be.Null);
		}

		[Test]
		public void can_get_a_package_matching_a_given_dependency() {
			// Versions of T4MVC: 2.6.30 2.6.31 2.6.32 2.6.40 2.6.41 2.6.42 2.6.43 2.6.44

			// exists
			source.Get(new PackageDependency("T4MVC")).ToString().ShouldEqual("T4MVC-2.6.44");
			source.Get(new PackageDependency("T4MVC 2.6.31")).ToString().ShouldEqual("T4MVC-2.6.31");
			source.Get(new PackageDependency("T4MVC > 2.6 < 2.6.32")).ToString().ShouldEqual("T4MVC-2.6.31");
			source.Get(new PackageDependency("T4MVC < 2.6.41")).ToString().ShouldEqual("T4MVC-2.6.40");
			source.Get(new PackageDependency("T4MVC =< 2.6.41")).ToString().ShouldEqual("T4MVC-2.6.41");
			source.Get(new PackageDependency("T4MVC < 2.6.41 < 2.6.40")).ToString().ShouldEqual("T4MVC-2.6.32");

			// version is out of range
			source.Get(new PackageDependency("T4MVC < 2.6")).Should(Be.Null);
			source.Get(new PackageDependency("T4MVC > 2.6.44")).Should(Be.Null);
			source.Get(new PackageDependency("T4MVC 2.6.45")).Should(Be.Null);

			// nothing with this Id exists at all
			source.Get(new PackageDependency("DoesntExist < 1.5"));
		}

		[Test]
		public void can_get_packages_with_an_id() {
			var packages = source.GetPackagesWithId("T4MVC");

			packages.Count.ShouldEqual(8);
			packages.ToStrings().ShouldEqual(new List<string>{
				"T4MVC-2.6.30", "T4MVC-2.6.31", "T4MVC-2.6.32", "T4MVC-2.6.40", "T4MVC-2.6.41", "T4MVC-2.6.42", "T4MVC-2.6.43", "T4MVC-2.6.44"
			});

			source.GetPackagesWithId("DoesntExist").Should(Be.Empty);
		}

		[Test][Ignore]
		public void can_get_just_the_latest_packages() {
		}
	}
}
