using System;
using MooGet.Options;

namespace MooGet.Commands {

	///<summary></summary>
	public class HelpCommand {

		[Command(Name = "help", Description = "Provide help on the 'moo' command")]
		public static object Run(string[] args) {
			return Util.HelpForCommand("help") + "\n";
		}
	}
}
