using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MooGet {
	public static class ObjectExtensions {
		public static T DeepClone<T>(this T o) {
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream()) {
				formatter.Serialize(stream, o);
				stream.Position = 0;
				return (T) formatter.Deserialize(stream);
			}
		}
	}
}
