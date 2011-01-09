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

		Package PackageWithDependency(string dependencyId, string dependencyVersion) {
			var package = new Package { Id = "MyPackage" };
			package.Dependencies.Add(new PackageDependency { PackageId = dependencyId, VersionString = dependencyVersion });
			return package;
		}

		[TearDown]
		public void Before() {
			Feed.Domain = "mooget.net"; // reset to default
		}

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
		public void can_return_latest_matching_dependencies_from_2_packages_with_conflicting_dependencies() {
		}

		[Test]
		public void can_return_latest_matching_dependencies_from_2_sources() {
			// source1 has versions 1.0, 1.5.0, 1.5.2, and 2.0 of FooBar
			var source1Packages = new List<Package>();
			new List<string> {
				"1.0", "1.5.0", "1.5.2", "2.0"
			}.ForEach(version => source1Packages.Add(new Package { Id = "FooBar", VersionString = version }));
			Feed.Domain = "source1.com";
			var source1 = Source.FromXml(Feed.GenerateFeed(source1Packages));

			// source2 has versions 1.5.1, 1.5.2, 1.5.3, 1.6, 1.9.0, 2.0, and 2.0.1 of FooBar
			var source2Packages = new List<Package>();
			new List<string> {
				"1.5.1", "1.5.2", "1.5.3", "1.6", "1.9.0", "2.0", "2.0.1"
			}.ForEach(version => source2Packages.Add(new Package { Id = "FooBar", VersionString = version }));
			Feed.Domain = "source2.net";
			var source2 = Source.FromXml(Feed.GenerateFeed(source2Packages));

			// let's try getting FooBar = 1.6
			var found = PackageWithDependency("FooBar",  "= 1.6").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.6.nupkg");

			// let's try getting FooBar ~> 1.5.0
			found = PackageWithDependency("FooBar",  "~> 1.5.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.5.3.nupkg");

			// let's try getting FooBar > 1.5.2
			found = PackageWithDependency("FooBar",  "> 1.5.2").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-2.0.1.nupkg");

			// let's try getting FooBar >= 1.5.2
			found = PackageWithDependency("FooBar",  ">= 1.5.2").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-2.0.1.nupkg");

			// let's try getting FooBar > 1.5.9 < 2.0
			found = PackageWithDependency("FooBar",  "> 1.5.9 < 2.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.9.0.nupkg");

			// let's try getting FooBar < 2.0
			found = PackageWithDependency("FooBar", "< 2.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.9.0.nupkg");

			// let's try getting FooBar <= 2.0
			found = PackageWithDependency("FooBar", "<= 2.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source1.com/packages/download?p=FooBar-2.0.nupkg");

			// let's try <= 2.0 but reverse the order that we provide the sources, so source2 gets priority
			found = PackageWithDependency("FooBar", "<= 2.0").FindDependencies(source2.AllPackages, source1.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-2.0.nupkg");
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
	}
}
