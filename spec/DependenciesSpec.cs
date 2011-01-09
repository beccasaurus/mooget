using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class DependenciesSpec : MooGetSpec {

		static List<LocalPackage>  packages     = LocalPackage.FromDirectory(PathToContent("unpacked_packages"));
		static List<RemotePackage> morePackages = new Source(PathToContent("example-feed.xml")).AllPackages;

		[Test]
		public void can_return_dependencies_for_a_package_with_no_dependencies() {
			var antiXss = packages.First(pkg => pkg.Id == "AntiXSS");

			// this package's defined dependencies
			antiXss.Dependencies.Should(Be.Empty);

			// all dependencies for this package (requires access to other packages to figure this out)
			// this returns RemotePackages that we can install.
			antiXss.FindDependencies(morePackages).Should(Be.Empty);
		}

		[Test]
		public void can_return_dependencies_for_a_package_with_1_dependency_with_no_subdependencies() {
			var castleCore = packages.First(pkg => pkg.IdAndVersion == "Castle.Core-1.1.0");

			// this package's defined dependencies
			castleCore.Dependencies.Count.ShouldEqual(1);
			castleCore.Dependencies.First().Id.ShouldEqual("log4net");

			// all dependencies for this package (requires access to other packages to figure this out)
			var found = castleCore.FindDependencies(morePackages);
			found.Count.ShouldEqual(1);
			found.First().IdAndVersion.ShouldEqual("log4net-1.2.10");
		}

		[Test]
		public void can_return_dependencies_for_a_package_with_1_dependency_with_subdependencies_that_have_subdependencies() {
			var castleCore = morePackages.First(pkg => pkg.IdAndVersion == "NHibernate.Core-2.1.2.4000");

			// this package's defined dependencies
			castleCore.Dependencies.Count.ShouldEqual(4);
			castleCore.Dependencies.Select(d => d.ToString()).ShouldContain("log4net = 1.2.10");
			castleCore.Dependencies.Select(d => d.ToString()).ShouldContain("Iesi.Collections = 1.0.1");
			castleCore.Dependencies.Select(d => d.ToString()).ShouldContain("Antlr = 3.1.1");
			castleCore.Dependencies.Select(d => d.ToString()).ShouldContain("Castle.DynamicProxy = 2.1.0");

			// all dependencies for this package (requires access to other packages to figure this out)
			var found = castleCore.FindDependencies(morePackages);
			found.Count.ShouldEqual(5);
			found.Select(p => p.IdAndVersion).ShouldContain("log4net-1.2.10");
			found.Select(p => p.IdAndVersion).ShouldContain("Iesi.Collections-1.0.1");
			found.Select(p => p.IdAndVersion).ShouldContain("Antlr-3.1.1");
			found.Select(p => p.IdAndVersion).ShouldContain("Castle.DynamicProxy-2.1.0");
			found.Select(p => p.IdAndVersion).ShouldContain("Castle.Core-1.1.0");
		}

		[Test][Ignore]
		public void can_return_ALL_possible_packages_that_could_be_used_to_resolve_dependencies() {
		}

		[Test][Ignore]
		public void an_exception_with_information_about_ALL_dependent_packages_that_could_not_be_found() {
		}

		[Test][Ignore]
		public void can_return_dependencies_for_a_package_with_no_2_dependencies_with_no_subdependencies() {
		}

		[Test][Ignore]
		public void can_return_dependencies_for_a_package_with_no_1_dependency_with_subdependencies() {
		}

		[Test][Ignore]
		public void can_return_dependencies_for_a_package_with_no_1_dependency_with_subdependencies_and_one_without() {
		}

		[Test][Ignore]
		public void can_return_dependencies_from_1_source() {
		}

		[Test][Ignore]
		public void can_return_dependencies_from_2_sources() {
		}
	}
}
