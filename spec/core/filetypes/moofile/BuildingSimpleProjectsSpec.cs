using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.Core {

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

		[Test]
		public void exe_with_global_reference_to_lib_dll() {
			var dir = PathToProject("exe-with-global-dll-reference");
			var exe = PathToProject("exe-with-global-dll-reference", "bin", "dog-cli.exe");
			var bin = Path.Combine(dir, "bin");
			if (Directory.Exists(bin)) Directory.Delete(bin, true);

			File.Exists(exe).ShouldBeFalse();

			cd(dir);
			Console.WriteLine(moo("build"));

			File.Exists(exe).ShouldBeTrue();
			run(exe).ShouldEqual("Dog bark:\nWoof! My name is Rover");
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

		[Test]
		public void exe_with_reference_to_nupkg() {
			var dir = PathToProject("exe-with-nupkg-reference");
			var exe = PathToProject("exe-with-nupkg-reference", "bin", "cli.exe");
			var bin = Path.Combine(dir, "bin");
			if (Directory.Exists(bin)) Directory.Delete(bin, true);

			File.Exists(exe).ShouldBeFalse();

			cd(dir);
			moo("build");

			run(exe).ShouldEqual("Calling lib:\nHello from MyLib.Foo.Bar!");

			File.Exists(exe).ShouldBeTrue();
		}

		// Moofile
		//
		// src
		//     MyDogs from ../../Foo.nupkg
		[Test][Ignore]
		public void exe_with_reference_to_1_particular_assembly_inside_of_nupkg() {
		}

		// Moofile
		//
		// src
		//     MyDogs.dll,MyCats,foo.exe from ../../Foo.nupkg
		[Test][Ignore]
		public void exe_with_reference_to_many_particular_assemblies_inside_of_nupkg() {
		}

		// For this to work, we need to implement 'moo install' for Moofile's.
		// These should install .nupkg as well as packages by name using the 
		// source(s) provided in the Moofile and should create a Moofile.lock 
		// with the *exact* version numbers of all of the installed packages being used.
		[TestFixture]
		public class exe_with_reference_to_package_name {

			[Test][Ignore]
			public void no_sources_defined_in_Moofile() {
			}

			[Test][Ignore]
			public void moo_install_has_not_been_run() {
			}

			[Test][Ignore]
			public void ok() {
			}
		}
	}
}
