using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet {

	/// <summary>A class for holding all of our common MooGet CommandFilter methods</summary>
	/// <remarks>
	/// Once this file gets really big, if it does, then we'll extract these methods 
	/// out into their own classes or files to better organize them.
	/// </remarks>
	public static class Filters {

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

		[CommandFilter("Run moo in debug mode with -D/--debug")]
		public static object MooDebugFilter(string[] args, CommandFilter filter) {
			var arguments = new OptionSet() {{ "D|debug", v => Moo.Debug = true }}.Parse(args);
			return filter.Invoke(arguments.ToArray());
		}

		[CommandFilter("Run moo in verbose mode with -V/--verbose")]
		public static object MooVerboseFilter(string[] args, CommandFilter filter) {
			var arguments = new OptionSet() {{ "V|verbose", v => Moo.Verbose = true }}.Parse(args);
			return filter.Invoke(arguments.ToArray());
		}

		[CommandFilter("Finds and runs the appropriate [Command] passed to moo.exe")]
		public static object CommandRunnerFilter(string[] args, CommandFilter filter) {
			// If, by the time we make it to this CommandRunnerFilter, all of the arguments 
			// have been removed ... display the splash screen.  Else FindAndRunCommand.
			if (args.Length == 0)
				return SplashScreenFilter(args, null);
			else
				return Moo.FindAndRunCommand(args);
		}
	}
}
