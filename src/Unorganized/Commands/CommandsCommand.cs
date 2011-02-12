using System;
using System.Linq;
using System.Text;
using MooGet.Options;

namespace MooGet.Commands {

	///<summary></summary>
	public class CommandsCommand {

		[Command(Name = "commands", Description = "Lists all available Moo commands")]
		public static object Run(string[] args) {
			var response = new StringBuilder();
			response.AppendLine("MOO commands:\n");
			var commands = Moo.Commands;

			// If you don't pass --debug, don't list the Debug commands
			if (! Moo.Debug)
				commands = commands.Where(cmd => cmd.Debug == false).ToList();

			foreach (var command in commands)
				response.AppendFormat("    {0}{1}{2}\n", command.Name, Util.Spaces(command.Name, 20), command.Description);
			return response;
		}
	}
}
