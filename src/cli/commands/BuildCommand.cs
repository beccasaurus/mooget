using System;
using System.IO;

namespace MooGet.Commands {

	/// <summary>moo build</summary>
	public class BuildCommand {

		[Command(Name = "build", Description = "Compiles the current project (using Moofile)")]
		public static object Run(string[] args) {
			var moofilePath = Path.Combine(Directory.GetCurrentDirectory(), "Moofile");
			if (File.Exists(moofilePath))
				return new Moofile(moofilePath).Build();
			else
				return "Moofile not found in current directory.  Don't know what to build.";
		}
	}
}
