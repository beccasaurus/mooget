using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class InstallCommandSpec : Spec {

		MooDir           mooDir       = new MooDir(PathToTemp("home", ".moo"));
		DirectoryOfNupkg remoteSource = new DirectoryOfNupkg(PathToContent("more_packages"));

		[SetUp]
		public void Before() {
			base.BeforeEach();
		}

		[Test][Description("moo help install")]
		public void help() {
			moo("help install").ShouldContain("Usage: moo install");
		}

		[Test][Description("moo install MyPackage --source URL")]
		public void can_install_package_from_source() {
			mooDir.Packages.Count.ShouldEqual(0);

			// gets the LATEST version (because no version was specified)
			moo("install NuGet.CommandLine -s {0}", remoteSource.Path).ShouldContain("Installed NuGet.CommandLine-1.1.2120.136");

			mooDir.Packages.Count.ShouldEqual(1);
			mooDir.Packages.First().IdAndVersion().ShouldEqual("NuGet.CommandLine-1.1.2120.136");

			moo("source add Foo {0}", remoteSource.Path);
			moo("install MarkdownSharp --source Foo").ShouldContain("Installed MarkdownSharp-1.13.0.0");

			mooDir.Packages.Count.ShouldEqual(2);
			mooDir.Packages.Select(p => p.IdAndVersion()).ToList().ShouldEqual(new List<string>{ "MarkdownSharp-1.13.0.0", "NuGet.CommandLine-1.1.2120.136" });
		}

		[Test][Description("moo install MyPackage -v 1.0.1 --source URL")]
		public void can_specify_exact_version_of_package_to_install_from_source() {
			mooDir.Packages.Count.ShouldEqual(0);

			moo("install NuGet.CommandLine -v 1.0.11220.26 -s {0}", remoteSource.Path).ShouldContain("Installed NuGet.CommandLine-1.0.11220.26");

			mooDir.Packages.Count.ShouldEqual(1);
			mooDir.Packages.First().IdAndVersion().ShouldEqual("NuGet.CommandLine-1.0.11220.26");
		}

		[Test][Description("moo install MyPackage")]
		public void can_install_package_from_all_remote_sources() {
			mooDir.Packages.Count.ShouldEqual(0);
			moo("install NuGet.CommandLine", remoteSource.Path).ShouldContain("Package not found: NuGet.CommandLine");
			mooDir.Packages.Count.ShouldEqual(0);

			moo("source add {0}", remoteSource.Path);

			moo("install NuGet.CommandLine", remoteSource.Path).ShouldContain("Installed NuGet.CommandLine-1.1.2120.136");

			mooDir.Packages.Count.ShouldEqual(1);
			mooDir.Packages.First().IdAndVersion().ShouldEqual("NuGet.CommandLine-1.1.2120.136");
		}

		[Test][Description("moo install MyPackage")][Ignore]
		public void can_install_package_from_default_source() {
		}
	}
}
