using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents a .nuspec file (can have a Path or simply the XML text)</summary>
	/// <remarks>
	///	Nuspec can be used to read and write Nuspec XML and files.
	///
	///	NOTE: there's a gotcha when using Nuspec to write XML.  You cannot modify any of the 
	/// List properties, eg. Dependencies or Authors.  Instead, you must *set* the whole property. 
	/// Inotherwords, you cannot <c>nuspec.Authors.Add("foo")</c> but you can <c>nuspec.Authors = authorsList</c>.
	/// Authors, Owners, and Tags all have helper *Text properties that you can modify freely, however, eg. AuthorsText.
	/// </remarks>
	public class Nuspec : PackageDetails, IPackage, IFile {

		public Nuspec() {}
		public Nuspec(string path) : this() {
			Path = path;
		}

		string      _path;
		XmlDocument _doc;

		/// <summary>This Nuspec's parent IPackage</summary>
		/// <remarks>
		/// Nuspec.Source and Nuspec.Files are delegated to this property, if present.
		/// </remarks>
		public virtual IPackage Package { get; set; }

		/// <summary>Delegated to the Nuspec.Package.Source, if present</summary>
		public virtual ISource Source { get { return (Package == null) ? null : Package.Source; } }

		/// <summary>Delegated to the Nuspec.Package.Files, if present</summary>
		public virtual List<string> Files { get { return (Package == null) ? null : Package.Files; } }

		/// <summary>Implementation of IPackage.Details.  Delegates to this Nuspec instance.</summary>
		public virtual PackageDetails Details { get { return this; } }

		/// <summary>File path to .nuspec file, if present on file system.</summary>
		/// <remarks>When set, this Nuspec's XML is read from the file (if it exists).</remarks>
		public virtual string Path {
			get { return _path; }
			set { _path = value; Xml = Util.ReadFile(value); }
		}

		public virtual  string Id            { get { return GetMeta("id");          } set { SetMeta("id",           value); } }
		public virtual  string VersionText { get { return GetMeta("version");     } set { SetMeta("version",      value); } }
		public override string Title         { get { return GetMeta("title");       } set { SetMeta("title",        value); } }
		public override string Description   { get { return GetMeta("description"); } set { SetMeta("description",  value); } }
		public override string Summary       { get { return GetMeta("summary");     } set { SetMeta("summary",      value); } }
		public override string ProjectUrl    { get { return GetMeta("projectUrl");  } set { SetMeta("projectUrl",   value); } }
		public override string LicenseUrl    { get { return GetMeta("licenseUrl");  } set { SetMeta("licenseUrl",   value); } }
		public override string IconUrl       { get { return GetMeta("iconUrl");     } set { SetMeta("iconUrl",      value); } }
		public override string Language      { get { return GetMeta("language");    } set { SetMeta("language",     value); } }

		public override List<string> Authors {
			get { return TrimmedListForNodes("author") ?? GetMeta("authors").ToTrimmedList(','); }
			set { SetMeta("authors", value.Join(","));                                           }
		}

		public override List<string> Owners {
			get { return GetMeta("owners").ToTrimmedList(','); }
			set { SetMeta("owners", value.Join(","));          }
		}

		public override List<string> Tags {
			get { return GetMeta("tags").ToList(' '); }
			set { SetMeta("tags", value.Join(" "));   }
		}

		public string RequireLicenseAcceptanceString {
			get { return GetMeta("requireLicenseAcceptance"); }
			set { SetMeta("requireLicenseAcceptance", value); }
		}

		public override bool RequiresLicenseAcceptance {
			get { return RequireLicenseAcceptanceString.ToBool();    }
			set { RequireLicenseAcceptanceString = value.ToString(); }
		}

		public virtual PackageVersion Version {
			get { return new PackageVersion(VersionText); }
			set { VersionText = (value == null) ? null : value.ToString(); }
		}

		// TODO test <dependencies> under <package> instead of under <metadata>
		public override List<PackageDependency> Dependencies {
			get {
				var dependencies = new List<PackageDependency>();
				MetaData.Nodes("dependency").ForEach(node => dependencies.Add(node.ToDependency()));
				return dependencies;
			}
			set {
				// remove existing <dependencies>
				var existingNode = MetaData.Node("dependencies");
				if (existingNode != null)
					existingNode.ParentNode.RemoveChild(existingNode);

				if (value == null || value.Count == 0) return;

				// add new <dependencies> and new <dependency> nodes underneath it
				var newNode = MetaData.NodeOrNew("dependencies");
				foreach (var dependency in value) {
					var node = newNode.NewNode("dependency");
					node.Attr("id", dependency.Id);
					if (dependency.Versions.Count > 0)
						node.Attr("version", dependency.VersionsString);
				}
			}
		}

		/// <summary>The &lt;file&gt; elements in this Nuspec, if any.</summary>
		public virtual List<NuspecFileSource> FileSources {
			get {
				var fileSources = new List<NuspecFileSource>();
				Doc.Nodes("file").ForEach(node => {
					var source = node.ToFileSource();
					source.Nuspec = this;
					fileSources.Add(source);
				});
				return fileSources;
			}
			set {}
		}

		#region Xml Stuff
		public virtual string Xml {
			get { return (Doc == null) ? null : Doc.ToXml(); }
			set { Doc = Util.GetXmlDocumentForString(value); }
		}
		public virtual XmlDocument Doc { get; set; }

		public virtual XmlNode MetaData { get { return Doc.Node("metadata"); } }
		#endregion

		#region Private
		string GetMeta(string tag)             { return MetaData.Node(tag).Text();     }
		void SetMeta(string tag, string value) { MetaData.NodeOrNew(tag).Text(value); }

		List<string> TrimmedListForNodes(string tag) {
			var nodes = Doc.Nodes(tag);
			if (nodes == null || nodes.Count == 0)
				return null;
			else
				return nodes.Select(node => node.InnerText.Trim()).ToList();
		}
		#endregion
	}
}
