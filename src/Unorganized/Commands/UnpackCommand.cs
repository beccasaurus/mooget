using System;

namespace MooGet.Commands {

	///<summary></summary>
	public class UnpackCommand {

		[Command(Name = "unpack", Description = "Unpack a package into the current directory")]
		public static object Run(string[] args) {
			var package = Moo.Unpack(args[0]);
			return string.Format("Unpacked {0}", package.IdAndVersion);
		}
	}
}
