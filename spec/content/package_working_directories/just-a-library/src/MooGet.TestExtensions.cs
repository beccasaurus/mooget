using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;

namespace MyLibrary {

	public class JustALibrary {

		[Command("command test")]
		public static object ExtensionTestCommand(string[] args) {
			return "Hello from test command!";
		}

		[CommandFilter("filter test")]
		public static object ExtensionTestCommand(string[] args, CommandFilter command) {
			var arguments = new List<string>(args);
			if (arguments.Contains("--with-header")) {
				arguments.Remove("--with-header");
				return "HEADER\n------\n" + command.Invoke(arguments.ToArray());
			} else
				return command.Invoke(args);
		}
	}
}
