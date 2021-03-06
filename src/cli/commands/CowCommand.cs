using System;

namespace MooGet.Commands {

	/// <summary>moo cow</summary>
	public class CowCommand {

		[Command("cow", "Moo.", Debug = true)]
		public static object Run(string[] args) {
			if (args.Length == 0)
				args = new string[] { "Moo" };
			return Cow.SayText(string.Join(" ", args)) + "\n";
		}
	}
}
