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

		Package PackageWithDependencies(params string[] dependencyStrings) {
			var package = new Package { Id = "MyPackage", VersionString = "1.0" };
			foreach (var dependencyString in dependencyStrings)
				package.Dependencies.Add(new PackageDependency(dependencyString));
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
			Console.WriteLine("LOOKING FOR DEPENDENCIES FOR Castle.Core");
			var found = castleCore.FindDependencies(morePackages);
			Console.WriteLine("DONE LOOKING ... WE THINK THERE SHOULD BE 5");
			Console.WriteLine("FOUND: {0}", string.Join(", ", found.Select(pkg => pkg.IdAndVersion).ToArray()));
			found.Count.ShouldEqual(5);
			found.Select(p => p.IdAndVersion).ShouldContain("log4net-1.2.10");
			found.Select(p => p.IdAndVersion).ShouldContain("Iesi.Collections-1.0.1");
			found.Select(p => p.IdAndVersion).ShouldContain("Antlr-3.1.1");
			found.Select(p => p.IdAndVersion).ShouldContain("Castle.DynamicProxy-2.1.0");
			found.Select(p => p.IdAndVersion).ShouldContain("Castle.Core-1.1.0");
		}

		[Test][Ignore]
		public void can_return_latest_matching_dependencies_from_2_packages() {
		}

		[Test][Ignore]
		public void exception_is_raised_if_conflicting_dependencies_are_found() {
		}

		[Test][Ignore]
		public void exception_is_raised_if_not_all_dependencies_are_found() {
		}

		[Test][Ignore]
		public void can_return_dependencies_for_package_that_has_dependencies_which_have_subdependencies_which_have_subdependencies() {
		}

		[Test]
		public void can_return_latest_matching_dependencies_from_2_sources_with_a_package_that_has_overlapping_dependencies() {
			// our package depends on HelloWorld and FooBar ~> 1.5.
			var myPackage  = PackageWithDependencies("HelloWorld", "FooBar ~> 1.5");

			// HelloWorld depends on FooBar
			var helloWorld = new Package { Id = "HelloWorld", VersionString = "1.0" };
			helloWorld.Dependencies.Add(new PackageDependency("FooBar"));

			// source1 has versions 1.0, 1.5.0, 1.5.2, and 2.0 of FooBar and it has HelloWorld, which also depends on FooBar
			var source1Packages = new List<Package>();
			new List<string> {
				"1.0", "1.5.0", "1.5.2", "2.0"
			}.ForEach(version => source1Packages.Add(new Package { Id = "FooBar", VersionString = version }));
			source1Packages.Add(helloWorld);
			Feed.Domain = "source1.com";
			var source1 = Source.FromXml(Feed.GenerateFeed(source1Packages));

			// source2 has versions 1.5.1, 1.5.2, 1.5.3, 1.6, 1.9.0, 2.0, and 2.0.1 of FooBar
			var source2Packages = new List<Package>();
			new List<string> {
				"1.5.1", "1.5.2", "1.5.3", "1.6", "1.9.0", "2.0", "2.0.1"
			}.ForEach(version => source2Packages.Add(new Package { Id = "FooBar", VersionString = version }));
			Feed.Domain = "source2.net";
			var source2 = Source.FromXml(Feed.GenerateFeed(source2Packages));

			// By itself, HelloWorld would find the *latest* version of FooBar to use
			var found = helloWorld.FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-2.0.1.nupkg");

			// When combined with our package, the latest ~> 1.5 version of FooBar is used (to meet ALL dependencies)
			found = Package.FindDependencies(new Package[] { helloWorld, myPackage }, source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.5.3.nupkg");

			// Just to be sure, if we re-arrange the order of the packages, it should give us the same results
			found = Package.FindDependencies(new Package[] { myPackage, helloWorld }, source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.5.3.nupkg");

			// If there are duplicate packages, we end up with the same results
			found = Package.FindDependencies(new Package[] { myPackage, helloWorld, myPackage, helloWorld }, source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.5.3.nupkg");
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
			var found = PackageWithDependencies("FooBar = 1.6").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.6.nupkg");

			// if no version is specified, get the latest
			found = PackageWithDependencies("FooBar").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-2.0.1.nupkg");

			// let's try getting FooBar ~> 1.5.0
			found = PackageWithDependencies("FooBar ~> 1.5.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.5.3.nupkg");

			// let's try getting FooBar > 1.5.2
			found = PackageWithDependencies("FooBar > 1.5.2").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-2.0.1.nupkg");

			// let's try getting FooBar >= 1.5.2
			found = PackageWithDependencies("FooBar >= 1.5.2").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-2.0.1.nupkg");

			// let's try getting FooBar > 1.5.9 < 2.0
			found = PackageWithDependencies("FooBar > 1.5.9 < 2.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.9.0.nupkg");

			// let's try getting FooBar < 2.0
			found = PackageWithDependencies("FooBar < 2.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source2.net/packages/download?p=FooBar-1.9.0.nupkg");

			// let's try getting FooBar <= 2.0
			found = PackageWithDependencies("FooBar <= 2.0").FindDependencies(source1.AllPackages, source2.AllPackages);
			found.Count.ShouldEqual(1);
			found.First().DownloadUrl.ShouldEqual("http://source1.com/packages/download?p=FooBar-2.0.nupkg");

			// let's try <= 2.0 but reverse the order that we provide the sources, so source2 gets priority
			found = PackageWithDependencies("FooBar <= 2.0").FindDependencies(source2.AllPackages, source1.AllPackages);
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
