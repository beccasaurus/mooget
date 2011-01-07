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

			[Test][Ignore]
			public void each_entry_has_a_unique_Atom_tag_id_that_is_always_the_same() {
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
					VersionString = "1.2.3.4",
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
			public void tags_generate_in_feed_properly() {
			}

			[Test][Ignore]
			public void dependencies_generate_in_feed_properly() {
			}

			[Test][Ignore]
			public void authors_generate_in_feed_properly() {
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
		}
	}
}
