using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using FluentXml;

namespace MooGet {

	/// <summary>Represents a .nuspec file (can have a Path or simply the XML text)</summary>
	/// <remarks>
	///	Nuspec can be used to read and write Nuspec XML and files.
	///
	///	NOTE: there's a gotcha when using Nuspec to write XML.  You cannot modify any of the 
	/// List properties, eg. Dependencies or Authors.  Instead, you must *set* the whole property. 
	/// Inotherwords, you cannot <c>nuspec.Authors.Add("foo")</c> but you can <c>nuspec.Authors = authorsList</c>.
	/// Authors, Owners, and Tags all have helper *Text properties that you can modify freely, however, eg. AuthorsText.
	/// 
	/// We should fix these XML oddities ... they're annoying.  I'd like to map the properties to XML the way we do it in Clide.
	///
	/// </remarks>
	public class Nuspec : PackageDetails, IPackage, IFile {

		/// <summary>The XML that every new Nuspec starts out with ... stubs out the main nodes for us</summary>
		public static string BlankNuspecXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
			+ "<package xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
				+ "<metadata xmlns=\"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd\"></metadata>"
			+ "</package>";

		public Nuspec() {
			Xml = BlankNuspecXml;
		}
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
			set {
				_path = value;
				if (this.Exists())
					Xml = Util.ReadFile(value);
			}
		}

		public virtual  string Id          { get { return GetMeta("id");          } set { SetMeta("id",           value); } }
		public virtual  string VersionText { get { return GetMeta("version");     } set { SetMeta("version",      value); } }
		public override string Title       { get { return GetMeta("title");       } set { SetMeta("title",        value); } }
		public override string Description { get { return GetMeta("description"); } set { SetMeta("description",  value); } }
		public override string Summary     { get { return GetMeta("summary");     } set { SetMeta("summary",      value); } }
		public override string ProjectUrl  { get { return GetMeta("projectUrl");  } set { SetMeta("projectUrl",   value); } }
		public override string LicenseUrl  { get { return GetMeta("licenseUrl");  } set { SetMeta("licenseUrl",   value); } }
		public override string IconUrl     { get { return GetMeta("iconUrl");     } set { SetMeta("iconUrl",      value); } }
		public override string Language    { get { return GetMeta("language");    } set { SetMeta("language",     value); } }

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
			set {
				// remove existing <files>
				var existingNode = MetaData.Node("files");
				if (existingNode != null)
					existingNode.ParentNode.RemoveChild(existingNode);

				if (value == null || value.Count == 0) return;

				// add new <files> and new <dependency> nodes underneath it
				var newNode = Doc.Node("package").NodeOrNew("files");
				foreach (var file in value) {
					var node = newNode.NewNode("file");
					node.Attr("src", file.Source);
					if (! string.IsNullOrEmpty(file.Target))
						node.Attr("target", file.Target);
				}
			}
		}

		public virtual void Save() {
			this.Write(Xml);
		}

		#region Xml Stuff
		public virtual string Xml {
			get { return Doc.ToXml(); }
			set { Doc = Util.GetXmlDocumentForString(value); }
		}

		public virtual XmlDocument Doc {
			get { return _doc ?? (_doc = new XmlDocument()); }
			set { _doc = value; }
		}

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
