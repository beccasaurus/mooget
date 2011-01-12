using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class ReadingNuspecSpec : MooGetSpec {

		[Test][Ignore]
		public void Can_read_Nuspec_from_a_nupkg_without_extracting_the_package() {
		}
	
		[Test]
		public void NUnit_example() {
			var package = Package.FromSpec(PathToContent("packages", "NUnit.2.5.7.10213", "NUnit.nuspec"));

			package.Id.ShouldEqual("NUnit");
			package.VersionString.ShouldEqual("2.5.7.10213");
			package.Description.ShouldEqual("NUnit is a unit-testing framework for all .Net languages. Initially ported from JUnit, the current production release, version 2.5, is the sixth major release of this xUnit based unit testing tool for Microsoft .NET.");
			package.Authors.Count.ShouldEqual(1);
			package.Authors.First().ShouldEqual("Charlie Poole");
			package.Language.ShouldEqual("en-US");
			package.RequireLicenseAcceptance.ShouldBeFalse();
			package.Created.ShouldEqual(DateTime.Parse("2010-10-25T22:55:39.6602+00:00"));
			package.Modified.ShouldEqual(DateTime.Parse("2010-10-25T22:55:39.6602+00:00"));
			package.LicenseUrl.Should(Be.Null);
			package.Dependencies.Should(Be.Empty);
		}

		[Test]
		public void FluentNHibernate_example() {
			var package = Package.FromSpec(PathToContent("packages", "FluentNHibernate.1.1.0.694", "FluentNHibernate.nuspec"));

			package.Id.ShouldEqual("FluentNHibernate");
			package.VersionString.ShouldEqual("1.1.0.694");
			package.Description.ShouldEqual("Fluent, XML-less, compile safe, automated,  convention-based mappings for NHibernate.  Get your fluent on.");
			package.Authors.Count.ShouldEqual(1);
			package.Authors.First().ShouldEqual("James Gregory");
			package.Language.ShouldEqual("en-US");
			package.RequireLicenseAcceptance.ShouldBeFalse();
			package.Created.ShouldEqual(DateTime.Parse("2010-10-25T22:55:28.92+00:00"));
			package.Modified.ShouldEqual(DateTime.Parse("2010-10-25T22:55:28.921+00:00"));
			package.LicenseUrl.ShouldEqual("http://github.com/jagregory/fluent-nhibernate/raw/master/LICENSE.txt");
			package.Dependencies.Count.ShouldEqual(1);
			package.Dependencies.First().Id.ShouldEqual("NHibernate.Core");
			package.Dependencies.First().VersionString.ShouldEqual("2.1.2.4000");
			package.Dependencies.First().MinVersionString.Should(Be.Null);
			package.Dependencies.First().MaxVersionString.Should(Be.Null);
			package.Tags.Should(Be.Empty);
		}

		[Test]
		public void CrazyLibrary_example() {
			var package = Package.FromSpec(PathToContent("my_nuspecs", "CrazyLibrary.nuspec"));

			package.Id.ShouldEqual("CrazyLibrary");
			package.Title.ShouldEqual("Crazy Library");
			package.VersionString.ShouldEqual("0.3.6");
			package.Description.ShouldEqual("A crazy library");
			package.Language.ShouldEqual("nl-BE");
			package.Created.ShouldEqual(DateTime.Parse("2010-10-25T22:55:38.3056+00:00"));
			package.Modified.ShouldEqual(DateTime.Parse("2010-10-25T22:55:38.3066+00:00"));

			package.LicenseUrl.ShouldEqual("http://www.apache.org/licenses/LICENSE-2.0");
			package.ProjectUrl.ShouldEqual("https://github.com/foo/bar");
			package.IconUrl.ShouldEqual("http://images.com/someplace/icons/mara.png");

			package.RequireLicenseAcceptance.ShouldBeTrue();

			package.Authors.Count.ShouldEqual(3);
			package.Authors.ShouldContain("First Guy");
			package.Authors.ShouldContain("Second Guy");
			package.Authors.ShouldContain("Third");

			package.Owners.Count.ShouldEqual(2);
			package.Owners.ShouldContain("Joe <foo@bar.com>");
			package.Owners.ShouldContain("Sally");

			package.Tags.Count.ShouldEqual(6);
			package.Tags.ShouldContain("My");
			package.Tags.ShouldContain("totally");
			package.Tags.ShouldContain("awesome");
			package.Tags.ShouldContain("space");
			package.Tags.ShouldContain("delimited");
			package.Tags.ShouldContain("tags");

			package.Dependencies.Count.ShouldEqual(2);
			package.Dependencies.First(d => d.Id == "Ninject").VersionString.ShouldEqual("2.1.0.76");
			package.Dependencies.First(d => d.Id == "WebActivator").VersionString.Should(Be.Null);
			package.Dependencies.First(d => d.Id == "WebActivator").MinVersionString.ShouldEqual("1.0.0.0");
			package.Dependencies.First(d => d.Id == "WebActivator").MaxVersionString.ShouldEqual("1.1");
		}

		[Test]
		public void can_read_tags() {
			var package = Package.FromSpec(PathToContent("my_nuspecs", "HasTags.nuspec"));

			package.Id.ShouldEqual("i-have-tags");
			package.VersionString.ShouldEqual("1.0.2.5");
			package.Description.ShouldEqual("I have tags!");
			package.Authors.Count.ShouldEqual(1);
			package.Authors.First().ShouldEqual("remi");
			package.Tags.Count.ShouldEqual(4);
			new List<string> { "foo", "bar", "Hello", "World" }.ForEach(tag => package.Tags.ShouldContain(tag));

			// example with commas, incase people comma delimit ...
			package = Package.FromSpec(PathToContent("my_nuspecs", "HasTagsWithCommas.nuspec"));

			package.Id.ShouldEqual("i-have-tags");
			package.VersionString.ShouldEqual("1.0.2.5");
			package.Description.ShouldEqual("I have tags with Commas!");
			package.Authors.Count.ShouldEqual(1);
			package.Authors.First().ShouldEqual("remi");
			package.Tags.Count.ShouldEqual(5);
			new List<string> { "foo", "bar", "Hello", "World", "With COMMAS" }.ForEach(tag => package.Tags.ShouldContain(tag));
		}

		// TODO find or make examples with:
		// summary
		// tags
		// iconUrl
		// projectUrl
		// dependencies
		// files specified
		// owners
		// <author> and <owner> should be able to have an email= attribute?  could be useful for permissions when pushing/etc
	}
}
