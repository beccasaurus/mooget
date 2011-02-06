using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class FeedSpec : MooGetSpec {

		// Atom formatted datetimes do not have milliseconds ... this gives us a date time like we would get after 
		// parsing it from an Atom feed
		DateTime ParseDateWithFeedAccuracy(string accurateFromNuspec) {
			return DateTime.Parse(Feed.Format(DateTime.Parse(accurateFromNuspec)));
		}

		[TestFixture]
		public class Parsing : FeedSpec {

			[Test][Ignore]
			public void can_parse_a_feeds_xml_into_a_list_of_packages() {
			}

		}

		[TestFixture]
		public class Generating : FeedSpec {

			[Test][Ignore]
			public void generated_feed_can_be_parsed_as_a_valid_Atom_feed() {
			}

			[Test][Ignore]
			public void the_feed_should_have_an_Atom_tag_id() {
			}

			[Test]
			public void each_entry_has_a_unique_Atom_tag_id_that_is_always_the_same() {
				var package = Package.FromSpec(PathToContent("some_nuspecs", "Ninject-2.0.1.0.nuspec"));
				Feed.IdForPackage(package, "foo.com").ShouldEqual("tag:foo.com,2010-10-25:Ninject-2.0.1.0.nupkg");

				package = Package.FromSpec(PathToContent("my_nuspecs", "HasTags.nuspec"));
				Feed.IdForPackage(package, "bar.org").ShouldEqual("tag:bar.org,2011-01-06:i-have-tags-1.0.2.5.nupkg");
			}

			[Test]
			public void can_take_0_packages_and_generate_an_empty_xml_feed() {
				var feedXml       = Feed.GenerateFeed(new List<Package>());
				var packagesInXml = Feed.ParseFeed(feedXml);

				packagesInXml.Should(Be.Empty); // it should parse OK (without blowing up) and it should be empty
			}

			[Test]
			public void can_take_1_package_and_generate_xml_for_a_feed() {
				var packages      = new List<Package> { new Package {
					Id            = "MyPackage",
					VersionText = "1.2.3.4",
					Description   = "About My Package",
					Tags          = new List<string> { "Foo", "Bar" }
				}};
				var feedXml       = Feed.GenerateFeed(packages);
				var packagesInXml = Feed.ParseFeed(feedXml); // <--- should this take the XML or a path to a Feed.  path seems more obvious

				packagesInXml.Count.ShouldEqual(1);
				var package = packagesInXml.First();
				package.IdAndVersion.ShouldEqual("MyPackage-1.2.3.4");
				package.Description.ShouldEqual("About My Package");
				package.Tags.Count.ShouldEqual(2);
				package.Tags.ShouldContain("Foo");
				package.Tags.ShouldContain("Bar");
			}

			[Test]
			public void dependency_versions_generate_properly() {
				var package = new Package { Id = "Foo" };
				package.Dependencies.Add(new PackageDependency("AnyVersion"));
				package.Dependencies.Add(new PackageDependency("EqualTo 1.2.3"));
				package.Dependencies.Add(new PackageDependency("GreaterThan > 1.5"));
				package.Dependencies.Add(new PackageDependency("SortaGreater ~>5.0"));
				package.Dependencies.Add(new PackageDependency("GreaterThanOrLess > 1.0 < 2.0.1"));

				var feedXml       = Feed.GenerateFeed(new List<Package> { package });
				var packagesInXml = Feed.ParseFeed(feedXml);

				packagesInXml.Count.ShouldEqual(1);
				var fromFeed = packagesInXml.First();
				fromFeed.Dependencies.Count.ShouldEqual(5);
				fromFeed.Dependencies.Select(d => d.ToString()).ToArray().ShouldContain("AnyVersion");
				fromFeed.Dependencies.Select(d => d.ToString()).ToArray().ShouldContain("EqualTo = 1.2.3");
				fromFeed.Dependencies.Select(d => d.ToString()).ToArray().ShouldContain("GreaterThan > 1.5");
				fromFeed.Dependencies.Select(d => d.ToString()).ToArray().ShouldContain("SortaGreater ~> 5.0");
				fromFeed.Dependencies.Select(d => d.ToString()).ToArray().ShouldContain("GreaterThanOrLess > 1.0 < 2.0.1");
			}

			[Test]
			public void can_take_2_packages_and_generate_xml_for_a_feed() {
				var packages = new List<Package>();
				packages.Add(Package.FromSpec(PathToContent("some_nuspecs", "Castle.Core-1.1.0.nuspec")));
				packages.Add(Package.FromSpec(PathToContent("some_nuspecs", "Castle.Core-2.5.1.nuspec")));

				var feedXml       = Feed.GenerateFeed(packages);
				var packagesInXml = Feed.ParseFeed(feedXml); // <--- should this take the XML or a path to a Feed.  path seems more obvious

				packagesInXml.Count.ShouldEqual(2);
				var first = packagesInXml.First();
				first.IdAndVersion.ShouldEqual("Castle.Core-1.1.0");
				first.Description.ShouldEqual("Core of the castle project");
				first.Authors.Count.ShouldEqual(1);
				first.Authors.First().ShouldEqual("Jonathon Rossi & Krzysztof Kozmic");
				first.Created.ShouldEqual(ParseDateWithFeedAccuracy("2010-10-25T22:55:19.5876+00:00"));
				first.Modified.ShouldEqual(ParseDateWithFeedAccuracy("2010-10-25T22:55:19.5886+00:00"));
				first.Dependencies.Count.ShouldEqual(1);
				first.Dependencies.First().Id.ShouldEqual("log4net");

				var last = packagesInXml.Last();
				last.IdAndVersion.ShouldEqual("Castle.Core-2.5.1");
				last.Description.ShouldEqual("Core of the Castle project");
				last.Authors.Count.ShouldEqual(1);
				last.Authors.First().ShouldEqual("Jonathon Rossi & Krzysztof Kozmic");
				last.Created.ShouldEqual(ParseDateWithFeedAccuracy("2010-10-25T22:55:19.8976+00:00"));
				last.Modified.ShouldEqual(ParseDateWithFeedAccuracy("2010-10-25T22:55:19.8986+00:00"));
				last.Dependencies.Count.ShouldEqual(0);
			}

			[Test][Ignore]
			public void can_take_a_list_of_packages_and_generate_xml_for_a_feed() {
			}

			[Test][Ignore]
			public void can_generate_feed_xml_providing_a_prefix_for_generating_the_download_url() {
			}

			[Test][Ignore]
			public void can_generate_feed_xml_providing_a_lambda_for_generating_the_download_url() {
			}

			[Test][Ignore]
			public void can_use_LocalPackage_objects_to_generate_the_feed_from() {
			}

			// TODO make sure to test all of the possible Atom feed fields ... version, license url, download url, etc ...

			[TestFixture]
			public class CrazyLibraryExamples {

				static string feedXml = Feed.GenerateFeed(new List<Package> { Package.FromSpec(PathToContent("my_nuspecs", "CrazyLibrary.nuspec")) });
				static RemotePackage package = Feed.ParseFeed(feedXml).First();

				[Test]
				public void language_is_included_in_feed() {
					package.Language.ShouldEqual("nl-BE");
				}

				[Test]
				public void licenseUrl_is_included_in_feed() {
					package.LicenseUrl.ShouldEqual("http://www.apache.org/licenses/LICENSE-2.0");
				}

				[Test]
				public void projectUrl_is_included_in_feed() {
					package.ProjectUrl.ShouldEqual("https://github.com/foo/bar");
				}

				[Test]
				public void iconUrl_is_included_in_feed() {
					package.IconUrl.ShouldEqual("http://images.com/someplace/icons/mara.png");
				}

				[Test]
				public void downloadUrl_is_included_in_feed() {
					package.DownloadUrl.ShouldEqual("http://mooget.net/packages/download?p=CrazyLibrary-0.3.6.nupkg");
				}

				[Test]
				public void requireLicenseAcceptance_is_included_in_feed() {
					package.RequireLicenseAcceptance.ShouldBeTrue();
				}

				[Test]
				public void tags_generate_in_feed_properly() {
					package.Tags.Count.ShouldEqual(6);
					package.Tags.ShouldContain("My");
					package.Tags.ShouldContain("totally");
					package.Tags.ShouldContain("awesome");
					package.Tags.ShouldContain("space");
					package.Tags.ShouldContain("delimited");
					package.Tags.ShouldContain("tags");
				}

				[Test]
				public void dependencies_generate_in_feed_properly() {
					package.Dependencies.Count.ShouldEqual(2);
					package.Dependencies.First(d => d.Id == "Ninject").VersionText.ShouldEqual("2.1.0.76");
					package.Dependencies.First(d => d.Id == "WebActivator").MinVersionText.ShouldEqual("1.0.0.0");
					package.Dependencies.First(d => d.Id == "WebActivator").MaxVersionText.ShouldEqual("1.1");
				}

				[Test]
				public void authors_generate_in_feed_properly() {
					package.Authors.Count.ShouldEqual(3);
					package.Authors.ShouldContain("First Guy");
					package.Authors.ShouldContain("Second Guy");
					package.Authors.ShouldContain("Third");
				}

				[Test]
				public void owners_generate_in_feed_properly() {
					package.Owners.Count.ShouldEqual(2);
					package.Owners.ShouldContain("Joe <foo@bar.com>");
					package.Owners.ShouldContain("Sally");
				}
			}
		}
	}
}
