using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class SpecCommandSpec : Spec {

		[Test][Description("moo help spec")]
		public void help() {
			moo("help spec").ShouldContain("Usage: moo spec");
		}

		// For now, ALL this does is make a silly stub ...
		[Test][Description("moo spec")][Ignore]
		public void moo_spec_generates_stub_nuspec() {
			File.Exists(PathToTemp("working", "working.nuspec")).Should(Be.False);

			System.Console.WriteLine(moo("spec"));

			var nuspec = new Nuspec(PathToTemp("working", "working.nuspec"));
			nuspec.Exists().Should(Be.True);
			nuspec.ShouldHaveProperties(new {
				Id          = "working",
				VersionText = "1.0.0",
				AuthorsText = "me <me@email.com>",
				Description = "About working",
				ProjectUrl  = "https://github.com/me/working"
			});

			nuspec.Dependencies.Count.ShouldEqual(1); // add 1 as an example

			// There should be example <file> elements for each of the common package parts
			var targets = nuspec.FileSources.Select(source => source.Target).ToArray();
			targets.ShouldContain("lib");
			targets.ShouldContain("content");
			targets.ShouldContain("tools");
			targets.ShouldContain("src");
		}
	}
}
