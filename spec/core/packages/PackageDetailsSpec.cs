using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class PackageDetailsSpec {

		IPackage package = new NewPackage { Id = "MyPackage", VersionText = "1.0" };
		PackageDetails details;

		[SetUp]
		public void Before() {
			details = new PackageDetails();
		}

		[Test]
		public void knows_the_package_it_belongs_to() {
			details.Package.Should(Be.Null);
			details.Package = package;
			details.Package.ShouldEqual(package);
		}

		[Test]
		public void Title() {
			details.Title.Should(Be.Null);
			details.Title = "My title";
			details.Title.ShouldEqual("My title");
		}

		[Test]
		public void Authors_and_AuthorsText() {
			details.Authors.Should(Be.Empty);
			details.AuthorsText.Should(Be.Empty);

			details.Authors.Add("John"); // add via List
			details.Authors.ShouldEqual(new List<string>{ "John" });
			details.AuthorsText.ShouldEqual("John");

			details.AuthorsText += ",  Sally <email>   "; // add via Text
			details.Authors.ShouldEqual(new List<string>{ "John", "Sally <email>" });
			details.AuthorsText.ShouldEqual("John,Sally <email>");
		}

		[Test]
		public void Owners_and_OwnersText() {
			details.Owners.Should(Be.Empty);
			details.OwnersText.Should(Be.Empty);

			details.Owners.Add("John"); // add via List
			details.Owners.ShouldEqual(new List<string>{ "John" });
			details.OwnersText.ShouldEqual("John");

			details.OwnersText += ",  Sally <email>   "; // add via Text
			details.Owners.ShouldEqual(new List<string>{ "John", "Sally <email>" });
			details.OwnersText.ShouldEqual("John,Sally <email>");
		}

		[Test]
		public void Description() {
			details.Description.Should(Be.Null);
			details.Description = "My description";
			details.Description.ShouldEqual("My description");
		}

		[Test]
		public void Summary() {
			details.Summary.Should(Be.Null);
			details.Summary = "My summary";
			details.Summary.ShouldEqual("My summary");
		}

		[Test]
		public void Language_and_Locale() {
			details.Language.Should(Be.Null);
			details.Locale.Should(Be.Null);

			details.Language = "en-US";

			details.Language.ShouldEqual("en-US");
			details.Locale.Should(Be.InstanceOf(typeof(System.Globalization.CultureInfo)));
			details.Locale.DisplayName.ShouldEqual("English (United States)");
			details.Locale.ToString().ShouldEqual("en-US");

			details.Locale = new System.Globalization.CultureInfo("en-GB");
			details.Locale.ToString().ShouldEqual("en-GB");
			details.Locale.DisplayName.ShouldEqual("English (United Kingdom)");
			details.Language.ShouldEqual("en-GB");
		}

		[Test]
		public void ProjectUrl() {
			details.ProjectUrl.Should(Be.Null);
			details.ProjectUrl = "http://www.foo.com";
			details.ProjectUrl.ShouldEqual("http://www.foo.com");
		}

		[Test]
		public void IconUrl() {
			details.IconUrl.Should(Be.Null);
			details.IconUrl = "http://www.foo.com/img.png";
			details.IconUrl.ShouldEqual("http://www.foo.com/img.png");
		}

		[Test]
		public void LicenseUrl() {
			details.LicenseUrl.Should(Be.Null);
			details.LicenseUrl = "http://www.foo.com/license.txt";
			details.LicenseUrl.ShouldEqual("http://www.foo.com/license.txt");
		}

		[Test]
		public void RequiresLicenseAcceptance() {
			details.RequiresLicenseAcceptance.ShouldBeFalse();
			details.RequiresLicenseAcceptance = true;
			details.RequiresLicenseAcceptance.ShouldBeTrue();
			details.RequiresLicenseAcceptance = false;
			details.RequiresLicenseAcceptance.ShouldBeFalse();
		}

		[Test]
		public void Dependencies() {
			details.Dependencies.Should(Be.Empty);

			details.Dependencies.Add(new PackageDependency("NUnit 1.0.1"));

			details.Dependencies.Count.ShouldEqual(1);
			details.Dependencies.First().PackageId.ShouldEqual("NUnit");
			details.Dependencies.First().VersionsString.ShouldEqual("= 1.0.1");
		}

		[Test]
		public void Tags() {
			details.Tags.Should(Be.Empty);
			details.TagsText.Should(Be.Empty);

			details.Tags.Add("Cool"); // add via List
			details.Tags.ShouldEqual(new List<string>{ "Cool" });
			details.TagsText.ShouldEqual("Cool");

			details.TagsText += " awesome"; // add via Text
			details.Tags.ShouldEqual(new List<string>{ "Cool", "awesome" });
			details.TagsText.ShouldEqual("Cool awesome");
		}
	}
}
