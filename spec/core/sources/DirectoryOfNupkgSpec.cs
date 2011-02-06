using System;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	/// <summary>Testing the core implementation of DirectoryOfNupkg.  1 [Test] for each ISource method (to start with)</summary>
	[TestFixture]
	public class DirectoryOfNupkgSpec : Spec {

		[SetUp]
		public void Before() {

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

		[Test][Ignore]
		public void Get() {
		}

		[Test]
		public void Packages() {
			var dir_1 = new DirectoryOfNupkg(PathToContent("packages"));
			dir_1.Packages.Count.ShouldEqual(5);
			dir_1.Packages.Ids().ShouldContainAll("FluentNHibernate", "MarkdownSharp", "Ninject", "NUnit", "???");

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
		public void Packages_GroupByVersoin() {
		}

		[Test][Ignore]
		public void GetPackagesWithId() {
		}

		[Test][Ignore]
		public void GetPackagesWithIdStartingWith() {
		}

		[Test][Ignore]
		public void GetPackagesMatchingDependency() {
		}

		[Test][Ignore]
		public void Fetch() {
		}

		[Test][Ignore]
		public void Push() {
		}

		[Test][Ignore]
		public void Yank() {
		}

		[Test][Ignore]
		public void Install() {
		}

		[Test][Ignore]
		public void Uninstall() {
		}
	}
}
