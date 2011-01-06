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

		[Test]
		public void can_take_0_packages_and_generate_an_empty_xml_feed() {
			var feedXml       = Feed.GenerateFeed(new List<Package>());
			var packagesInXml = Feed.ParseFeed(feedXml);

			packagesInXml.Should(Be.Empty); // it should parse OK (without blowing up) and it should be empty
		}

		[Test][Ignore]
		public void can_take_1_package_and_generate_xml_for_a_feed() {
		}

		[Test][Ignore]
		public void can_take_2_packages_and_generate_xml_for_a_feed() {
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
