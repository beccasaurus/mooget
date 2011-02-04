using System;
using System.IO;
using System.Linq;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class ShowSpec : MooGetSpec {

		[TestFixture]
		public class API : MooGetSpec {
						
		}

		[TestFixture]
		public class Integration : MooGetSpec {

			[SetUp]
			public void Before() {
				moo("source add {0}", PathToContent("example-feed.xml"));
				moo("source rm {0}",  Moo.OfficialNugetFeed);
			}

			[Test][Ignore]
			public void can_show_an_installed_packages_info() {
			}

			[Test]
			public void can_show_a_packages_info_from_feed() {
				var output = moo("show IdontExist");
				output.ShouldContain("not found");

				output = moo("show Castle.DynamicProxy");
				Console.WriteLine(output);
				output.ShouldNotContain("not found");
				output.ShouldContain("library for generating lightweight .NET proxies");
				output.ShouldContain("Jonathon Rossi & Krzysztof Kozmic");
				output.ShouldContain("2.2.0");
				output.ShouldContain("Castle.Core = 1.2.0");
				output.ShouldContain("http://173.203.67.148/ctp1/packages/download?p=Castle.DynamicProxy.2.2.0.nupkg");
			}
		}
	}
}
