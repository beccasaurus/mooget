using System;
using System.IO;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class InstallSpec : MooGetSpec {

		[TestFixture]
		public class API : MooGetSpec {
			
			[Test]
			public void can_install_a_nupkg() {
				Directory.Exists(PathToTempHome(".moo")).ShouldBeFalse();

				Moo.Install(PathToContent("packages", "MarkdownSharp.1.13.0.0.nupkg"));

				Directory.Exists(PathToTempHome(".moo")).ShouldBeTrue();
				
				// nupkg should be copied into ~/.moo/cache
				File.Exists(PathToTempHome(".moo", "cache", "MarkdownSharp-1.13.0.0.nupkg")).ShouldBeTrue();

				// nuspec should be copied into ~/.moo/specifications
				File.Exists(PathToTempHome(".moo", "specifications", "MarkdownSharp-1.13.0.0.nuspec")).ShouldBeTrue();

				// bin directory should be created, although MarkdownSharp doesn't have any executables, so it will be empty
				Directory.Exists(PathToTempHome(".moo", "bin")).ShouldBeTrue();

				// it should unpack MarkdownSharp into ~/.moo/packages/
				// we don't currently mess with anything in the nupkg except we don't unpack stupid things like the psmdcp or [Content_Types] or _rels
				// we will probably want to normalize the FrameworkName?
				// NOTE System.Runtime.Versioning.FrameworkName is only available in .NET 4.0 so we need to make our own
				File.Exists(PathToTempHome(".moo", "packages", "MarkdownSharp-1.13.0.0", "lib", "35", "MarkdownSharp.dll")).ShouldBeTrue();

				// ignore stupid stuff!
				File.Exists(PathToTempHome(".moo", "packages", "MarkdownSharp-1.13.0.0", "[Content_Types].xml")).ShouldBeFalse();
				Directory.Exists(PathToTempHome(".moo", "packages", "MarkdownSharp-1.13.0.0", "_rels")).ShouldBeFalse();
				Directory.Exists(PathToTempHome(".moo", "packages", "MarkdownSharp-1.13.0.0", "package")).ShouldBeFalse();
			}

			[Test][Ignore]
			public void InstallsDependencies() {}
		}

		[TestFixture]
		public class Integration : MooGetSpec {

			[Test][Ignore]
			public void can_install_a_nupkg() {
				//Directory.Exists(PathToTemp("working", "MarkdownSharp.1.13.0.0")).ShouldBeFalse();

				//moo("unpack {0}", PathToContent("packages", "MarkdownSharp.1.13.0.0.nupkg")).ShouldEqual("Unpacked MarkdownSharp.1.13.0.0");

				//Directory.Exists(PathToTemp("working", "MarkdownSharp.1.13.0.0")).ShouldBeTrue();
			}
		}
	}
}
