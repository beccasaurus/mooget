using System;
using MooGet.Options;

namespace MooGet.Commands {

	///<summary></summary>
	public class HelpCommand {

		[Command(Name = "help", Description = "Provide help on the 'moo' command")]
		public static void Run(string[] args) {
			if (args.Length == 0)
				Console.WriteLine(Util.HelpForCommand("help"));
			else
				Console.WriteLine("UNSUPPORTED");
		}
	}
}
