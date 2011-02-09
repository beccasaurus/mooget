using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	/// <summary>Testing the core implementation of MooDir.  1 [Test] for each ISource method (to start with)</summary>
	[TestFixture]
	public class MooDirSpec : Spec {

		MooDir dir;

		[SetUp]
		public void Before() {
			base.BeforeEach();
			dir = new MooDir(PathToTemp("MyMooDir"));
			dir.Initialize();
		}

		[Test]
		public void can_initialize() {
			var mooDir = new MooDir(PathToTemp("FooBar"));
			PathToTemp("FooBar").AsDir().Exists().Should(Be.False);

			mooDir.Initialize();

			PathToTemp("FooBar"             ).AsDir().Exists().Should(Be.True);
			PathToTemp("FooBar", "packages" ).AsDir().Exists().Should(Be.True); // unpacked packages go here
			PathToTemp("FooBar", "cache"    ).AsDir().Exists().Should(Be.True); // .nupkg's are cached here
			PathToTemp("FooBar", "bin"      ).AsDir().Exists().Should(Be.True); // this should be added to the user's PATH to run tools

			mooDir.PackageDirectory.ShouldEqual(PathToTemp("FooBar", "packages"));
			mooDir.CacheDirectory.ShouldEqual(PathToTemp("FooBar", "cache"));
			mooDir.BinDirectory.ShouldEqual(PathToTemp("FooBar", "bin"));
		}

		[Test]
		public void can_have_a_Name() {
			new MooDir { Name = "MySource" }.Name.ShouldEqual("MySource");
		}

		[Test]
		public void can_set_Path() {
			new MooDir("/").Path.ShouldEqual("/");
			new MooDir { Path = "/" }.Path.ShouldEqual("/");
		}

		[Test]
		public void can_have_AuthData() {
			new MooDir { AuthData = 5 }.AuthData.ShouldEqual(5);
		}

		// [Test][Ignore]
		// public void Get() {
		// 	dir.Get(new PackageDependency("MarkdownSharp")).Should(Be.Null);
		// 	dir.Get(new PackageDependency("NUnit")).Should(Be.Null);

		// 	dir.Push(new Nupkg(PathToContent("packages/NUnit.2.5.7.10213.nupkg")));

		// 	dir.Get(new PackageDependency("MarkdownSharp")).Should(Be.Null);
		// 	var nunit = dir.Get(new PackageDependency("NUnit"));
		// 	nunit.ShouldNot(Be.Null);
		// 	nunit.Id.ShouldEqual("NUnit");
		// 	nunit.Version.ToString().ShouldEqual("2.5.7.10213");
		// }

		[Test]
		public void Packages() {
			dir.Packages.Should(Be.Empty);

			// if we copy an UnpackedPackage into our PackageDirectory, it should show up!
			Path.Combine(dir.PackageDirectory, "MarkdownSharp.1.13.0.0").AsDir().Exists().Should(Be.False);
			new UnpackedPackage(PathToContent("packages/MarkdownSharp.1.13.0.0")).Copy(dir.PackageDirectory);
			Path.Combine(dir.PackageDirectory, "MarkdownSharp.1.13.0.0").AsDir().Exists().Should(Be.True);

			dir.Packages.Count.ShouldEqual(1);

			var package = dir.Packages.First();
			package.Id.ShouldEqual("MarkdownSharp");
			package.Should(Be.InstanceOf(typeof(MooDirPackage)));
			(package as MooDirPackage).Unpacked.Should(Be.InstanceOf(typeof(UnpackedPackage)));
			(package as MooDirPackage).Unpacked.Path.ShouldEqual(Path.Combine(dir.PackageDirectory, "MarkdownSharp.1.13.0.0"));
			(package as MooDirPackage).Nupkg.Should(Be.Null);

			// if we put the cached nupkg where it's supposed to be, we should see it ...
			Path.Combine(dir.CacheDirectory, "MarkdownSharp.1.13.0.0.nupkg").AsFile().Exists().Should(Be.False);
			new Nupkg(PathToContent("packages/MarkdownSharp.1.13.0.0.nupkg")).Copy(dir.CacheDirectory);
			Path.Combine(dir.CacheDirectory, "MarkdownSharp.1.13.0.0.nupkg").AsFile().Exists().Should(Be.True);

			package = dir.Packages.First();
			package.Id.ShouldEqual("MarkdownSharp");
			package.Should(Be.InstanceOf(typeof(MooDirPackage)));
			(package as MooDirPackage).Unpacked.Should(Be.InstanceOf(typeof(UnpackedPackage)));
			(package as MooDirPackage).Unpacked.Path.ShouldEqual(Path.Combine(dir.PackageDirectory, "MarkdownSharp.1.13.0.0"));
			(package as MooDirPackage).Nupkg.Should(Be.InstanceOf(typeof(Nupkg)));
			(package as MooDirPackage).Nupkg.Path.ShouldEqual(Path.Combine(dir.CacheDirectory, "MarkdownSharp.1.13.0.0.nupkg"));
		}

		// [Test][Ignore]
		// public void LatestPackages() {
		// 	morePackages.GetPackagesWithId("NuGet.CommandLine").Select(p => p.Version.ToString()).ToList().Join(", ").
		// 		ShouldEqual("1.0.11220.26, 1.1.2120.134, 1.1.2120.136");
		// 	morePackages.LatestPackages.Where(p => p.Id == "NuGet.CommandLine").Select(p => p.Version.ToString()).ToList().Join(", ").
		// 		ShouldEqual("1.1.2120.136");
		// }

		// /// <summary>This tests an external extension method supporting any List of Package</summary>
		// [Test][Ignore]
		// public void Packages_Latest() {
		// }

		// /// <summary>This tests an external extension method supporting any List of Package</summary>
		// [Test][Ignore]
		// public void Packages_GroupByVersion() {
		// }

		// [Test][Ignore]
		// public void GetPackagesWithId() {
		// 	morePackages.GetPackagesWithId("NUnit").Ids().ShouldEqual(new List<string>{ });
		// 	morePackages.GetPackagesWithId("Antlr").Ids().ShouldEqual(new List<string>{ "Antlr" });
		// 	morePackages.GetPackagesWithId("NuGet.CommandLine").Ids().ShouldEqual(new List<string>{ "NuGet.CommandLine", "NuGet.CommandLine", "NuGet.CommandLine" });
		// }

		// [Test][Ignore]
		// public void GetPackagesWithIdStartingWith() {
		// 	morePackages.GetPackagesWithIdStartingWith("zzz").Ids().ShouldEqual(new List<string>{ });
		// 	morePackages.GetPackagesWithIdStartingWith("A").Ids().ShouldEqual(new List<string>{ "Antlr", "Apache.NMS", "Apache.NMS.ActiveMQ", "AttributeRouting" });
		// 	morePackages.GetPackagesWithIdStartingWith("ap").Ids().ShouldEqual(new List<string>{ "Apache.NMS", "Apache.NMS.ActiveMQ" });
		// }

		// [Test][Ignore]
		// public void GetPackagesMatchingDependencies() {
		// 	morePackages.GetPackagesMatchingDependencies(new PackageDependency("NUnit")).Should(Be.Empty);
		// 	morePackages.GetPackagesMatchingDependencies(new PackageDependency("Antlr")).Count.ShouldEqual(1);
		// 	morePackages.GetPackagesMatchingDependencies(new PackageDependency("NuGet.CommandLine")).Count.ShouldEqual(3);
		// 	morePackages.GetPackagesMatchingDependencies(new PackageDependency("NuGet.CommandLine >= 1.1")).Count.ShouldEqual(2);

		// 	var matches = morePackages.GetPackagesMatchingDependencies(new PackageDependency("NuGet.CommandLine >= 1.1"), new PackageDependency("NuGet.CommandLine < 1.1.2120.135"));
		// 	matches.Count.ShouldEqual(1);
		// 	matches.First().IdAndVersion().ShouldEqual("NuGet.CommandLine-1.1.2120.134");
		// }

		// [Test][Ignore]
		// public void Fetch() {
		// 	dir.Push(new Nupkg(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg")));
		// 	Directory.CreateDirectory(PathToTemp("mydir"));
		// 	var mydir = new MooDir(PathToTemp("mydir"));

		// 	dir.Packages.Count.ShouldEqual(1);
		// 	mydir.Packages.Count.ShouldEqual(0);

		// 	dir.Fetch(new PackageDependency("NoExist"), mydir.Path).Should(Be.Null);
		// 	mydir.Packages.Count.ShouldEqual(0);

		// 	dir.Fetch(new PackageDependency("just-a-tool"), mydir.Path).Id.ShouldEqual("just-a-tool");
		// 	mydir.Packages.Count.ShouldEqual(1);
		// 	mydir.Packages.First().Id.ShouldEqual("just-a-tool");
		// 	(mydir.Packages.First() as Nupkg).Path.ShouldEqual(System.IO.Path.Combine(PathToTemp("mydir"), "just-a-tool-1.0.0.0.nupkg"));
		// }

		[Test]
		public void Push() {
			Assert.Fail("Need to do Zip.ExtractTo() first");

			dir.Packages.Should(Be.Empty);

			var pkg = dir.Push(new Nupkg(PathToContent("packages/MarkdownSharp.1.13.0.0.nupkg")));

			// check the IPackage that we get back
			pkg.Should(Be.InstanceOf(typeof(MooDirPackage)));
			pkg.Id.ShouldEqual("MarkdownSharp");
			pkg.Source.ShouldEqual(dir);
			(pkg as MooDirPackage).Nupkg.Path.ShouldEqual(Path.Combine(dir.CacheDirectory, "MarkdownSharp-1.13.0.0.nupkg"));
			(pkg as MooDirPackage).Unpacked.Path.ShouldEqual(Path.Combine(dir.PackageDirectory, "MarkdownSharp-1.13.0.0"));

			dir.Packages.Count.ShouldEqual(1);
			dir.Packages.First().Id.ShouldEqual("MarkdownSharp");

			/*
			dir.Push(new Nupkg(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg")));

			dir.Packages.Count.ShouldEqual(2);
			dir.Packages.Ids().ShouldEqual(new List<string>{ "MarkdownSharp", "just-a-tool" });

			TODO after installing just-a-tool, files should show up in the BinDirectory !!!!!  a .bat file and a bash script
			*/
		}

		// [Test][Ignore]
		// public void Yank() {
		// 	dir.Push(new Nupkg(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg")));
		// 	dir.Push(new Nupkg(PathToContent("packages/MarkdownSharp.1.13.0.0.nupkg")));
		// 	dir.Packages.Ids().ShouldEqual(new List<string>{ "MarkdownSharp", "just-a-tool" });

		// 	dir.Yank(new PackageDependency("DontExist")).ShouldBeFalse();
		// 	dir.Packages.Ids().ShouldEqual(new List<string>{ "MarkdownSharp", "just-a-tool" });

		// 	dir.Yank(new PackageDependency("MarkdownSharp")).ShouldBeTrue();
		// 	dir.Packages.Ids().ShouldEqual(new List<string>{ "just-a-tool" });

		// 	dir.Yank(new PackageDependency("just-a-tool")).ShouldBeTrue();
		// 	dir.Packages.Should(Be.Empty);
		// }

		// [Test][Ignore]
		// public void Install() {
		// 	Directory.CreateDirectory(PathToTemp("mydir"));
		// 	var mydir = new MooDir(PathToTemp("mydir"));
		// 	mydir.Packages.Should(Be.Empty);

		// 	// if we don't provide any sources, it can't find the package we're talking about ...
		// 	Should.Throw<PackageNotFoundException>("Package not found: FluentNHibernate", () => {
		// 		mydir.Install(new PackageDependency("FluentNHibernate"));
		// 	});

		// 	// we find the package we're talking about, but we're missing one of the dependencies
		// 	Should.Throw<MissingDependencyException>("No packages were found that satisfy these dependencies: Iesi.Collections = 1.0.1, Castle.DynamicProxy = 2.1.0", () => {
		// 		mydir.Install(new PackageDependency("FluentNHibernate"), morePackages);
		// 	});

		// 	mydir.Packages.Should(Be.Empty);

		// 	// check the dependencies that we're going to install (assuming Install() calls FindDependencies)
		// 	var dependencies = Source.FindDependencies(morePackages.Get(new PackageDependency("FluentNHibernate")), morePackages, morePackageDependencies);
		// 	dependencies.Count.ShouldEqual(6);
		// 	dependencies.Select(pkg => pkg.IdAndVersion()).ToList().ShouldContainAll(
		// 		"NHibernate-2.1.2.4000", "log4net-1.2.10", "Iesi.Collections-1.0.1", "Antlr-3.1.1", 
		// 		"Castle.DynamicProxy-2.1.0", "Castle.Core-1.1.0", "log4net-1.2.10"
		// 	);

		// 	// Inspect their sources to see that some come from 1, some come from another
		// 	dependencies.First(d => d.Id == "NHibernate").Source.ShouldEqual(morePackages);
		// 	dependencies.First(d => d.Id == "log4net").Source.ShouldEqual(morePackages);
		// 	dependencies.First(d => d.Id == "Antlr").Source.ShouldEqual(morePackages);
		// 	dependencies.First(d => d.Id == "Castle.Core").Source.ShouldEqual(morePackages);
		// 	dependencies.First(d => d.Id == "Iesi.Collections").Source.ShouldEqual(morePackageDependencies);
		// 	dependencies.First(d => d.Id == "Castle.DynamicProxy").Source.ShouldEqual(morePackageDependencies);

		// 	mydir.Install(new PackageDependency("FluentNHibernate"), morePackages, morePackageDependencies);

		// 	mydir.Packages.Count.ShouldEqual(7);
		// 	mydir.Packages.Select(pkg => pkg.IdAndVersion()).ToList().ShouldContainAll(
		// 		"NHibernate-2.1.2.4000", "log4net-1.2.10", "Iesi.Collections-1.0.1", "Antlr-3.1.1", 
		// 		"Castle.DynamicProxy-2.1.0", "Castle.Core-1.1.0", "FluentNHibernate-1.1.0.694"
		// 	);
		// }

		// [Test][Ignore]
		// public void Uninstall_without_dependencies() {
		// 	Directory.CreateDirectory(PathToTemp("mydir"));
		// 	var mydir = new MooDir(PathToTemp("mydir"));

		// 	mydir.Install(new PackageDependency("FluentNHibernate"), morePackages, morePackageDependencies);
		// 	mydir.Packages.Count.ShouldEqual(7);
		// 	mydir.Packages.Select(pkg => pkg.IdAndVersion()).ToList().ShouldContainAll(
		// 		"NHibernate-2.1.2.4000", "log4net-1.2.10", "Iesi.Collections-1.0.1", "Antlr-3.1.1", 
		// 		"Castle.DynamicProxy-2.1.0", "Castle.Core-1.1.0", "FluentNHibernate-1.1.0.694"
		// 	);

		// 	mydir.Uninstall(new PackageDependency("FluentNHibernate"), false).Should(Be.True);

		// 	mydir.Packages.Count.ShouldEqual(6);
		// 	mydir.Packages.Select(pkg => pkg.IdAndVersion()).ToList().ShouldContainAll(
		// 		"NHibernate-2.1.2.4000", "log4net-1.2.10", "Iesi.Collections-1.0.1", "Antlr-3.1.1", 
		// 		"Castle.DynamicProxy-2.1.0", "Castle.Core-1.1.0"
		// 	);

		// 	mydir.Uninstall(new PackageDependency("FluentNHibernate"), false).Should(Be.False);
		// 	mydir.Packages.Count.ShouldEqual(6);
		// }

		// [Test][Ignore]
		// public void Uninstall_with_dependencies() {
		// 	Directory.CreateDirectory(PathToTemp("mydir"));
		// 	var mydir = new MooDir(PathToTemp("mydir"));

		// 	mydir.Install(new PackageDependency("FluentNHibernate"), morePackages, morePackageDependencies);
		// 	mydir.Packages.Count.ShouldEqual(7);
		// 	mydir.Packages.Select(pkg => pkg.IdAndVersion()).ToList().ShouldContainAll(
		// 		"NHibernate-2.1.2.4000", "log4net-1.2.10", "Iesi.Collections-1.0.1", "Antlr-3.1.1", 
		// 		"Castle.DynamicProxy-2.1.0", "Castle.Core-1.1.0", "FluentNHibernate-1.1.0.694"
		// 	);

		// 	mydir.Uninstall(new PackageDependency("FluentNHibernate"), true).Should(Be.True);

		// 	mydir.Packages.Count.ShouldEqual(0);

		// 	mydir.Uninstall(new PackageDependency("FluentNHibernate"), true).Should(Be.False);
		// 	mydir.Packages.Count.ShouldEqual(0);
		// }
	}
}
