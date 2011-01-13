using System;

namespace MooGet.Commands {

	public class CowCommand {

		[Command(Name = "cow", Description = "Moo.")]
		public static object Run(string[] args) {
			if (args.Length == 0)
				args = new string[] { "Moo" };
			return Cow.SayText(string.Join(" ", args)) + "\n";
		}
	}
}
