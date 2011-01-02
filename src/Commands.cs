// We'll eventually want to move certain complicated commands into their own 
// files but, for now, we put all commands in here ...

using System;

namespace MooGet {
	public partial class Moo {

		[Command("Provide help on the 'moo' command")]
		public static void Help(string[] args) {
			Cow.Say("NuGet + Super Cow Powers = MooGet");
			Console.WriteLine("\nRun moo help for help documentation");
		}

		[Command("Print configuration information")]
		public static void Config(string[] args) {
			Console.WriteLine("mooDir: {0}", Util.HomeDirectory);
		}

		[Command("Unpack a package into the current directory")]
		public static void Unpack(string[] args) {
			Moo.Unpack(args[0]);
		}

		[Command(Name = "commands", Description = "Lists all available Moo commands")]
		public static void ListCommands(string[] args) {
			Console.WriteLine("MOO commands:\n");
			foreach (var command in Commands)
				Console.WriteLine("    {0}{1}{2}", command.Name, Spaces(command.Name, 20), command.Description);
		}

		[Command("Moo.")]
		public static void CowCommand(string[] args) {
			Cow.Say(string.Join(" ", args));
		}

		// helper methods for getting spaces ... useful for commands ...
		static string Spaces(string str, int numSpaces) {
			string spaces = "";
			for (int i = 0; i < numSpaces - str.Length; i++)
				spaces += " ";
			return spaces;
		}
	}
}
