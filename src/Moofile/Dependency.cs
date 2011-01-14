using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {
	public partial class Moofile {

		/// <summary>A MooFile Dependency typically refers to a PackageDependency, but may be a dll, csproj file, etc</summary>
		public class Dependency {

			// TODO add reference to Moofile
			// TODO add IsDll IsDirectory IsNupkg predicates

			public Dependency() {}
			public Dependency(string text) {
				Text = text;
			}

			/// <summary>Raw text value of the dependency</summary>
			public string Text { get; set; }

			/// <summary>Reference back to this Dependency's Moofile</summary>
			public Moofile Moofile { get; set; }

			/// <summary>Reference back to this Dependency's Moofile.Group</summary>
			//public MooGet.Moofile.Group Group { get; set; }
		}
	}
}
