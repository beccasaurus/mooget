using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a Nufile file which specifies NuGet package dependencies for a project or solution</summary>
	public class Nufile {
		string _text;

		public Nufile() {}
		public Nufile(string nufilePath) {
			Text = Util.ReadFile(nufilePath);
		}

		public List<Dependency> GlobalDependencies { get; set; }
		public List<Group>      Groups             { get; set; }

		public Group this[string name] {
			get { return Groups.FirstOrDefault(g => g.Name == name); }
		}

		public string Text {
			get { return _text; }
			set {
				_text = value;
				Parse();
			}
		}

		void Parse() {
			Console.WriteLine("Nufile.Parse()");
			Groups              = new List<Group>();
			GlobalDependencies  = new List<Dependency>();
			var lines           = Text.Trim().Split('\n');
			var sections        = new Dictionary<string, List<string>>(); // track indentations
			sections["global"]  = new List<string>();
			string currentGroup = null;

			for (int i = 0; i < lines.Length; i++) {
				var line     = lines[i];
				var nextLine = ((i+1) >= lines.Length) ? null : lines[i + 1];

				if (line.Trim().Length == 0)
					continue;

				if (LineIndented(line))
					sections[currentGroup].Add(line.Trim());
				else if (LineIndented(nextLine)) {
					currentGroup = line.Trim();
					sections[currentGroup] = new List<string>();
				} else
					sections["global"].Add(line);
			}

			foreach (var section in sections) {
				if (section.Key == "global") {
					foreach (var dependencyText in section.Value)
						GlobalDependencies.Add(new Dependency(dependencyText));
				} else {
					var group = new Group(section.Key);
					foreach (var dependencyText in section.Value)
						group.Dependencies.Add(new Dependency(dependencyText));
					Groups.Add(group);
				}
			}
		}

		static bool LineIndented(string line) {
			if (line == null) return false;
			return line.StartsWith(" ") || line.StartsWith("\t");
		}

		// TODO move this into its own file
		/// <summary>A NuFile Dependency typically refers to a PackageDependency, but may be a dll, csproj file, etc</summary>
		public class Dependency {

			public Dependency() {}
			public Dependency(string text) {
				Text = text;
			}

			/// <summary>Raw text value of the dependency</summary>
			public string Text { get; set; }
		}

		// TODO move this into its own file
		/// <summary>A NuFile Group has a name and typically refers to a project name or directory</summary>
		public class Group {
			List<Dependency> _dependencies = new List<Dependency>();

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
