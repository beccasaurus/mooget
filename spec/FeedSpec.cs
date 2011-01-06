using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class FeedSpec : MooGetSpec {

		[Test][Ignore]
		public void can_parse_a_feeds_xml_into_a_list_of_packages() {
		}

		[Test][Ignore]
		public void generated_feed_can_be_parsed_as_a_valid_Atom_feed() {
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

		[Test][Ignore]
		public void can_take_2_packages_and_generate_xml_for_a_feed() {
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
	}
}
