using System;

namespace MooGet {

	/// <summary>Has methods like ToBool() and ToInt() that safely convert the given object</summary>
	public static class SafeConversionExtensions {

		public static string SafeString(this object o) {
			return (o == null) ? string.Empty : o.ToString();
		}

		public static bool ToBool(this object o) {
			bool b = false;
			bool.TryParse(o.SafeString(), out b);
			return b;
		}
	}
}
