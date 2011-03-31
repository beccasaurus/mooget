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
	public class NuGetODataPackageSpec : Spec {

		Service service;

		[SetUp]
		public void Before() {
			base.BeforeEach();
			FakeResponse(NuGetServiceRoot,                                                       "root.xml");
			FakeResponse(NuGetServiceRoot + "$metadata",                                         "metadata.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$top=1",                                   "Packages_top_1.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$top=3",                                   "Packages_top_3.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$top=3&$skip=2",                           "Packages_top_3_skip_2.xml");
			FakeResponse(NuGetServiceRoot + "Packages(Id='NUnit',Version='2.5.7.10213')",        "Packages_NUnit.xml");
			FakeResponse(NuGetServiceRoot + "Packages",                                          "Packages.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='combres.mvc','2.2.1.2'",       "Packages_page_2.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='ImpromptuInterface','1.2.1'",  "Packages_page_3.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='MvcContrib.WatiN','2.0.96.0'", "Packages_page_4.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='postal','0.2.0'",              "Packages_page_5.xml");
			FakeResponse(NuGetServiceRoot + "Packages?$skiptoken='StructureMap-MVC3','1.0.2'",   "Packages_page_6.xml");

			service = new Service(NuGetServiceRoot);
		}

		[Test]
		public void can_initialize_a_nuget_odata_package_with_an_entity() {
			var entity  = service["Packages"].First;
			var package = new NuGetODataPackage(entity);

			package.ShouldHaveProperties(new {
				Id          = "51Degrees.mobi",
				VersionText = "0.1.11.10"		
			});

			package.Details.ShouldHaveProperties(new {
				Title                     = "51Degrees.mobi",
				Description               = "Detect mobile devices, get really accurate handset properties and redirect to web pages designed for mobile devices. Request.Browser properties will be populated with data from the WURFL open source project.",
				Summary                   = "Detect mobile devices, get really accurate handset properties and redirect to web pages designed for mobile devices. Request.Browser properties will be populated with data from the WURFL open source project.",
				ProjectUrl                = "http://51degrees.codeplex.com/",
				LicenseUrl                = "http://51degrees.codeplex.com/license",
				IconUrl                   = "http://51degrees.mobi/portals/0/NuGetLogo.png",
				RequiresLicenseAcceptance = false,
				AuthorsText               = "James Rosewell,Thomas Holmes",
				OwnersText                = "",
				TagsText                  = ""
			});

			package.Details.Dependencies.Count.ShouldEqual(1);
			package.Details.Dependencies.First().ToString().ShouldEqual("WebActivator >= 1.0.0.0");
		}

		[Test]
		public void parses_dependencies() {
			var entity  = service["Packages"].All.FirstOrDefault(e => e["Id"].ToString() == "combres");
			var package = new NuGetODataPackage(entity);

			package.Details.Dependencies.Count.ShouldEqual(4);
			package.Details.Dependencies.ToStrings().ShouldEqual(new List<string>{
				"fasterflect >= 2.0.1", "log4net >= 1.2.10", "dotless >= 1.1.0", "YUICompressor.NET >= 1.5.0.0"
			});
		}

		[Test]
		public void parses_tags() {
			var entity  = service["Packages"].All.FirstOrDefault(e => e["Id"].ToString() == "combres");
			var package = new NuGetODataPackage(entity);

			package.Details.Tags.ToStrings().ShouldEqual(new List<string>{
				"asp.net", "asp.net-mvc", "azure", "closure", "minification", "yui", "yslow", "performance", "optimization", "speed"
			});
		}

		[Test]
		public void can_get_the_download_url() {
			var entity  = service["Packages"].All.FirstOrDefault(e => e["Id"].ToString() == "combres");
			var package = new NuGetODataPackage(entity);

			package.DownloadUrl.ShouldEqual("http://packages.nuget.org/v1/Package/Download/combres/2.2.1.2");
		}

		[Test]
		public void can_get_arbitrary_metadata() {
			var entity  = service["Packages"].All.FirstOrDefault(e => e["Id"].ToString() == "combres");
			var package = new NuGetODataPackage(entity);

			package.Prop("PackageHash").ShouldEqual("37uLwItEgRY0uKNkziRcgydgLBJ8ubjke09KQWEZ2O7gtUj7PaRIIMwbywRsA+k/5jtjH9sLRkajO9vDW7XU6A==");
		}
	}
}
