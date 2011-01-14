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
		}
	}
}
