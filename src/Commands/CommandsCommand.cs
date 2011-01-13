using System;
using System.Text;
using MooGet.Options;

namespace MooGet.Commands {

	///<summary></summary>
	public class CommandsCommand {

		[Command(Name = "commands", Description = "Lists all available Moo commands")]
		public static object Run(string[] args) {
			var response = new StringBuilder();
			response.AppendLine("MOO commands:\n");
			foreach (var command in Moo.Commands)
				response.AppendFormat("    {0}{1}{2}\n", command.Name, Util.Spaces(command.Name, 20), command.Description);
			return response;
		}
	}
}
