using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class ZipSpec : Spec {

		Zip markdown;
		Zip nunit;
		Zip tool;
	   
		[SetUp]
		public void Before() {
			base.BeforeEach();
			markdown = new Zip(PathToContent("packages/MarkdownSharp.1.13.0.0.nupkg"));
			nunit    = new Zip(PathToContent("packages/NUnit.2.5.7.10213.nupkg"));
			tool     = new Zip(PathToContent("package_working_directories/just-a-tool-1.0.0.0.nupkg"));
		}

		[Test]
		public void has_Path() {
			new Zip().Path.Should(Be.Null);
			new Zip("/foo.nupkg").Path.ShouldEqual("/foo.nupkg");
			new Zip { Path = "/foo.nupkg" }.Path.ShouldEqual("/foo.nupkg");
		}

		[Test]
		public void Exists() {
			new Zip("/i/dont/exist.zip").Exists.Should(Be.False);
			new Zip(PathToContent("packages/MarkdownSharp.1.13.0.0.nupkg")).Exists.Should(Be.True);
		}

		[Test]
		public void Paths() {
			markdown.Paths.ShouldEqual(new List<string>{ "MarkdownSharp.nuspec", "lib/35/MarkdownSharp.dll", "lib/35/MarkdownSharp.pdb", "lib/35/MarkdownSharp.xml" });
			tool.Paths.ShouldEqual(new List<string>{ "just-a-tool.nuspec", "tools/tool.exe" });
		}

		[Test]
		public void Read() {
			tool.Read("just-a-tool.nuspec").Should(Be.StringStarting("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<package>\n  <metadata>"));
			tool.Read("just-a-tool.nuspec").Should(Be.StringEnding("  </metadata>\n</package>\n"));
		}

/*
NUnit paths:

NUnit.nuspec, fit-license.txt, license.txt, Logo.ico, NUnitFitTests.html, Content/NUnitSampleTests.cs.pp, lib/nunit.framework.dll, 
lib/nunit.framework.xml, lib/nunit.mocks.dll, lib/pnunit.framework.dll, Tools/agent.conf, Tools/agent.log.conf, Tools/launcher.log.conf, 
Tools/nunit-agent-x86.exe, Tools/nunit-agent-x86.exe.config, Tools/nunit-agent.exe, Tools/nunit-agent.exe.config, Tools/nunit-console-x86.exe, 
Tools/nunit-console-x86.exe.config, Tools/nunit-console.exe, Tools/nunit-console.exe.config, Tools/nunit-x86.exe, Tools/nunit-x86.exe.config, 
Tools/nunit.exe, Tools/nunit.exe.config, Tools/nunit.framework.dll, Tools/NUnitFitTests.html, Tools/NUnitTests.config, Tools/NUnitTests.nunit, 
Tools/pnunit-agent.exe, Tools/pnunit-agent.exe.config, Tools/pnunit-launcher.exe, Tools/pnunit-launcher.exe.config, Tools/pnunit.framework.dll, 
Tools/pnunit.tests.dll, Tools/runFile.exe, Tools/runFile.exe.config, Tools/runpnunit.bat, Tools/test.conf, Tools/lib/Failure.png, Tools/lib/fit.dll, 
Tools/lib/Ignored.png, Tools/lib/Inconclusive.png, Tools/lib/log4net.dll, Tools/lib/nunit-console-runner.dll, 
Tools/lib/nunit-gui-runner.dll, Tools/lib/nunit.core.dll, Tools/lib/nunit.core.interfaces.dll, Tools/lib/nunit.fixtures.dll, 
Tools/lib/nunit.uiexception.dll, Tools/lib/nunit.uikit.dll, Tools/lib/nunit.util.dll, Tools/lib/Skipped.png, Tools/lib/Success.png
*/
		[Test]
		public void Search() {
			// ONLY the base directory
			nunit.Search("*.html").ShouldEqual(new List<string>{ "NUnitFitTests.html" });

			// ALL directories
			nunit.Search(@"**.html").ShouldEqual(new List<string>{ "NUnitFitTests.html", "Tools/NUnitFitTests.html" });
			
			// ONLY subdirectories
			nunit.Search(@"**\*.html").ShouldEqual(new List<string>{ "Tools/NUnitFitTests.html" });

			// Custom Regex
			nunit.Search(new Regex("log4|interfaces")).ShouldEqual(new List<string>{ "Tools/lib/log4net.dll", "Tools/lib/nunit.core.interfaces.dll" });
		}

		[Test]
		public void AddNew_and_AddExisting() {
			var myzip = new Zip(PathToTemp("my.zip"));
			myzip.Exists.Should(Be.False);
			myzip.Paths.Should(Be.Null);

			myzip.AddNew("README", "This is the README text!");
			myzip.AddExisting("lib/foo.txt", PathToContent("moofile_examples/Moofile2"));
			myzip.AddExisting("this/is/a/deep/directory/foo.exe", PathToContent("package_working_directories/just-a-tool/tools/tool.exe"));

			myzip.Exists.Should(Be.True);
			myzip.Paths.ShouldEqual(new List<string>{ "README", "lib/foo.txt", "this/is/a/deep/directory/foo.exe" });
			myzip.Read("README").ShouldEqual("This is the README text!");
			myzip.Read("lib/foo.txt").ShouldEqual("src\n\tForSource\n\nspec\n\tForSpecs1\n\tForSpecs2\n");
		}

		[Test]
		public void Extract() {
			// if extract to existing directory, extracts into it
			var dir = PathToTemp("ExtractHere").AsDir().Create();
			PathToTemp("ExtractHere"                                 ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0"          ).AsDir().Exists().Should(Be.False);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "tools" ).AsDir().Exists().Should(Be.False);

			tool.Extract(dir.Path);

			PathToTemp("ExtractHere"                                              ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0"                       ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "tools"              ).AsDir().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "just-a-tool.nuspec" ).AsFile().Exists().Should(Be.True);
			PathToTemp("ExtractHere", "just-a-tool-1.0.0.0", "tools", "tool.exe"  ).AsFile().Exists().Should(Be.True);

			// if extract to new directory, extract exactly there
			PathToTemp("Exact_Dir"          ).AsDir().Exists().Should(Be.False);
			PathToTemp("Exact_Dir", "tools" ).AsDir().Exists().Should(Be.False);

			tool.Extract(PathToTemp("Exact_Dir"));

			PathToTemp("Exact_Dir"                       ).AsDir().Exists().Should(Be.True);
			PathToTemp("Exact_Dir", "tools"              ).AsDir().Exists().Should(Be.True);
			PathToTemp("Exact_Dir", "just-a-tool.nuspec" ).AsFile().Exists().Should(Be.True);
			PathToTemp("Exact_Dir", "tools", "tool.exe"  ).AsFile().Exists().Should(Be.True);
		}
	}
}
