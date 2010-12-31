using System;

namespace MooGet {

	/// <summary>The command line runner</summary>
	public class Runner {

		// We'll refactor to using a nice option parser when we need to ... we'll keep is stupid simple until then
		public static void Main(string[] args) {
			if (args.Length == 0)
				Console.WriteLine("moo");
			else {
				switch (args[0]) {
					case "config":
						PrintConfig();
						break;
					default:
						Console.WriteLine("Unknown command: {0}", args[0]);
						break;
				}
			}
		}

		static void PrintConfig() {
			Console.WriteLine("mooDir: {0}", Util.HomeDirectory);
		}
	}
}
