using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class LocalPackageSpec : MooGetSpec {

		List<LocalPackage> packages = LocalPackage.FromDirectory(PathToContent("unpacked_packages"));

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
			ninject.Path.ShouldEqual(PathToContent("unpacked_packages", "Ninject-2.0.1.0"));
			ninject.NuspecPath.ShouldEqual(PathToContent("unpacked_packages", "Ninject-2.0.1.0", "Ninject.nuspec"));
			ninject.Libraries.ShouldEqual(new string[] { PathToContent("unpacked_packages", "Ninject-2.0.1.0", "lib", "Ninject.dll") });
			ninject.Tools.Should(Be.Empty);
			ninject.Content.Should(Be.Empty);

			// getting arbitrary files
			ninject.GetFiles().Length.ShouldEqual(4);
			ninject.GetFiles("lib").Length.ShouldEqual(2);
			ninject.GetFiles("lib", "*.dll").Length.ShouldEqual(1);
			ninject.GetFiles("tools").Length.ShouldEqual(0);
		}

		[Test][Ignore]
		public void can_get_all_exe_tools() {
		}

		[Test][Ignore]
		public void can_get_all_dll_libraries() {
		}

		[Test][Ignore]
		public void can_get_all_dll_libraries_for_a_particular_framework_version() {
		}

		[Test][Ignore]
		public void can_get_all_framework_versions_that_libraries_are_available_for() {
		}

		[Test]
		public void can_get_all_content_files() {
			var routing = packages.First(pkg => pkg.Id == "RestfulRouting");
			routing.Content.Length.ShouldEqual(3);
			routing.Content.Select(file => Path.GetFileName(file)).ShouldContain("Routes.cs.pp");
		}

		[Test]
		public void can_get_all_local_packages_from_many_local_directories() {
			var morePackages = LocalPackage.FromDirectories(PathToContent("unpacked_packages"), PathToContent("packages"));
			morePackages.Count.ShouldEqual(12);
			foreach (var idAndVersion in new string[] { "AntiXSS-4.0.1", "Castle.Core-1.1.0", "Castle.Core-1.2.0", "Castle.Core-2.5.1", 
					 "Ninject-2.0.1.0", "Ninject-2.1.0.76", "RestfulRouting-1.0", "RhinoMocks-3.6",
					 "FluentNHibernate-1.1.0.694", "MarkdownSharp-1.13.0.0", "NUnit-2.5.7.10213" })
				morePackages.Select(pkg => pkg.IdAndVersion).ShouldContain(idAndVersion);
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
