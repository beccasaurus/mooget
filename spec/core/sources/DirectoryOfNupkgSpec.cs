using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	/// <summary>Testing the core implementation of DirectoryOfNupkg.  1 [Test] for each ISource method (to start with)</summary>
	[TestFixture]
	public class DirectoryOfNupkgSpec : Spec {

		DirectoryOfNupkg dir;

		[SetUp]
		public void Before() {
			base.BeforeEach();
			Directory.CreateDirectory(PathToTemp("nupkgs"));
			dir = new DirectoryOfNupkg(PathToTemp("nupkgs"));
		}

		[Test]
		public void can_have_a_Name() {
			new DirectoryOfNupkg { Name = "MySource" }.Name.ShouldEqual("MySource");
		}

		[Test]
		public void Path() {
			new DirectoryOfNupkg("/").Path.ShouldEqual("/");
			new DirectoryOfNupkg { Path = "/" }.Path.ShouldEqual("/");
		}

		[Test]
		public void can_have_AuthData() {
			new DirectoryOfNupkg { AuthData = 5 }.AuthData.ShouldEqual(5);
		}

		[Test]
		public void Get() {
			dir.Get(new PackageDependency("MarkdownSharp")).Should(Be.Null);
			dir.Get(new PackageDependency("NUnit")).Should(Be.Null);

			dir.Push(new Nupkg(PathToContent("packages/NUnit.2.5.7.10213.nupkg")));

			dir.Get(new PackageDependency("MarkdownSharp")).Should(Be.Null);
			var nunit = dir.Get(new PackageDependency("NUnit"));
			nunit.ShouldNot(Be.Null);
			nunit.Id.ShouldEqual("NUnit");
			nunit.Version.ToString().ShouldEqual("2.5.7.10213");
		}

		[Test]
		public void Packages() {
			var dir_1 = new DirectoryOfNupkg(PathToContent("packages"));
			dir_1.Packages.Count.ShouldEqual(5);
			dir_1.Packages.Ids().ShouldContainAll("FluentNHibernate", "MarkdownSharp", "NUnit", "Ninject", "MarkdownSharp");

			var dir_2 = new DirectoryOfNupkg(PathToContent("package_working_directories"));
			dir_2.Packages.Count.ShouldEqual(3);
			dir_2.Packages.Ids().ShouldContainAll("just-a-library", "just-a-tool", "library-with-no-dependencies");
		}

		[Test][Ignore]
		public void LatestPackages() {
		}

		/// <summary>This tests an external extension method supporting any List of Package</summary>
		[Test][Ignore]
		public void Packages_Latest() {
		}

		/// <summary>This tests an external extension method supporting any List of Package</summary>
		[Test][Ignore]
		public void Packages_GroupByVersion() {
		}

		[Test][Ignore]
		public void GetPackagesWithId() {
		}

		[Test][Ignore]
		public void GetPackagesWithIdStartingWith() {
		}

		[Test]
		public void GetPackagesMatchingDependency() {
		}

		[Test]
		public void Fetch() {
			dir.Push(new Nupkg(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg")));
			Directory.CreateDirectory(PathToTemp("mydir"));
			var mydir = new DirectoryOfNupkg(PathToTemp("mydir"));

			dir.Packages.Count.ShouldEqual(1);
			mydir.Packages.Count.ShouldEqual(0);

			dir.Fetch(new PackageDependency("NoExist"), mydir.Path).Should(Be.Null);
			mydir.Packages.Count.ShouldEqual(0);

			dir.Fetch(new PackageDependency("just-a-tool"), mydir.Path).Id.ShouldEqual("just-a-tool");
			mydir.Packages.Count.ShouldEqual(1);
			mydir.Packages.First().Id.ShouldEqual("just-a-tool");
			(mydir.Packages.First() as Nupkg).Path.ShouldEqual(System.IO.Path.Combine(PathToTemp("mydir"), "just-a-tool-1.0.0.0.nupkg"));
		}

		[Test]
		public void Push() {
			dir.Packages.Should(Be.Empty);

			var pkg = dir.Push(new Nupkg(PathToContent("packages/MarkdownSharp.1.13.0.0.nupkg")));
			pkg.Should(Be.InstanceOf(typeof(Nupkg)));
			pkg.Id.ShouldEqual("MarkdownSharp");
			(pkg as Nupkg).Path.ShouldEqual(System.IO.Path.Combine(dir.Path, "MarkdownSharp.1.13.0.0.nupkg"));

			dir.Packages.Count.ShouldEqual(1);
			dir.Packages.First().Id.ShouldEqual("MarkdownSharp");

			dir.Push(new Nupkg(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg")));

			dir.Packages.Count.ShouldEqual(2);
			dir.Packages.Ids().ShouldEqual(new List<string>{ "MarkdownSharp", "just-a-tool" });
		}

		[Test]
		public void Yank() {
			dir.Push(new Nupkg(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg")));
			dir.Push(new Nupkg(PathToContent("packages/MarkdownSharp.1.13.0.0.nupkg")));
			dir.Packages.Ids().ShouldEqual(new List<string>{ "MarkdownSharp", "just-a-tool" });

			dir.Yank(new PackageDependency("DontExist")).ShouldBeFalse();
			dir.Packages.Ids().ShouldEqual(new List<string>{ "MarkdownSharp", "just-a-tool" });

			dir.Yank(new PackageDependency("MarkdownSharp")).ShouldBeTrue();
			dir.Packages.Ids().ShouldEqual(new List<string>{ "just-a-tool" });

			dir.Yank(new PackageDependency("just-a-tool")).ShouldBeTrue();
			dir.Packages.Should(Be.Empty);
		}

		[Test][Ignore]
		public void Install() {
		}

		[Test][Ignore]
		public void Uninstall() {
		}
	}
}
