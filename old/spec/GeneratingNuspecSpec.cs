using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class GeneratingNuspecSpec : MooGetSpec {

		[Test]
		public void uses_directory_name_if_called_with_no_arguments() {
			var nuspecPath = PathToTemp("working", "working.nuspec");
			File.Exists(nuspecPath).ShouldBeFalse();

			moo("gen nuspec").ShouldContain("Generated working.nuspec");

			File.Exists(nuspecPath).ShouldBeTrue();
			var package = Package.FromSpec(nuspecPath);
			package.Id.ShouldEqual("working");
			package.Description.ShouldEqual("");
			package.VersionText.ShouldEqual("1.0.0.0");
			package.Authors.Count.ShouldEqual(1);
			package.Authors.First().ShouldEqual("me");
			package.Created.ToString("d").ShouldEqual(DateTime.Now.ToString("d"));
			package.Modified.ToString("d").ShouldEqual(DateTime.Now.ToString("d"));
		}
	}
}
