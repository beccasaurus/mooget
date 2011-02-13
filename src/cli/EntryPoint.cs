using System;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Class that simply holds the Main() methods for moo.exe</summary>
	public class EntryPoint {

		/// <summary>moo.exe Entry method</summary>
		/// <remarks>
		///	moo.exe runs by running all [CommandFilter] methods defined 
		///	in moo.exe or in any installed packages that have libraries 
		///	named MooGet.*.dll
		///
		///	The Moo.CommandRunnerFilter is meant to be the last filter that 
		///	gets run, as it finds and runs all [Command] methods defined 
		///	in moo.exe or in any installed packages that have libraries 
		///	named MooGet.*.dll and runs them.
		///
		///	Moo.Filters returns the full List&lt;CommandFilter&gt; that moo.exe runs.
		/// </remarks>
		public static void Main(string[] args) {

			// Setup all of our CommandFilter, which is what we run.
			var filters = new List<CommandFilter>(CommandFilter.Filters);
			for (var i = 0; i < filters.Count - 1; i++)
				filters[i].InnerFilter = filters[i + 1];

			try {

				if (Moo.Verbose)
					Moo.Log.Debug("Invoking top-most filter: {0}", filters[0].Name);

				// Invoke our full stack of filters and 
				// write the final string response out 
				// to the console.
				Console.Write(filters[0].Invoke(args));

			} catch (Exception ex) {
				// Use CowSay to display the uncaught exception
				Cow.Columns = 80;
				Cow.Say("Moo. There was a problem:                                                   {0}", ex.GetNonInvokationException());
			}
		}
	}
}
