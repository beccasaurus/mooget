using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class UninstallCommandSpec : Spec {

		MooDir mooDir = new MooDir(PathToTemp("home", ".moo"));

		[Test][Description("moo help uninstall")]
		public void help() {
			moo("help uninstall").ShouldContain("Usage: moo uninstall");
		}

		[Test][Description("moo uninstall MyPackage")]
		public void can_uninstall_the_only_installed_version_of_a_package() {
			moo("install {0}", PathToContent("package_working_directories", "just-a-tool-1.0.0.0.nupkg"));
			mooDir.Packages.Count.ShouldEqual(1);
			File.Exists(Path.Combine(mooDir.BinDirectory, "tool")).Should(Be.True);
			File.Exists(Path.Combine(mooDir.BinDirectory, "tool.bat")).Should(Be.True);

			moo("uninstall just-a-tool").ShouldContain("Uninstalled just-a-tool-1.0.0.0");

			mooDir.Packages.Count.ShouldEqual(0);
			File.Exists(Path.Combine(mooDir.BinDirectory, "tool")).Should(Be.False);
			File.Exists(Path.Combine(mooDir.BinDirectory, "tool.bat")).Should(Be.False);

			moo("uninstall just-a-tool").ShouldContain("Could not uninstall just-a-tool");
		}

		[Test][Description("moo uninstall MyPackage --all")]
		public void can_uninstall_all_versions_of_a_package() {
			moo("install {0}", PathToContent("more_packages", "NuGet.CommandLine-1.0.11220.26.nupkg"));
			moo("install {0}", PathToContent("more_packages", "NuGet.CommandLine-1.1.2120.136.nupkg"));
			mooDir.Packages.Count.ShouldEqual(2);

			var output = moo("uninstall NuGet.CommandLine --all");
			output.ShouldContain("Uninstalled NuGet.CommandLine-1.0.11220.26");
			output.ShouldContain("Uninstalled NuGet.CommandLine-1.1.2120.136");

			mooDir.Packages.Count.ShouldEqual(0);

			moo("uninstall NuGet.CommandLine --version 1.0.11220.26").ShouldContain("Could not uninstall NuGet.CommandLine");
		}

		[Test][Description("moo uninstall MyPackage --version 1.0")]
		public void can_uninstall_just_one_version_of_a_package() {
			moo("install {0}", PathToContent("more_packages", "NuGet.CommandLine-1.0.11220.26.nupkg"));
			moo("install {0}", PathToContent("more_packages", "NuGet.CommandLine-1.1.2120.136.nupkg"));
			mooDir.Packages.Count.ShouldEqual(2);

			moo("uninstall NuGet.CommandLine --version 1.0.11220.26").ShouldContain("Uninstalled NuGet.CommandLine-1.0.11220.26");

			mooDir.Packages.Count.ShouldEqual(1);
			mooDir.Packages.First().IdAndVersion().ShouldEqual("NuGet.CommandLine-1.1.2120.136");

			moo("uninstall NuGet.CommandLine --version 1.0.11220.26").ShouldContain("Could not uninstall NuGet.CommandLine = 1.0.11220.26");
		}

		[Test][Description("moo uninstall ... ?")][Ignore]
		public void can_uninstall_package_without_uninstalling_dependencies() {
		}
	}
}
