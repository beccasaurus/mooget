using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class SourceSpec : MooGetSpec {

		[TestFixture]
		public class Integration : SourceSpec {

			[Test]
			public void can_add_a_source() {
				moo("source").ShouldContain(Moo.OfficialNugetFeed);
				moo("source").ShouldNotContain("http://foo.com/whatever");

				moo("source add http://foo.com/whatever");

				moo("source").ShouldContain(Moo.OfficialNugetFeed);
				moo("source").ShouldContain("http://foo.com/whatever");
			}

			[Test]
			public void can_remove_a_source() {
				moo("source add http://foo.com/whatever");

				moo("source").ShouldContain(Moo.OfficialNugetFeed);
				moo("source").ShouldContain("http://foo.com/whatever");

				moo("source rm {0}", Moo.OfficialNugetFeed);

				moo("source").ShouldNotContain(Moo.OfficialNugetFeed);
				moo("source").ShouldContain("http://foo.com/whatever");
			}

			[Test]
			public void default_sources_include_official_NuGet_feed() {
				// Moo.Dir should not exist because it's not "installed"
				Directory.Exists(PathToTempHome(".moo")).ShouldBeFalse();

				moo("source").ShouldContain(Moo.OfficialNugetFeed);

				// Moo.Dir should still not exist because it's not "installed"
				Directory.Exists(PathToTempHome(".moo")).ShouldBeFalse();
			}
		}

		// see ./spec/content/example-feed.xml to see the feed that these specs use
		static Source source = new Source(PathToContent("example-feed.xml"));

		[Test]
		public void can_get_all_packages_from_a_source() {
			var packages = source.AllPackages;
			packages.Count.ShouldEqual(151);
			packages.First().Id.ShouldEqual("Adam.JSGenerator");
			packages.Last().Id.ShouldEqual("xmlbuilder");
		}

		/*
<entry>
  <id>urn:uuid:23ddcce9-f31c-4d28-93fd-0496a433b972</id>
  <title type="text">Adam.JSGenerator</title>
  <published>2010-10-25T22:55:16Z</published>
  <updated>2010-10-25T22:55:16Z</updated>
  <author>
    <name>Dave Van den Eynde</name>
  </author>
  <author>
    <name>Wouter Demuynck</name>
  </author>
  <link rel="enclosure" href="http://173.203.67.148/ctp1/packages/download?p=Adam.JSGenerator.1.1.0.0.nupkg"/>
  <content type="text">Adam.JSGenerator helps producing snippets of JavaScript code from managed code.</content>
  <pkg:requireLicenseAcceptance>false</pkg:requireLicenseAcceptance>
  <pkg:packageId>Adam.JSGenerator</pkg:packageId>
  <pkg:version>1.1.0.0</pkg:version>
  <pkg:language>en-US</pkg:language>
  <pkg:tags xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    <string xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays">JavaScript JSON</string>
  </pkg:tags>
</entry>
		*/
		[Test]
		public void can_read_all_package_info__example_1() {
			var package = source.AllPackages.First();

			package.Id.ShouldEqual("Adam.JSGenerator");
			package.Title.ShouldEqual("Adam.JSGenerator");
			package.Description.ShouldEqual("Adam.JSGenerator helps producing snippets of JavaScript code from managed code.");
			package.DownloadUrl.ShouldEqual("http://173.203.67.148/ctp1/packages/download?p=Adam.JSGenerator.1.1.0.0.nupkg");
			package.Authors.Count.ShouldEqual(2);
			package.Authors.ShouldContain("Dave Van den Eynde");
			package.Authors.ShouldContain("Wouter Demuynck");
			package.VersionString.ShouldEqual("1.1.0.0");
			package.Language.ShouldEqual("en-US");
			package.Tags.Count.ShouldEqual(2);
			package.Tags.ShouldContain("JavaScript");
			package.Tags.ShouldContain("JSON");
			package.RequireLicenseAcceptance.ShouldBeFalse();
		}

		/*
<entry>
  <id>urn:uuid:42a992a5-fb8a-4402-bf74-8d75b8a59e00</id>
  <title type="text">xmlbuilder</title>
  <published>2010-10-25T22:55:49Z</published>
  <updated>2010-10-25T22:55:49Z</updated>
  <author>
    <name>Rogerio Araujo</name>
  </author>
  <link rel="enclosure" href="http://173.203.67.148/ctp1/packages/download?p=xmlbuilder.1.0.1.nupkg"/>
  <link rel="license" href="http://www.apache.org/licenses/LICENSE-2.0.html"/>
  <content type="text">A DSL to help on XML authoring, with this library you can create xml content with few lines of code, there's no need to use System.Xml classes, the XMLBuilder hides all complexity behind xml generation.</content>
  <pkg:requireLicenseAcceptance>false</pkg:requireLicenseAcceptance>
  <pkg:packageId>xmlbuilder</pkg:packageId>
  <pkg:version>1.0.1</pkg:version>
  <pkg:language>en-US</pkg:language>
</entry>
		*/
		[Test] // be sure to get its LicenseUrl
		public void can_read_all_package_info__example_2() {
			var package = source.AllPackages.Last();

			package.Id.ShouldEqual("xmlbuilder");
			package.VersionString.ShouldEqual("1.0.1");
			package.Title.ShouldEqual("xmlbuilder");
			package.Description.ShouldEqual("A DSL to help on XML authoring, with this library you can create xml content with few lines of code, there's no need to use System.Xml classes, the XMLBuilder hides all complexity behind xml generation.");
			package.Authors.Count.ShouldEqual(1);
			package.Authors.First().ShouldEqual("Rogerio Araujo");
			package.DownloadUrl.ShouldEqual("http://173.203.67.148/ctp1/packages/download?p=xmlbuilder.1.0.1.nupkg");
			package.LicenseUrl.ShouldEqual("http://www.apache.org/licenses/LICENSE-2.0.html");
			package.Language.ShouldEqual("en-US");
			package.RequireLicenseAcceptance.ShouldBeFalse();
		}

		[Test]
		public void can_read_all_dependencies() {
			var package = source.Get("ESRI.SilverlightWpf.Design.Types");
			package.Dependencies.Count.ShouldEqual(4);
			foreach (var dependency in new Dictionary<string,string>(){
				{"ESRI.SilverlightWpf.Core",      "2.0.0.314"},
				{"ESRI.SilverlightWpf.Behaviors", "2.0.0.314"},
				{"ESRI.SilverlightWpf.Bing",      "2.0.0.314"},
				{"ESRI.SilverlightWpf.Toolkit",   "2.0.0.314"}
			}) {
				package.Dependencies.Select(d => d.Id).ShouldContain(dependency.Key);
				package.Dependencies.Select(d => d.VersionString).ShouldContain(dependency.Value);
			}
		}

		// either nothing currently uses this or it's not displayed in the feed
		//[Test][Ignore]
		//public void can_read_summary() {}

		// // nothing currently uses this ... i'll add some fake examples of my own
		// [Test][Ignore]
		// public void can_read_ProjectUrl() {}

		// // nothing currently uses this ... i'll add some fake examples of my own
		// [Test][Ignore]
		// public void can_read_IconUrl() {}

		// // not displayed in the feed?  or no packages use this?
		// [Test][Ignore] // for us, this should be useful for tracking who can push/remove updates
		// public void can_read_Owners() {}

		[Test]
		public void reads_many_tags_split_by_spaces_if_only_one_string_element() {
			var package = source.Get("FluentCassandra");
			package.Tags.Count.ShouldEqual(5);
			foreach (var tag in new string[] { "Fluent", "Cassandra", "Apache", "NoSQL", "Database" })
				package.Tags.ShouldContain(tag);
		}

		[Test]
		public void reads_many_tags_without_splitting_if_many_string_elements() {
			var package = source.Get("common.eventing");
			package.Tags.Count.ShouldEqual(4);
			foreach (var tag in new string[] { "pub", "sub", "event", "broker" })
				package.Tags.ShouldContain(tag);
		}

		// nuspec doesn't specify what categories are for, so we stick them into the package's tags
		[Test]
		public void can_read_category_terms_as_tags() {
			var socketLabs = source.AllPackages.First(p => p.Id == "SocketLabs");
			socketLabs.Tags.Count.ShouldEqual(1);
			socketLabs.Tags.First().ShouldEqual("Email"); // <--- comes from a category
		}

		[Test]
		public void can_get_just_the_latest_versions_of_packages_from_a_source() {
			var packages = source.Packages; // or LatestPackages
			Assert.That( packages.Count, Is.EqualTo(140));

			// TODO need to check that the versions are actually the LATEST and not, say ... the oldest?
		}

		[Test][Ignore]
		public void can_get_all_of_the_versions_available_for_a_particular_package() {
		}

		[Test][Ignore("TODO clean up searching ... re-spec searching AS WE NEED IT")]
		public void can_search_for_package_by_title() {
			var packages = source.SearchByTitle("ninject");
			Assert.That( packages.Count, Is.EqualTo(4) );
			Assert.That( packages.Select(p => p.Title).ToArray(), 
					     Is.EqualTo(new string[] { "MvcTurbine.Ninject", "Ninject", "Ninject.MVC3", "SNAP.Ninject" }));
		}

		[Test]
		public void can_search_for_package_by_description() {
			var packages = source.SearchByDescription("dependency injection");
			Assert.That( packages.Count, Is.EqualTo(5) );
			Assert.That( packages.Select(p => p.Title).ToArray(), 
					     Is.EqualTo(new string[] { "LightCore", "LightCore.WebIntegration", "Ninject", "structuremap", "Unity" }));
		}

		[Test]
		public void can_search_for_package_by_title_or_description() {
			var packages = source.SearchByTitleOrDescription("xml");
			Assert.That( packages.Count, Is.EqualTo(6) );
			Assert.That( packages.Select(p => p.Title).ToArray(),
					     Is.EqualTo(new string[] { "AntiXSS", "Artem.XmlProviders", "chargify", "FluentNHibernate", "protobuf-net", "xmlbuilder" }));

		}
	}
}
