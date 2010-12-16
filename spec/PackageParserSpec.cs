using System;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs {

	[TestFixture]
	public class Parsing_packages_from_feed : MooGetSpec {

		public static List<Package> Packages = Package.ParseFeed(ReadContent("example-feed.xml"));

		[Test]
		public void Can_get_packages_from_a_feed_string() {
			Assert.That(Packages.Count,     Is.EqualTo(151));
			Assert.That(Packages[0].Name,   Is.EqualTo("Adam.JSGenerator"));
			Assert.That(Packages[150].Name, Is.EqualTo("xmlbuilder"));
		}

		[TestFixture]
		public class Correctly_parses_all_package_metadata : Parsing_packages_from_feed {

			[Test]
			public void Example_1() {
				var package = Packages[0];
				Assert.That( package.Name, Is.EqualTo("Adam.JSGenerator") );
				Assert.That( package.Publis, Is.EqualTo("Adam.JSGenerator") ); /// ... feed has lots of meta stuff we want ...
			}
		}
	}
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
