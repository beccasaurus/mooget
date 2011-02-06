using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class NupkgSpec : Spec {

		Nupkg nunit;
		Nupkg fluent;

		[SetUp]
		public void Before() {
			nunit  = new Nupkg(PathToContent("packages/NUnit.2.5.7.10213.nupkg"));
			fluent = new Nupkg(PathToContent("packages/FluentNHibernate.1.1.0.694.nupkg"));
		}

		[Test]
		public void has_a_Path() {
			nunit.Path.ShouldEqual(PathToContent("packages/NUnit.2.5.7.10213.nupkg"));
			fluent.Path.ShouldEqual(PathToContent("packages/FluentNHibernate.1.1.0.694.nupkg"));
		}

		[Test]
		public void can_check_if_Exists() {
			nunit.Exists.ShouldBeTrue();
			fluent.Exists.ShouldBeTrue();
			new Nupkg("/i/dont/exist.nupkg").Exists.ShouldBeFalse();
		}

		[Test]
		public void Nuspec() {
			nunit.Nuspec.ShouldHaveProperties(new {
				Id          = "NUnit",
				VersionText = "2.5.7.10213",
				AuthorsText = "Charlie Poole"
			});

			fluent.Nuspec.ShouldHaveProperties(new {
				Id          = "FluentNHibernate",
				VersionText = "1.1.0.694",
				AuthorsText = "James Gregory"
			});
		}

		[Test]
		public void Id() {
			nunit.Id.ShouldEqual("NUnit");
			fluent.Id.ShouldEqual("FluentNHibernate");
		}

		[Test]
		public void Version() {
			nunit.Version.ToString().ShouldEqual("2.5.7.10213");
			nunit.VersionText.ShouldEqual("2.5.7.10213");
			fluent.Version.ToString().ShouldEqual("1.1.0.694");
			fluent.VersionText.ShouldEqual("1.1.0.694");
		}

		[Test]
		public void Details() {
			// simple delegates to the Nuspec
			nunit.Details.ShouldEqual(nunit.Nuspec);
			fluent.Details.ShouldEqual(fluent.Nuspec);
		}

		[Test]
		public void Files() {
			// simply delegates to Zip.Paths (as a string[])
			nunit.Files.ShouldEqual(nunit.Zip.Paths.ToArray());
			fluent.Files.ShouldEqual(fluent.Zip.Paths.ToArray());
		}

		[Test][Ignore]
		public void Libraries() {
		}

		[Test][Ignore]
		public void just_NET35_Libraries() {
		}

		[Test][Ignore]
		public void just_NET40_Libraries() {
		}

		[Test][Ignore]
		public void Tools() {
		}

		[Test][Ignore]
		public void Content() {
		}

		[Test][Ignore]
		public void SourceFiles() {
		}
	}
}
