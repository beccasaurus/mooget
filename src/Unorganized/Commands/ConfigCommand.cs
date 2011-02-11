using System;

namespace MooGet.Commands {

	///<summary></summary>
	public class ConfigCommand {

		[Command(Name = "config", Description = "Print configuration information")]
		public static object Run(string[] args) {
			return string.Format(@"
mooDir: {0}
Debug: {1}
",
Moo.Dir, Moo.Debug).TrimStart('\n');
		}
	}
}
