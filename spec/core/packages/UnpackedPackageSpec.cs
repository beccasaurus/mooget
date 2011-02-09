using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class UnpackedPackageSpec : Spec {

		UnpackedPackage nunit;
		UnpackedPackage fluent;

		[SetUp]
		public void Before() {
			nunit  = new UnpackedPackage(PathToContent("packages/NUnit.2.5.7.10213"));
			fluent = new UnpackedPackage(PathToContent("packages/FluentNHibernate.1.1.0.694"));
		}

		[Test]
		public void has_a_Path() {
			nunit.Path.ShouldEqual(PathToContent("packages/NUnit.2.5.7.10213"));
			fluent.Path.ShouldEqual(PathToContent("packages/FluentNHibernate.1.1.0.694"));
		}

		[Test]
		public void can_check_if_Exists() {
			nunit.Exists.ShouldBeTrue();
			fluent.Exists.ShouldBeTrue();
			new UnpackedPackage("/i/dont/exist").Exists.ShouldBeFalse();
		}

		[Test]
		public void Nuspec() {
			nunit.Nuspec.ShouldHaveProperties(new {
				Id          = "NUnit",
				VersionText = "2.5.7.10213",
				AuthorsText = "Charlie Poole"
			});

			fluent.Nuspec.ShouldHaveProperties(new {
				Id          = "FluentNHibernate",
				VersionText = "1.1.0.694",
				AuthorsText = "James Gregory"
			});
		}

		[Test]
		public void Id() {
			nunit.Id.ShouldEqual("NUnit");
			fluent.Id.ShouldEqual("FluentNHibernate");
		}

		[Test]
		public void Version() {
			nunit.Version.ToString().ShouldEqual("2.5.7.10213");
			nunit.VersionText.ShouldEqual("2.5.7.10213");
			fluent.Version.ToString().ShouldEqual("1.1.0.694");
			fluent.VersionText.ShouldEqual("1.1.0.694");
		}

		[Test]
		public void Details() {
			nunit.Details.ShouldEqual(nunit.Nuspec);
			fluent.Details.ShouldEqual(fluent.Nuspec);
		}

		[Test]
		public void Files() {
			fluent.Files.Select(path => path.Replace(fluent.Path, "")).ToList().ShouldEqual(new List<string>{
				"/FluentNHibernate.nuspec", 
				"/[Content_Types].xml", 
				"/_rels/.rels", 
				"/lib/FluentNHibernate.XML", 
				"/lib/FluentNHibernate.dll", 
				"/lib/FluentNHibernate.pdb", 
				"/package/services/metadata/core-properties/902c256232984c97952ba394905bddfe.psmdcp"
			});
			nunit.Files.Length.ShouldEqual(57);
			var nunitFiles = nunit.Files.Select(path => path.Replace(nunit.Path, "")).ToList();
			nunitFiles.ShouldContain("/Logo.ico");
			nunitFiles.ShouldContain("/NUnit.nuspec");
			nunitFiles.ShouldContain("/Content/NUnitSampleTests.cs.pp");
			nunitFiles.ShouldContain("/Tools/NUnitTests.config");
			nunitFiles.ShouldContain("/Tools/nunit-agent-x86.exe");
			nunitFiles.ShouldContain("/Tools/lib/fit.dll");
			nunitFiles.ShouldContain("/Tools/lib/Skipped.png");
			nunitFiles.ShouldContain("/lib/nunit.framework.dll");
			nunitFiles.ShouldContain("/lib/pnunit.framework.dll");
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
