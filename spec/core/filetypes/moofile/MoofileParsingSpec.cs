using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class MoofileParsingSpec : MooGetSpec {

		[Test][Ignore]
		public void can_put_source_in_Moofile_using_full_url() {
		}

		[Test][Ignore]
		public void can_put_source_in_Moofile_using_shortcut_for_well_known_sources() {
		}

		[Test][Ignore]
		public void can_put_many_sources_in_Moofile() {
		}
	
		[Test]
		public void example_1() {
			var moofile = new Moofile(PathToContent("moofile_examples", "Moofile1"));
			moofile.Groups.Count.ShouldEqual(0);
			moofile.GlobalDependencies.Count.ShouldEqual(1);
			moofile.GlobalDependencies.First().Text.ShouldEqual("log4net");
		}
	
		[Test]
		public void example_2() {
			var moofile = new Moofile(PathToContent("moofile_examples", "Moofile2"));
			moofile.GlobalDependencies.Count.ShouldEqual(0);
			moofile.Groups.Count.ShouldEqual(2);
			moofile["src"].Dependencies.Count.ShouldEqual(1);
			moofile["src"].Dependencies.First().Text.ShouldEqual("ForSource");
			moofile["spec"].Dependencies.Count.ShouldEqual(2);
			moofile["spec"].Dependencies.Select(d => d.Text).ShouldContain("ForSpecs1");
			moofile["spec"].Dependencies.Select(d => d.Text).ShouldContain("ForSpecs2");
		}
	
		[Test]
		public void example_3() {
			var moofile = new Moofile(PathToContent("moofile_examples", "Moofile3"));

			moofile.GlobalDependencies.Count.ShouldEqual(4);
			moofile.GlobalDependencies.Select(d => d.Text).ShouldContain("these");
			moofile.GlobalDependencies.Select(d => d.Text).ShouldContain("are");
			moofile.GlobalDependencies.Select(d => d.Text).ShouldContain("for");
			moofile.GlobalDependencies.Select(d => d.Text).ShouldContain("all");

			moofile.Groups.Count.ShouldEqual(2);
			moofile["Foo\\Foo.csproj"].Dependencies.Count.ShouldEqual(1);
			moofile["Foo\\Foo.csproj"].Dependencies.First().Text.ShouldEqual("this-one");
			moofile["Folder"].Dependencies.Count.ShouldEqual(1);
			moofile["Folder"].Dependencies.Select(d => d.Text).ShouldContain("that.one");
		}
	
		[Test]
		public void example_4() {
			var moofile = new Moofile(PathToContent("moofile_examples", "Moofile4"));

			moofile.GlobalDependencies.Count.ShouldEqual(2);
			moofile.GlobalDependencies.Select(d => d.Text).ShouldContain("this");
			moofile.GlobalDependencies.Select(d => d.Text).ShouldContain("that > 1.0");
			// PackageDependency needs to be updated to support inclusive/exclusive min/max versions ... TODO
			//var dep = moofile.GlobalDependencies.First(d => d.Text == "this > 1.0").PackageDependency;
			//dep.Id.ShouldEqual("this");
			//dep.

			moofile.Groups.Count.ShouldEqual(1);
			moofile["spec"].Dependencies.Count.ShouldEqual(1);
			moofile["spec"].Dependencies.First().Text.ShouldEqual("NUnit	>1.0	<=	2.5.6.7");
			// TODO check the PackageDependency this generates ...
		}

		[Test]
		public void example_7() {
			var moofile = new Moofile(PathToContent("moofile_examples", "Moofile7"));

			moofile.GlobalDependencies.Count.ShouldEqual(6);
			foreach (var globalDependency in new string[] { "forAll 1.0.2.5", "some", "global", "more", "global", "notthis" })
				moofile.GlobalDependencies.Select(d => d.Text).ToArray().ShouldContain(globalDependency);

			moofile.Groups.Count.ShouldEqual(3);
			moofile.Groups.Find(g => g.Name == "src").Dependencies.Count.ShouldEqual(3);
			moofile.Groups.Find(g => g.Name == "src").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec1");
			moofile.Groups.Find(g => g.Name == "src").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec_2");
			moofile.Groups.Find(g => g.Name == "src").Dependencies.Select(d => d.Text).ShouldContain("Just.Source");
			moofile.Groups.Find(g => g.Name == "spec").Dependencies.Count.ShouldEqual(3);
			moofile.Groups.Find(g => g.Name == "spec").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec1");
			moofile.Groups.Find(g => g.Name == "spec").Dependencies.Select(d => d.Text).ShouldContain("JustForSrcAndSpec_2");
			moofile.Groups.Find(g => g.Name == "spec").Dependencies.Select(d => d.Text).ShouldContain("Just-Spec");

			moofile.Configuration["source"].Trim().ShouldEqual("this is for 3 configuration sections\nmany lines\nof stuff\n-t:library");
			moofile.Configuration["spec\\Web"].Trim().ShouldEqual("this is for 3 configuration sections\n /p:Configuration=Release /out:bin\\Debug\\Foo.Web.Specs.dll");
			moofile.Configuration["another"].Trim().ShouldEqual("this is for 3 configuration sections");
			moofile.Configuration["something-else"].Trim().ShouldEqual("many lines\nof stuff");
			moofile.Configuration["moo-sources"].Trim().ShouldEqual("nuget, mooget");
			moofile.Configuration["spec\\Foo"].Trim().ShouldEqual("/out:bin\\$config\\Foo.dll");
			moofile.Configuration["has-inner"].Trim().ShouldEqual("this line\nthis line\nall these");
			moofile.Configuration.Count.ShouldEqual(8);
		}
	}
}
