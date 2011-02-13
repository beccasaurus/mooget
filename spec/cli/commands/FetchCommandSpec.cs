using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class FetchCommandSpec : Spec {

		IDirectory       dir          = PathToTemp("FetchHere").AsDir();
		DirectoryOfNupkg remoteSource = new DirectoryOfNupkg(PathToContent("more_packages"));

		[SetUp]
		public void Before() {
			base.BeforeEach();
			dir.Create();
		}

		[Test][Description("moo help fetch")]
		public void help() {
			moo("help fetch").ShouldContain("Usage: moo fetch");
		}

		[Test][Description("moo fetch MyPackage --source URL")]
		public void can_fetch_package_from_source() {
			cd(dir.Path);
			dir.Files().Count.ShouldEqual(0);

			// gets the LATEST version (because no version was specified)
			moo("fetch NuGet.CommandLine -s {0}", remoteSource.Path).ShouldContain("Fetched NuGet.CommandLine-1.1.2120.136.nupkg");

			dir.Files().Count.ShouldEqual(1);
			dir.Files().First().Name().ShouldEqual("NuGet.CommandLine-1.1.2120.136.nupkg");

			moo("source add Foo {0}", remoteSource.Path);
			moo("fetch MarkdownSharp --source Foo").ShouldContain("Fetched MarkdownSharp-1.13.0.0.nupkg");

			dir.Files().Count.ShouldEqual(2);
			dir.Files().Select(f => f.Name()).ToList().ShouldEqual(new List<string>{ "MarkdownSharp-1.13.0.0.nupkg", "NuGet.CommandLine-1.1.2120.136.nupkg" });
		}

		[Test][Description("moo fetch MyPackage -v 1.0.1 --source URL")][Ignore]
		public void can_specify_exact_version_of_package_to_fetch_from_source() {
		}

		[Test][Description("moo fetch MyPackage")][Ignore]
		public void can_fetch_package_from_default_source() {
		}
	}
}
