using System;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Helpers for lazy initialization of variables</summary>
	/// <remarks>
	///
	/// Instead of having to say:
	/// <code>
	///		if (foo == null)
	///			foo = new Bar();
	///	     return foo;
	/// </code>
	///
	/// You can simply say: <c>return this.Lazy(ref foo);</c>
	/// </remarks>
	public static class LazyExtensions {

		public static List<T> Lazy<T>(this object self, ref List<T> list) {
			if (list == null) list = new List<T>();
			return list;
		}
	}
}
