using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class NupkgSpec : Spec {

		// A Nupkg implements IPackageFile which gives it nearly all of the methods we need to satisfy this spec.
		IPackageFile nunit;
		IPackageFile fluent;

		[SetUp]
		public void Before() {
			base.BeforeEach();
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
			nunit.Exists().ShouldBeTrue();
			fluent.Exists().ShouldBeTrue();
			new Nupkg("/i/dont/exist.nupkg").Exists.ShouldBeFalse();
		}

		[Test]
		public void Id() {
			nunit.Id.ShouldEqual("NUnit");
			fluent.Id.ShouldEqual("FluentNHibernate");
		}

		[Test]
		public void Version() {
			nunit.Version.ToString().ShouldEqual("2.5.7.10213");
			nunit.Version.ToString().ShouldEqual("2.5.7.10213");
			fluent.Version.ToString().ShouldEqual("1.1.0.694");
		}

		[Test]
		public void Details() {
			nunit.Id.ShouldEqual("NUnit");
			nunit.Version.ToString().ShouldEqual("2.5.7.10213");
			nunit.Details.AuthorsText.ShouldEqual("Charlie Poole");

			(nunit as Nupkg).Nuspec.ShouldHaveProperties(new {
				Id          = "NUnit",
				VersionText = "2.5.7.10213",
				AuthorsText = "Charlie Poole"
			});

			fluent.Id.ShouldEqual("FluentNHibernate");
			fluent.Version.ToString().ShouldEqual("1.1.0.694");
			fluent.Details.AuthorsText.ShouldEqual("James Gregory");

			(fluent as Nupkg).Nuspec.ShouldHaveProperties(new {
				Id          = "FluentNHibernate",
				VersionText = "1.1.0.694",
				AuthorsText = "James Gregory"
			});
		}

		[Test]
		public void Files() {
			nunit.Files.ShouldEqual((nunit as Nupkg).Zip.Paths);
			fluent.Files.ShouldEqual((fluent as Nupkg).Zip.Paths);
		}

		[Test]
		public void Unpack() {
			var tool = new Nupkg(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg"));

			// if extract to existing directory, extracts into it
			var dir = PathToTemp("ExtractHere").AsDir().Create();
			PathToTemp("ExtractHere"                                 ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0"          ).AsDir().Exists().Should(Be.False);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "tools" ).AsDir().Exists().Should(Be.False);

			var unpacked = tool.Unpack(dir.Path);

			unpacked.Should(Be.InstanceOf(typeof(UnpackedNupkg)));
			unpacked.Path.ShouldEqual(PathToTemp("ExtractHere", "just-a-tool-1.0.0.0"));
			unpacked.Id.ShouldEqual("just-a-tool");
			(unpacked as UnpackedNupkg).Nuspec.Path.ShouldEqual(PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "just-a-tool.nuspec"));
			unpacked.Tools.ShouldEqual(new List<string>{ PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "tools", "tool.exe") });

			PathToTemp("ExtractHere"                                              ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0"                       ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "tools"              ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "just-a-tool.nuspec" ).AsFile().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "tools", "tool.exe"  ).AsFile().Exists().Should(Be.True);

			// if extract to new directory, extract exactly there
			PathToTemp("Exact_Dir"          ).AsDir().Exists().Should(Be.False);
			PathToTemp("Exact_Dir", "tools" ).AsDir().Exists().Should(Be.False);

			unpacked = tool.Unpack(PathToTemp("Exact_Dir"));

			unpacked.Should(Be.InstanceOf(typeof(UnpackedNupkg)));
			unpacked.Path.ShouldEqual(PathToTemp("Exact_Dir"));
			unpacked.Id.ShouldEqual("just-a-tool");
			(unpacked as UnpackedNupkg).Nuspec.Path.ShouldEqual(PathToTemp("Exact_Dir", "just-a-tool.nuspec"));
			unpacked.Tools.ShouldEqual(new List<string>{ PathToTemp("Exact_Dir", "tools", "tool.exe") });

			PathToTemp("Exact_Dir"                       ).AsDir().Exists().Should(Be.True);
			PathToTemp("Exact_Dir", "tools"              ).AsDir().Exists().Should(Be.True);
			PathToTemp("Exact_Dir", "just-a-tool.nuspec" ).AsFile().Exists().Should(Be.True);
			PathToTemp("Exact_Dir", "tools", "tool.exe"  ).AsFile().Exists().Should(Be.True);

			// it should name the directory using the package id and version, not the name of the .nupkg file ...
			PathToTemp("MarkdownSharp-1.13.0.0").AsDir().Exists().Should(Be.False);
			new Nupkg(PathToContent("packages/unnamed.nupkg")).Unpack(PathToTemp(""));
			PathToTemp("MarkdownSharp-1.13.0.0").AsDir().Exists().Should(Be.True);
		}
	}
}
