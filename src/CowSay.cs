using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	public class Cow {

		public static int Columns = 38;

		static string CowAscii = @"
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||";

		public static void Say(string text, params object[] o) {
			Console.WriteLine(SayText(string.Format(text, o).Replace("\n", "")));
		}

		public static string SayText(string text) {
			var sb    = new StringBuilder();
			var parts = text.Split(' ');
			var rows  = new List<StringBuilder>();

			// process text into rows
			rows.Add(new StringBuilder());
			foreach (var part in parts) {
				var currentRow = rows.Last();
				if (currentRow.Length > 0 && (currentRow.Length + part.Length) > Columns)
					rows.Add(new StringBuilder(part + " "));
				else
					currentRow.Append(part + " ");
			}

			// get the length of the longest row, uses to set the width of the bubble
			int length = rows.Select(r => r.ToString().Length).Max();

			// print the _______ above the bubble
			sb.Append(" " + Chars('_', length + 1) + "\n");

			for (int i = 0; i < rows.Count; i++) {
				var rowText = rows[i].ToString();
				var isFirst = i == 0;
				var isLast  = i == rows.Count - 1;

				// add character that starts line
				if (rows.Count == 1)
					sb.Append("< ");
				else if (isFirst)
					sb.Append("/ ");
				else if (isLast)
					sb.Append("\\ ");
				else
					sb.Append("| ");

				sb.Append(rowText);
				sb.Append(Chars(' ', length - rowText.Length)); // pad the right to match up with all other lines

				// add character that ends line
				if (rows.Count == 1)
					sb.Append(">");
				else if (isFirst)
					sb.Append("\\");
				else if (isLast)
					sb.Append("/");
				else
					sb.Append("|");

				sb.Append("\n");
			}

			// print the -------- below the bubble
			sb.Append(" " + Chars('-', length + 1));

			sb.Append(CowAscii);

			return sb.ToString();
		}

		static string Chars(char character, int length) {
			string str = "";
			for (int i = 0; i < length; i++)
				str += character;
			return str;
		}
	}
}
