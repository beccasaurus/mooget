using System;
using System.IO;

namespace MooGet {

	/// <summary>Represents the primary API for most MooGet actions</summary>
	public class Moo {

		public static void Unpack(string nupkg) {
			Unpack(nupkg, Directory.GetCurrentDirectory());
		}

		public static void Unpack(string nupkg, string directoryToUnpackInto) {
			//
			//if (! File.Exists(nupkg)) { TODO test
			//
			//}
			var packageName = Path.GetFileNameWithoutExtension(nupkg);
			Util.Unzip(nupkg, Path.Combine(directoryToUnpackInto, packageName));
			Console.WriteLine("Unpacked {0}", packageName);
		}
	}
}
