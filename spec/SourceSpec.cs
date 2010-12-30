using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class SourceSpec : MooGetSpec {

		// see ./spec/content/example-feed.xml to see the feed that these specs use
		static Source              source   = new Source(PathToContent("example-feed.xml"));
		static List<SourcePackage> packages = source.Packages;

		[Test]
		public void can_get_all_packages_from_a_source() {
			Assert.That( packages.Count, Is.EqualTo(151));

			Assert.That( packages[0].Id,          Is.EqualTo("Adam.JSGenerator"));
			Assert.That( packages[0].Title,       Is.EqualTo("Adam.JSGenerator"));
			Assert.That( packages[0].Description, Is.EqualTo("Adam.JSGenerator helps producing snippets of JavaScript code from managed code."));

			Assert.That( packages[150].Id,          Is.EqualTo("xmlbuilder"));
			Assert.That( packages[150].Title,       Is.EqualTo("xmlbuilder"));
			Assert.That( packages[150].Description, Is.EqualTo("A DSL to help on XML authoring, with this library you can create xml content with few lines of code, there's no need to use System.Xml classes, the XMLBuilder hides all complexity behind xml generation."));
		}

		[Test][Ignore]
		public void can_see_all_versions_that_a_source_package_has_available() {}

		[Test]
		public void can_search_for_package_by_title() {
			var packages = source.SearchByTitle("ninject");
			Assert.That( packages.Count, Is.EqualTo(5) );
			Assert.That( packages.Select(p => p.Title).ToArray(), 
					     Is.EqualTo(new string[] { "MvcTurbine.Ninject", "Ninject", "Ninject", "Ninject.MVC3", "SNAP.Ninject" }));
		}

		[Test]
		public void can_search_for_package_by_description() {
			var packages = source.SearchByDescription("dependency injection");
			Assert.That( packages.Count, Is.EqualTo(6) );
			Assert.That( packages.Select(p => p.Title).ToArray(), 
					     Is.EqualTo(new string[] { "LightCore", "LightCore.WebIntegration", "Ninject", "Ninject", "structuremap", "Unity" }));
		}

		[Test]
		public void can_search_for_package_by_title_or_description() {
			var packages = source.SearchByTitleOrDescription("xml");
			Assert.That( packages.Count, Is.EqualTo(7) );
			Assert.That( packages.Select(p => p.Title).ToArray(),
					     Is.EqualTo(new string[] { "AntiXSS", "Artem.XmlProviders", "chargify", "FluentNHibernate", "FluentNHibernate", "protobuf-net", "xmlbuilder" }));

		}
	}
}

/*
	XML for the first and last packages, for reference

entry>
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
  <pkg:keywords xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    <string xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays">JavaScript JSON</string>
  </pkg:keywords>
</entry>

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
