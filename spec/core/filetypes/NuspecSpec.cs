using System;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class NuspecSpec : Spec {

		Nuspec minimum;
		Nuspec maximum;

		[SetUp]
		public void Before() {
			minimum = new Nuspec(PathToContent("nuspecs/MeetsMinimumSpecification.nuspec"));
			maximum = new Nuspec(PathToContent("nuspecs/MeetsSpecification.nuspec"));
		}

		[Test][Ignore]
		public void Files() {
		}

		[Test]
		public void Id() {
			minimum.Id.ShouldEqual("Min.Id");
			maximum.Id.ShouldEqual("Package-Id");

			minimum.Id = "New Id";
			minimum.Id.ShouldEqual("New Id");
		}

		[Test]
		public void Version() {
			minimum.VersionString.ShouldEqual("1.2.3.45678");
			minimum.Version.ToString().ShouldEqual("1.2.3.45678");
			maximum.VersionString.ShouldEqual("1.2.3.45678");
			maximum.Version.ToString().ShouldEqual("1.2.3.45678");

			minimum.Version = new PackageVersion("1.2.3");
			minimum.VersionString.ShouldEqual("1.2.3");

			minimum.VersionString = "4.5.6";
			minimum.Version.ToString().ShouldEqual("4.5.6");
		}

		[Test]
		public void Details_Title() {
			minimum.Title.Should(Be.Null);
			maximum.Title.ShouldEqual("Package ID");

			minimum.Title = "Foo";
			minimum.Title.ShouldEqual("Foo");
		}

		[Test]
		public void Details_Description() {
			minimum.Description.ShouldEqual("This is my minimum package");
			maximum.Description.ShouldEqual("Description of Package-Id");

			minimum.Description = "Hi";
			minimum.Description.ShouldEqual("Hi");
		}

		[Test]
		public void Details_Summary() {
			minimum.Summary.Should(Be.Null);
			maximum.Summary.ShouldEqual("Longer summary descripting Package-Id in detail");

			minimum.Summary = "Hi";
			minimum.Summary.ShouldEqual("Hi");
		}

		[Test]
		public void Details_ProjectUrl() {
			minimum.ProjectUrl.Should(Be.Null);
			maximum.ProjectUrl.ShouldEqual("http://github.com/me/project");

			minimum.ProjectUrl = "http://foo.com";
			minimum.ProjectUrl.ShouldEqual("http://foo.com");
		}

		[Test]
		public void Details_IconUrl() {
			minimum.IconUrl.Should(Be.Null);
			maximum.IconUrl.ShouldEqual("http://github.com/me/project/icon.png");

			minimum.IconUrl = "http://foo.com";
			minimum.IconUrl.ShouldEqual("http://foo.com");
		}

		[Test]
		public void Details_LicenseUrl() {
			minimum.LicenseUrl.Should(Be.Null);
			maximum.LicenseUrl.ShouldEqual("http://github.com/me/project/LICENSE.txt");

			minimum.LicenseUrl = "http://foo.com";
			minimum.LicenseUrl.ShouldEqual("http://foo.com");
		}

		[Test]
		public void Details_RequiredLicenseAcceptance() {
			minimum.RequiresLicenseAcceptance.Should(Be.False);
			maximum.RequiresLicenseAcceptance.Should(Be.False);

			minimum.RequiresLicenseAcceptance = true;
			minimum.RequiresLicenseAcceptance.Should(Be.True);
		}

		[Test]
		public void Details_Authors() {
			minimum.Authors.ShouldEqual(new List<string> { "remi", "bob"       });
			minimum.AuthorsText.ShouldEqual("remi,bob");
			maximum.Authors.ShouldEqual(new List<string> { "Lander", "Murdoch" });
			maximum.AuthorsText.ShouldEqual("Lander,Murdoch");

			// Modifying Authors does nothing!  you MUST edit AuthorsText or set Authors = new List<string>
			minimum.Authors.Add("Rover");
			minimum.Authors.ShouldEqual(new List<string>{ "remi", "bob" });
			minimum.AuthorsText.ShouldEqual("remi,bob");

			minimum.AuthorsText = "joe , sue";
			minimum.Authors.ShouldEqual(new List<string>{ "joe", "sue" });
			minimum.AuthorsText.ShouldEqual("joe,sue");

			var authors = minimum.Authors;
			authors.Add("foo");
			minimum.Authors = authors;
			minimum.Authors.ShouldEqual(new List<string>{ "joe", "sue", "foo" });
			minimum.AuthorsText.ShouldEqual("joe,sue,foo");
		}

		[Test]
		public void Details_Owners() {
			minimum.Owners.Should(Be.Empty);
			minimum.OwnersText.Should(Be.Empty);
			maximum.Owners.ShouldEqual(new List<string> { "remi", "wanda" });
			maximum.OwnersText.ShouldEqual("remi,wanda");

			minimum.OwnersText = "joe , sue";
			minimum.Owners.ShouldEqual(new List<string>{ "joe", "sue" });
			minimum.OwnersText.ShouldEqual("joe,sue");

			// Modifying Owners does nothing!  you MUST edit OwnersText or set Owners = new List<string>
			minimum.Owners.Add("Rover");
			minimum.Owners.ShouldEqual(new List<string>{ "joe", "sue" });
			minimum.OwnersText.ShouldEqual("joe,sue");

			var owners = minimum.Owners;
			owners.Add("Rover");
			minimum.Owners = owners;
			minimum.Owners.ShouldEqual(new List<string>{ "joe", "sue", "Rover" });
			minimum.OwnersText.ShouldEqual("joe,sue,Rover");
		}

		[Test][Ignore]
		public void Details_Language() {
		}

		[Test][Ignore]
		public void Details_Tags() {
		}

		[Test][Ignore]
		public void Details_Dependencies() {
		}
	}
}
