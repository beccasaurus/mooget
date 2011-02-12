using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {
	public partial class Moofile {

		/// <summary>Represents a Group of dependencies for a particular project or directory</summary>
		public class Group {
			List<Dependency> _dependencies = new List<Dependency>();

			// TODO add reference to Moofile
			// TODO add IsDirectory IsProjectName IsProjectFile

			public Group() {}
			public Group(string name) {
				Name = name;
			}
			public Group(string name, List<Dependency> dependencies) {
				Name = name;
				Dependencies = dependencies;
			}

			public string Name { get; set; }

			public List<Dependency> Dependencies {
				get { return _dependencies;  }
				set { _dependencies = value; }
			}
		}
	}
}
