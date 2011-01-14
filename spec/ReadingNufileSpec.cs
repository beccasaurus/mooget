using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	// TODO rename to just NufileSpec
	[TestFixture]
	public class ReadingNufileSpec : MooGetSpec {

		[Test][Ignore]
		public void can_put_source_in_Nufile_using_full_url() {
		}

		[Test][Ignore]
		public void can_put_source_in_Nufile_using_shortcut_for_well_known_sources() {
		}

		[Test][Ignore]
		public void can_put_many_sources_in_Nufile() {
		}
	
		[Test]
		public void example_1() {
			var nufile = new Nufile(PathToContent("nufile_examples", "Nufile1"));
			nufile.Groups.Count.ShouldEqual(0);
			nufile.GlobalDependencies.Count.ShouldEqual(1);
			nufile.GlobalDependencies.First().Text.ShouldEqual("log4net");
		}
	
		[Test]
		public void example_2() {
			var nufile = new Nufile(PathToContent("nufile_examples", "Nufile2"));
			nufile.GlobalDependencies.Count.ShouldEqual(0);
			nufile.Groups.Count.ShouldEqual(2);
			nufile["src"].Dependencies.Count.ShouldEqual(1);
			nufile["src"].Dependencies.First().Text.ShouldEqual("ForSource");
			nufile["spec"].Dependencies.Count.ShouldEqual(2);
			nufile["spec"].Dependencies.Select(d => d.Text).ShouldContain("ForSpecs1");
			nufile["spec"].Dependencies.Select(d => d.Text).ShouldContain("ForSpecs2");
		}
	
		[Test]
		public void example_3() {
			var nufile = new Nufile(PathToContent("nufile_examples", "Nufile3"));

			nufile.GlobalDependencies.Count.ShouldEqual(4);
			nufile.GlobalDependencies.Select(d => d.Text).ShouldContain("these");
			nufile.GlobalDependencies.Select(d => d.Text).ShouldContain("are");
			nufile.GlobalDependencies.Select(d => d.Text).ShouldContain("for");
			nufile.GlobalDependencies.Select(d => d.Text).ShouldContain("all");

			nufile.Groups.Count.ShouldEqual(2);
			nufile["Foo\\Foo.csproj"].Dependencies.Count.ShouldEqual(1);
			nufile["Foo\\Foo.csproj"].Dependencies.First().Text.ShouldEqual("this-one");
			nufile["Folder"].Dependencies.Count.ShouldEqual(1);
			nufile["Folder"].Dependencies.Select(d => d.Text).ShouldContain("that.one");
		}
	
		[Test]
		public void example_4() {
			var nufile = new Nufile(PathToContent("nufile_examples", "Nufile4"));

			nufile.GlobalDependencies.Count.ShouldEqual(2);
			nufile.GlobalDependencies.Select(d => d.Text).ShouldContain("this");
			nufile.GlobalDependencies.Select(d => d.Text).ShouldContain("that > 1.0");
			// PackageDependency needs to be updated to support inclusive/exclusive min/max versions ... TODO
			//var dep = nufile.GlobalDependencies.First(d => d.Text == "this > 1.0").PackageDependency;
			//dep.Id.ShouldEqual("this");
			//dep.

			nufile.Groups.Count.ShouldEqual(1);
			nufile["spec"].Dependencies.Count.ShouldEqual(1);
			nufile["spec"].Dependencies.First().Text.ShouldEqual("NUnit	>1.0	<=	2.5.6.7");
			// TODO check the PackageDependency this generates ...
		}

		[Test]
		public void example_7() {
			var nufile = new Nufile(PathToContent("nufile_examples", "Nufile7"));

			nufile.GlobalDependencies.Count.ShouldEqual(6);
			foreach (var globalDependency in new string[] { "forAll 1.0.2.5", "some", "global", "more", "global", "notthis" })
				nufile.GlobalDependencies.Select(d => d.Text).ToArray().ShouldContain(globalDependency);

			nufile.Groups.Count.ShouldEqual(3);
			nufile.Groups.Find(g => g.Name == "src").Dependencies.Count.ShouldEqual(3);
			nufile.Groups.Find(g => g.Name == "src").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec1");
			nufile.Groups.Find(g => g.Name == "src").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec_2");
			nufile.Groups.Find(g => g.Name == "src").Dependencies.Select(d => d.Text).ShouldContain("Just.Source");
			nufile.Groups.Find(g => g.Name == "spec").Dependencies.Count.ShouldEqual(3);
			nufile.Groups.Find(g => g.Name == "spec").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec1");
			nufile.Groups.Find(g => g.Name == "spec").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec_2");
			nufile.Groups.Find(g => g.Name == "spec").Dependencies.Select(d => d.Text).ShouldContain("Just-Spec");

			nufile.Configuration["source"].Trim().ShouldEqual("this is for 3 configuration sections\nmany lines\nof stuff\n-t:library");
			nufile.Configuration["spec\\Web"].Trim().ShouldEqual("this is for 3 configuration sections\n /p:Configuration=Release /out:bin\\Debug\\Foo.Web.Specs.dll");
			nufile.Configuration["another"].Trim().ShouldEqual("this is for 3 configuration sections");
			nufile.Configuration["something-else"].Trim().ShouldEqual("many lines\nof stuff");
			nufile.Configuration["moo-sources"].Trim().ShouldEqual("nuget, mooget");
			nufile.Configuration["spec\\Foo"].Trim().ShouldEqual("/out:bin\\$config\\Foo.dll");
			nufile.Configuration["has-inner"].Trim().ShouldEqual("this line\nthis line\nall these");
			nufile.Configuration.Count.ShouldEqual(8);
		}
	}
}
