using System;
using System.IO;

namespace MooGet {

	/// <summary>Represents a &lt;file&gt; element in a Nuspec</summary>
	public class NuspecFileSource {

		/// <summary>This &lt;file&gt; element's src attribute</summary>
		public virtual string Source { get; set; }

		/// <summary>This &lt;file&gt; element's target attribute</summary>
		public virtual string Target { get; set; }

		// TODO we'll eventually want a method to actually find the matching files

		public override string ToString() {
			if (! string.IsNullOrEmpty(Target))
				return string.Format("<file src='{0}' target='{1}' />", Source, Target);
			else
				return string.Format("<file src='{0}' />", Source);
		}
	}   
}
