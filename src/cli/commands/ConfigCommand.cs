using System;

namespace MooGet.Commands {

	///<summary>moo config</summary>
	public class ConfigCommand {

		[Command("config", "Print configuration information", Debug = true)]
		public static object Run(string[] args) {
			if (args.Length == 1 && args[0] == "--help")
				return Util.HelpForCommand("config");

			return string.Format(@"
MooDir:      {0}
Debug:       {1}
Verbose:     {2}
System HOME: {3}
System TMP:  {4}
",
Moo.Dir.Path,
Moo.Debug,
Moo.Verbose,
Util.HomeDirectory,
Util.TempDirectory
).TrimStart('\n');
		}
	}
}
