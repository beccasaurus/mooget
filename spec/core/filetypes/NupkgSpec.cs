using System;
using System.IO;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class NupkgSpec : Spec {

		Nupkg nunit;
		Nupkg fluent;

		[SetUp]
		public void Before() {
			nunit  = new Nupkg(PathToContent("packages/NUnit.2.5.7.10213.nupkg"));
			fluent = new Nupkg(PathToContent("packages/FluentNHibernate.1.1.0.694.nupkg"));
		}

		[Test]
		public void has_a_Path() {
			nunit.Path.ShouldEqual(PathToContent("packages/NUnit.2.5.7.10213.nupkg"));
			fluent.Path.ShouldEqual(PathToContent("packages/FluentNHibernate.1.1.0.694.nupkg"));
		}

		[Test]
		public void can_check_if_Exists() {
			nunit.Exists.ShouldBeTrue();
			fluent.Exists.ShouldBeTrue();
			new Nupkg("/i/dont/exist.nupkg").Exists.ShouldBeFalse();
		}

		[Test][Ignore]
		public void Nupsec() {
			nunit.Nuspec.Id.ShouldEqual("NUnit");
			fluent.Nuspec.Id.ShouldEqual("FluentNHibernate");
		}

		[Test][Ignore]
		public void Id() {
		}

		[Test][Ignore]
		public void Version() {
		}

		[Test][Ignore]
		public void Details() {
		}

		[Test][Ignore]
		public void Files() {
		}

		[Test][Ignore]
		public void Libraries() {
		}

		[Test][Ignore]
		public void just_NET35_Libraries() {
		}

		[Test][Ignore]
		public void just_NET40_Libraries() {
		}

		[Test][Ignore]
		public void Tools() {
		}

		[Test][Ignore]
		public void Content() {
		}

		[Test][Ignore]
		public void SourceFiles() {
		}
	}
}
