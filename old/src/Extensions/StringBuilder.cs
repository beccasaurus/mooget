using System;
using System.Text;

namespace MooGet {

	/// <summary>Helpful StringBuilder extensions for making it easier to build responses</summary>
	public static class StringBuilderExtensions {
		
		public static void Line(this StringBuilder builder, string str, params object[] o) {
			builder.AppendLine(string.Format(str, o));
		}

		public static void Indent(this StringBuilder builder, int times, string str, params object[] o) {
			for (int i = 0; i < times; i++) builder.Append(Moo.Indentation);
			builder.Line(str, o);
		}
		
		public static void Indent(this StringBuilder builder, string str, params object[] o) {
			builder.Indent(1, str, o);
		}
	}
}
