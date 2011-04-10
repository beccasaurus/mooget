using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IO.Interfaces;

// For now, this is miscellaneous extension methods ... we'll split it out into more files, as needed

namespace Clide.Extensions {

	public static class StringExtensions {

		/// <summary>Given this string, return a new Guid.  This supports the "{ABC...123}" format used in sln/csproj/etc files.</summary>
		public static Guid ToGuid(this string guid) {
			var str = guid.TrimStart('{').TrimEnd('}'); // in sln and csproj files, guids are often wrapped with curly braces
			if (str.Length != 36) throw new Exception(string.Format("This doesn't look like a valid GUID, it's not 36 characters: {0}", str));
			return new Guid(str);
		}

		/// <summary>Simply returns the string surrounded by double quotes</summary>
		public static string Quoted(this string str) { return "\"" + str + "\""; }

		/// <summary>Simple returns the string surrounded by curly braces</summary>
		public static string WithCurlies(this string str) { return "{" + str + "}"; }

		/// <summary>Simple returns the string surrounded by curly braces, surrounded by double quotes</summary>
		public static string QuotedWithCurlies(this string str) { return str.WithCurlies().Quoted(); }

		/// <summary>Pads the end of the string with (num spaces - str.length) spaces.  Useful for formatting console output.</summary>
		public static string WithSpaces(this string str, int numSpaces) {
			return string.Format("{0}{1}", str, str.Spaces(numSpaces));
		}   

		/// <summary>Gets *just* the spaces for WithSpaces</summary>
		public static string Spaces(this string str, int numSpaces) {
			string spaces = ""; 
			for (int i = 0; i < numSpaces - (str.SafeString()).Length; i++)
				spaces += " ";
			return spaces;
		}

		/// <summary>If the object isn't null, we return ToString(), else we return an empty string</summary>
		public static string SafeString(this object o) {
			return (o == null) ? string.Empty : o.ToString();
		}
	}

	public static class StringBuilderExtensions {

		/// <summary>Append a blank new line (using Environment.NewLine) to this StringBuilder</summary>
		public static StringBuilder AppendLine(this StringBuilder builder) {
			builder.Append(Environment.NewLine);
			return builder;
		}

		/// <summary>Append a formatted line (using AppendFormat) to this StringBuilder, then add a newline</summary>
		public static StringBuilder AppendLine(this StringBuilder builder, string message, params object[] objects) {
			builder.AppendFormat(message, objects);
			builder.AppendLine();
			return builder;
		}
	}

	public static class NullableGuidExtensions {

		/// <summary>Simply returns a uppercase Guid surrounded by double quotes and curly braces, as sln/csproj files like to do</summary>
		/// <remarks>
		/// eg. "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
		/// </remarks>
		public static string QuotedWithCurlies(this Guid? guid) { return guid.ToString().ToUpper().QuotedWithCurlies(); }

		/// <summary>Simply returns an uppercase Guid surrounded by curly braces</summary>
		public static string WithCurlies(this Guid? guid) { return guid.ToString().ToUpper().WithCurlies(); }
	}
}
