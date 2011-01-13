// Our Moo.* command filters

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MooGet {

	public partial class Moo {

		[CommandFilter("Prints the Moo splash screen if no arguments passed")]
		public static object SplashScreenFilter(string[] args, CommandFilter filter) {
			if (args.Length == 0)
				return Cow.SayText("NuGet + Super Cow Powers = MooGet") + "\n\nRun moo help for help documentation\n";
			else
				return filter.Invoke(args);
		}

		[CommandFilter("Prints Moo version in response to -v/--version")]
		public static object MooVersionFilter(string[] args, CommandFilter filter) {
			if (args.Length == 1)
				if (args[0] == "-v" || args[0] == "--version")
					return Moo.Version + "\n";
			return filter.Invoke(args);
		}

		[CommandFilter("Finds and runs the appropriate [Command] passed to moo.exe")]
		public static object CommandRunnerFilter(string[] args, CommandFilter filter) {
			return FindAndRunCommand(args);
		}
	}
}
