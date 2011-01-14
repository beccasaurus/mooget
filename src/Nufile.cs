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

		public Dictionary<string, string> Configuration      { get; set; }
		public List<Dependency>           GlobalDependencies { get; set; }
		public List<Group>                Groups             { get; set; }

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
			Groups              = new List<Group>();
			GlobalDependencies  = new List<Dependency>();
			Configuration       = new Dictionary<string, string>();
			var lines           = Text.Trim().Split('\n');
			var sections        = new Dictionary<string, List<string>>(); // track indentations
			sections["global"]  = new List<string>();
			string currentGroup = null;

			for (int i = 0; i < lines.Length; i++) {
				var line     = lines[i];
				var nextLine = ((i+1) >= lines.Length) ? null : lines[i + 1];

				// ignore empty lines
				if (line.Trim().Length == 0)
					continue;

				// ignore comments
				if (line.StartsWith("#") || line.StartsWith("//"))
					continue;

				if (LineIndented(line))
					sections[currentGroup].Add(line.Trim());
				else if (LineIndented(nextLine)) {
					currentGroup = line.Trim();
					sections[currentGroup] = new List<string>();
				} else
					sections["global"].Add(line);
			}

			// before adding sections as Configuration, Dependencies, or GlobalDependencies 
			// find everything key with a comma and split it up so src, spec: foo turns into src: foo and spec: foo
			foreach (var section in new Dictionary<string, List<string>>(sections)) {

				// For sections that have commas, we split up the section name and we take all of the dependecies 
				// and add them to ALL of the groups specified
				if (section.Key.Contains(",")) {
					if (section.Key.Contains(":")) {
						foreach (var part in section.Key.Split(',')) {
							var key = part.Replace(":", "").Trim();
							if (! Configuration.ContainsKey(key)) Configuration[key] = "";
							foreach (var value in section.Value)
								Configuration[key] += "\n" + value;
						}
						sections.Remove(section.Key);
					} else {
						foreach (var part in section.Key.Split(',')) {
							var key = part.Trim();
							var group = Groups.FirstOrDefault(g => g.Name == key);
							if (group == null) {
								group = new Group(key);
								Groups.Add(group);
							}
							foreach (var value in section.Value)
								group.Dependencies.Add(new Dependency(value));
						}
						sections.Remove(section.Key); // we processed this into groups ourselves
					}
				}

				// For the global section we look at all of the global values and, if they have a comma and a colon, 
				// we add the configuration information to the configuration for all of the comma separated values
				if (section.Key == "global") {
					foreach (var item in section.Value) {
						if (item.Contains(",")) {
							
							// Configuration
							if (item.Contains(":")) {
								// if there is a :, we only split this up if the , is BEFORE the first :
								// That's because the comma is part of the VALUE of this configuration
								var colonIndex = item.IndexOf(":");
								var commaIndex = item.IndexOf(",");
								if (colonIndex > -1 && commaIndex > colonIndex) continue;

								// Everything after the colon is the value
								var value = item.Substring(colonIndex + 1);

								// take all of the parts before the colon and set their configuration values
								foreach (var part in item.Substring(0, colonIndex).Split(',')) {
									var key = part.Trim();
									if (! Configuration.ContainsKey(key)) Configuration[key] = "";
									Configuration[key] += "\n" + value;
								}
							} else {
								Console.WriteLine("Global section has commas but no colon.  I don't understand:  {0}", item);
							}
						}
					}
				}
			}

			// Process sections and import them into GlobalDependencies, Groups, and Configuration
			foreach (var section in sections) {
				if (section.Key == "global") {
					foreach (var dependencyText in section.Value) {
						if (dependencyText.Contains(":"))
							AddToConfigurationFromString(Configuration, dependencyText);
						else
							GlobalDependencies.Add(new Dependency(dependencyText));
					}
				} else {
					if (section.Key.Contains(":")) {
						Configuration.Add(section.Key.Replace(":", ""), string.Join("\n", section.Value.ToArray()));
					} else {
						var group = Groups.FirstOrDefault(g => g.Name == section.Key);
						if (group == null) {
							group = new Group(section.Key);
							Groups.Add(group);
						}
						foreach (var dependencyText in section.Value)
							group.Dependencies.Add(new Dependency(dependencyText));
					}
				}
			}
		}

		// takes foo: bar and sets config["foo"] += "\n" + "bar"
		static void AddToConfigurationFromString(Dictionary<string, string> config, string text) {
			var parts = new List<string>(text.Split(':'));
			var key   = parts.First();
			parts.RemoveAt(0);
			var value = string.Join(":", parts.ToArray());
			if (! config.ContainsKey(key)) config[key] = "";
			config[key] += "\n" + value;
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
