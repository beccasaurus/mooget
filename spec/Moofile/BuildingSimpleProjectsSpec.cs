using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class BuildingSimpleProjectsSpec : MooGetSpec {

		string PathToProject(params string[] relative) {
			var paths = new List<string> { "moofile_projects", "explicit_configuration" };
			paths.AddRange(relative);
			return PathToContent(paths.ToArray());
		}

		[Test]
		public void just_one_dll() {
			var dir = PathToProject("just-one-dll");
			var dll = PathToProject("just-one-dll", "bin", "my-code.dll");
			var bin = Path.Combine(dir, "bin");
			if (Directory.Exists(bin)) Directory.Delete(bin, true);

			File.Exists(dll).ShouldBeFalse();

			cd(dir);
			moo("build");

			File.Exists(dll).ShouldBeTrue();
		}

		[Test]
		public void just_one_exe() {
			var dir = PathToProject("just-one-exe");
			var exe = PathToProject("just-one-exe", "bin", "my-command.exe");
			var bin = Path.Combine(dir, "bin");
			if (Directory.Exists(bin)) Directory.Delete(bin, true);

			File.Exists(exe).ShouldBeFalse();

			cd(dir);
			moo("build");

			run(exe).ShouldEqual("hi");

			File.Exists(exe).ShouldBeTrue();
		}

		[Test][Ignore]
		public void exe_with_global_reference_to_lib_dll() {
		}

		[Test]
		public void exe_with_reference_to_lib_dll() {
			var dir = PathToProject("exe-with-dll-reference");
			var exe = PathToProject("exe-with-dll-reference", "bin", "dog-cli.exe");
			var bin = Path.Combine(dir, "bin");
			if (Directory.Exists(bin)) Directory.Delete(bin, true);

			File.Exists(exe).ShouldBeFalse();

			cd(dir);
			moo("build");

			run(exe).ShouldEqual("Dog bark:\nWoof! My name is Rover");

			File.Exists(exe).ShouldBeTrue();
		}
	}
}
