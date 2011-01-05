using System;
using System.IO;
using System.Linq;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class LocalPackageSpec : MooGetSpec {

		[Test]
		public void can_get_all_local_packages_from_a_directory_of_unpacked_packages() {
			var packages = LocalPackage.FromDirectory(PathToContent("unpacked_packages"));
			packages.Count.ShouldEqual(8);
			foreach (var idAndVersion in new string[] { "AntiXSS-4.0.1", "Castle.Core-1.1.0", "Castle.Core-1.2.0", "Castle.Core-2.5.1", 
					 "Ninject-2.0.1.0", "Ninject-2.1.0.76", "RestfulRouting-1.0", "RhinoMocks-3.6" })
				packages.Select(pkg => pkg.IdAndVersion).ShouldContain(idAndVersion);

			var ninject = packages.First(pkg => pkg.IdAndVersion == "Ninject-2.0.1.0");

			// check some generic Package properties
			ninject.Id.ShouldEqual("Ninject");
			ninject.VersionString.ShouldEqual("2.0.1.0");

			// check LocalPackage specific properties/methods
			ninject.Path.ShouldEqual("PATH TO THE NINJECT DIRECTORY");
			ninject.NuspecPath.ShouldEqual("PATH TO THE NUSPEC FILE");
			ninject.LibPath.ShouldEqual("PATH TO THE /lib directory of this package");
			ninject.Libraries.ShouldEqual("A LIST OF ALL OF THIS PACKAGE's DLLs in /lib ... or just the most recent .NET version under lib?");

			// check to see if it's installed ... install it ... uninstall it ...
			Assert.Fail("haven't written these specs yet ...");
		}

		[Test][Ignore]
		public void can_get_all_local_packages_from_many_local_directories() {
		}

		[Test][Ignore]
		public void by_default_all_packages_come_from_Moo_Dir() {
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
