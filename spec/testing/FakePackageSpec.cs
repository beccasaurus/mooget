using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;
using MooGet.Test;

namespace MooGet.Specs.Testing {

	[TestFixture]
	public class FakePackageSpec : Spec {

		[Test]
		public void Defaults() {
			new FakePackage().ShouldHaveProperties(new {
				Id          = "MyPackage",
				VersionText	= "1.0.0.0"
			});
			new FakePackage().Details.ShouldHaveProperties(new {
				Description = "My description",
				AuthorsText = "remi,Wanda"
			});
			new FakePackage().Details.Dependencies.Should(Be.Empty);
		}

		[Test]
		public void can_set_name() {
			new FakePackage("Foo.Bar").ShouldHaveProperties(new {
				Id          = "Foo.Bar",
				VersionText = "1.0.0.0"		
			});
		}

		[Test]
		public void can_set_name_and_version() {
			new FakePackage("Foo.Bar 4.5").ShouldHaveProperties(new {
				Id          = "Foo.Bar",
				VersionText = "4.5"		
			});
		}

		[Test]
		public void can_add_dependencies() {
			var package = new FakePackage("Foo 1.0", "This >= 5", "That > 4.7.1 <= 5.0");
			package.Id.ShouldEqual("Foo");
			package.VersionText.ShouldEqual("1.0");
			package.Details.Dependencies.Count.ShouldEqual(2);
			package.Details.Dependencies.ToStrings().ShouldEqual(new List<string>{ "This >= 5", "That > 4.7.1 <= 5.0" });
		}
	}
}
