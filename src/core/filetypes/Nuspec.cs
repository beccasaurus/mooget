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
	public class Nuspec : PackageDetails, IPackage {

		public Nuspec() {}
		public Nuspec(string path) : this() {
			Path = path;
		}

		string      _path;
		XmlDocument _doc;

		public virtual PackageDetails Details { get { return this; } }

		public virtual ISource Source { get { return null; } }

		public virtual string Path {
			get { return _path; }
			set { _path = value; Xml = Util.ReadFile(value); }
		}

		public virtual  string Id            { get { return GetMeta("id");          } set { SetMeta("id",           value); } }
		public virtual  string VersionString { get { return GetMeta("version");     } set { SetMeta("version",      value); } }
		public override string Title         { get { return GetMeta("title");       } set { SetMeta("title",        value); } }
		public override string Description   { get { return GetMeta("description"); } set { SetMeta("description",  value); } }
		public override string Summary       { get { return GetMeta("summary");     } set { SetMeta("summary",      value); } }
		public override string ProjectUrl    { get { return GetMeta("projectUrl");  } set { SetMeta("projectUrl",   value); } }
		public override string LicenseUrl    { get { return GetMeta("licenseUrl");  } set { SetMeta("licenseUrl",   value); } }
		public override string IconUrl       { get { return GetMeta("iconUrl");     } set { SetMeta("iconUrl",      value); } }
		public override string Language      { get { return GetMeta("language");    } set { SetMeta("language",     value); } }

		public override List<string> Authors {
			get { return GetMeta("authors").ToTrimmedList(','); }
			set { SetMeta("authors", value.Join(","));          }
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
			get { return new PackageVersion(VersionString); }
			set { VersionString = (value == null) ? null : value.ToString(); }
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
				var newNode = MetaData.CreateNode("dependencies");
				foreach (var dependency in value) {
					var node = newNode.ForceCreateNode("dependency");
					node.Attr("id", dependency.Id);
					if (dependency.Versions.Count > 0)
						node.Attr("version", dependency.VersionsString);
				}
			}
		}

		public virtual string[] Files { get { return null; } }

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
		void SetMeta(string tag, string value) { MetaData.CreateNode(tag).Text(value); }
		#endregion
	}
}
