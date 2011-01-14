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
			Console.WriteLine(moo("build"));

			File.Exists(dll).ShouldBeTrue();
		}
	}
}
