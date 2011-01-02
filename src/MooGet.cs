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
					case "unpack":
						Moo.Unpack(args[1]);
						break;
					case "gui-test":
						System.Windows.Forms.MessageBox.Show("moo");
						break;
					case "embedded-stuff":
						foreach (string name in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
							Console.WriteLine(name);
							string text = "";
							using (var reader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name)))
								text = reader.ReadToEnd();
							Console.WriteLine(text);
						}
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
