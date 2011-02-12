using System;

namespace MooGet.Commands {

	///<summary>moo config</summary>
	public class ConfigCommand {

		[Command(Name = "config", Description = "Print configuration information")]
		public static object Run(string[] args) {
			return string.Format(@"
mooDir: {0}
Debug: {1}
Verbose: {2}
",
Moo.Dir,
Moo.Debug,
Moo.Verbose).TrimStart('\n');
		}
	}
}
