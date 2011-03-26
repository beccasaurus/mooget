using System;
using System.IO;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a &lt;file&gt; element in a Nuspec</summary>
	public class NuspecFileSource {

		public NuspecFileSource(){}
		public NuspecFileSource(Nuspec nuspec) : this() {
			Nuspec = nuspec;
		}

		/// <summary>Refernece to the Nuspec this belongs to</summary>
		public virtual Nuspec Nuspec { get; set; }

		/// <summary>This &lt;file&gt; element's src attribute</summary>
		public virtual string Source { get; set; }

		/// <summary>This &lt;file&gt; element's target attribute</summary>
		public virtual string Target { get; set; }

		/// <summary>If the Source refers to one file, this returns that.  If it has wildcards, it returns all matching files.</summary>
		public virtual List<string> GetFiles(string baseDir) {
			return baseDir.AsDir().Search(Source).Paths();
		}

		public virtual List<string> GetFiles() {
			return GetFiles(Nuspec.DirName());
		}

		public override string ToString() {
			if (! string.IsNullOrEmpty(Target))
				return string.Format("<file src='{0}' target='{1}' />", Source, Target);
			else
				return string.Format("<file src='{0}' />", Source);
		}
	}   
}
