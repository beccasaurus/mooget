using System;
using System.IO;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class UnpackSpec : MooGetSpec {

		[TestFixture]
		public class API : MooGetSpec {
			
			[Test]
			public void can_unpack_a_nupkg_into_the_current_directory() {
				Directory.Exists(PathToTemp("MarkdownSharp.1.13.0.0")).ShouldBeFalse();

				// Unpack(nupkg [, directory to unpack into]);
				Moo.Unpack(PathToContent("packages", "MarkdownSharp.1.13.0.0.nupkg"), TempDirectory);

				Directory.Exists(PathToTemp("MarkdownSharp.1.13.0.0")).ShouldBeTrue();
				File.Exists(PathToTemp("MarkdownSharp.1.13.0.0", "MarkdownSharp.nuspec")).ShouldBeTrue();
				File.Exists(PathToTemp("MarkdownSharp.1.13.0.0", "lib", "35", "MarkdownSharp.dll")).ShouldBeTrue();
			}
		}

		[TestFixture]
		public class Integration : MooGetSpec {

			[Test]
			public void can_unpack_a_nupkg_into_the_current_directory() {
				Directory.Exists(PathToTemp("working", "MarkdownSharp.1.13.0.0")).ShouldBeFalse();

				moo("unpack {0}", PathToContent("packages", "MarkdownSharp.1.13.0.0.nupkg")).ShouldEqual("Unpacked MarkdownSharp.1.13.0.0");

				Directory.Exists(PathToTemp("working", "MarkdownSharp.1.13.0.0")).ShouldBeTrue();
			}
		}
	}
}
