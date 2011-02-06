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

		[Test]
		public void Details_Language() {
			minimum.Language.Should(Be.Null);
			minimum.Locale.Should(Be.Null);
			maximum.Language.ShouldEqual("en-US");
			maximum.Locale.DisplayName.ShouldEqual("English (United States)");

			minimum.Language = "en-GB";
			minimum.Language.ShouldEqual("en-GB");
			minimum.Locale.DisplayName.ShouldEqual("English (United Kingdom)");
		}

		[Test]
		public void Details_Tags() {
			minimum.Tags.Should(Be.Empty);
			minimum.TagsText.Should(Be.Empty);
			maximum.Tags.ShouldEqual(new List<string>{ "foo", "bar", "foo.bar" });
			maximum.TagsText.ShouldEqual("foo bar foo.bar");

			minimum.TagsText = "hi there";
			minimum.TagsText.ShouldEqual("hi there");
			minimum.Tags.ShouldEqual(new List<string>{ "hi", "there" });

			minimum.Tags = new List<string>{ "this" };
			minimum.TagsText.ShouldEqual("this");
			minimum.Tags.ShouldEqual(new List<string>{ "this" });
		}

		[Test]
		public void Details_Dependencies() {
			minimum.Dependencies.Should(Be.Empty);
			maximum.Dependencies.Count.ShouldEqual(3);
			maximum.Dependencies.ToStrings().ShouldEqual(new List<string> { "NUnit", "NHibernate.Core = 2.1.2.4000", "FooBar >= 1.0" });

			// Just like Authors, Owners, Tags, and everything else ... you CANNOT modify the Dependencies List object if you 
			// want to change the xml.  Instead you need to assign the whole List via the set {}
			var dependencies = new List<PackageDependency>();
			dependencies.Add(new PackageDependency("Foo"));
			dependencies.Add(new PackageDependency("Bar 1.0"));
			dependencies.Add(new PackageDependency("Neat >= 1.0 < 9.9"));

			// can add dependencies if we didn't have any
			minimum.Dependencies = dependencies;
			minimum.Dependencies.ToStrings().ShouldEqual(new List<string> { "Foo", "Bar = 1.0", "Neat >= 1.0 < 9.9" });

			// can override existing dependencies
			maximum.Dependencies.ToStrings().ShouldEqual(new List<string> { "NUnit", "NHibernate.Core = 2.1.2.4000", "FooBar >= 1.0" });
			maximum.Dependencies = dependencies;
			maximum.Dependencies.ToStrings().ShouldEqual(new List<string> { "Foo", "Bar = 1.0", "Neat >= 1.0 < 9.9" });
		}

		[Test][Ignore]
		public void Files() {
		}
	}
}
