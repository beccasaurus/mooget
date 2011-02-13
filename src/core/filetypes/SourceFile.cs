using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Wraps the sources.list file that is stored in the root of a MooDir for configuring sources</summary>
	public class SourceFile : IFile {
		public SourceFile() {}
		public SourceFile(string path) : this() {
			Path = path;
		}

		public string Path { get; set; }

		// TODO cache (based on file mtime)
		public List<ISource> Sources {
			get {
				var sources = new List<ISource>();
				if (! this.Exists()) return sources;
				foreach (var line in this.Lines()) {
					var source = LineToSource(line);
					if (source != null)
						sources.Add(source);
				}
				return sources;
			}
		}

		public ISource Add(string path) {
			return Add(null, path);
		}

		public ISource Add(string name, string path) {
			// If we're referencing a system path, we need to save the FULL path in our SourceFile
			if (Directory.Exists(path))
				path = System.IO.Path.GetFullPath(path);

			// we just do this to make sure it looks like a valid source
			// and so we can return it, incase we want it
			var line   = string.Format("{0} {1}", name, path).Trim();
			var source = LineToSource(line);
			if (source != null)
				this.AppendLine(line);
			return source;
		}

		public void Remove(string nameOrPath) {
			var builder = new StringBuilder();

			foreach (var line in this.Lines()) {
				if (line.Trim().StartsWith("#")) {
					builder.AppendLine(line); // ignore comments
					continue;
				}
				if (string.IsNullOrEmpty(line.Trim())) {
					builder.AppendLine(line); // ignore empty lines
					continue;
				}
				var source = LineToSource(line);
				if (source == null) {
					builder.AppendLine(line); // not valid ??? add it back and ignore it
				} else {
					if ((source.Name != null && source.Name == nameOrPath) || source.Path == nameOrPath) {
						// it was a match ... do NOT write this line back to the file!
					} else {
						builder.AppendLine(line); // not a match, write it ...
						continue;
					}
				}
			}

			this.Write(builder.ToString());
		}

		ISource LineToSource(string line) {
			if (line.Trim().StartsWith("#"))       return null; // skip comments
			if (string.IsNullOrEmpty(line.Trim())) return null; // skip empty lines

			var parts  = new List<string>(line.Split(' '));
			var path   = parts.Last(); parts.RemoveAt(parts.Count - 1);
			var name   = parts.Join(" ").Trim();
			var source = Source.ForPath(path);

			if (source != null && ! string.IsNullOrEmpty(name))
				source.Name = name;

			return source;
		}
	}
}
