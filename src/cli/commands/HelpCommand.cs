using System;
using System.Linq;
using MooGet.Options;

namespace MooGet.Commands {

	///<summary>moo help</summary>
	public class HelpCommand {

		[Command(Name = "help", Description = "Provide help on the 'moo' command")]
		public static object Run(string[] args) {

			// TODO clean up!  once we have more commands ...

			if (args.Length == 0) {
				return Util.HelpForCommand("moo");
			} else if (args.Length == 1 && args[0] == "--help") {
				return Util.HelpForCommand("help");
			} else {
				var command = Moo.Commands.FirstOrDefault(cmd => cmd.Name == args[0]);
				if (command == null)
					return string.Format("Command not found: {0}\n", args[0]);
				else
					return command.Run(new string[]{ "--help" });
			}
		}
	}
}
