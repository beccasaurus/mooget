using System;
using System.Linq;
using System.Text;
using Mono.Options;

namespace MooGet.Commands {

	/// <summary>moo commands</summary>
	public class CommandsCommand {

		[Command(Name = "commands", Description = "List all available Moo commands")]
		public static object Run(string[] args) {
			
			// TODO using another command (that takes more arguments) as an example,
			//      we should make a baseclass for making it easy to take arguments 
			//      and return help documentation, etc
			
			if (args.Length == 1 && args[0] == "--help") {
				return Util.HelpForCommand("commands");
			} else {
				var response = new StringBuilder();
				response.AppendLine("MOO commands:\n");
				var commands = Moo.Commands;

				// If you don't pass --debug, don't list the Debug commands
				if (! Moo.Debug)
					commands = commands.Where(cmd => cmd.Debug == false).ToList();

				var spaces = commands.Select(c => c.Name.Length).Max() + 4;
				foreach (var command in commands)
					response.AppendFormat("    {0}{1}\n", command.Name.WithSpaces(spaces), command.Description);
				return response;
			}
		}
	}
}
