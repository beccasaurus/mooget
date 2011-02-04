using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Represents the details of a Package (eg. authors, description, etc)</summary>
	public class PackageDetails {

		// can these be on the same line? TODO
		List<string> _authors;
		List<string> _owners;
		List<string> _tags;
		List<PackageDependency> _dependencies;
		string _language;

		/// <summary>Reference to the Package that these details belong to</summary>
		public virtual IPackage Package { get; set; }

		/// <summary>The human-friendly title of the package.</summary>
		public virtual string Title { get; set; }

		/// <summary>A description of the package.</summary>
		public virtual string Description { get; set; }

		/// <summary>A summary of the package.</summary>
		public virtual string Summary { get; set; }

		/// <summary>A URL for the home page of the package.</summary>
		public virtual string ProjectUrl { get; set; }

		/// <summary>
		/// A URL for the image to use as the icon for the package in the Add Library Package Reference dialog box. 
		/// This should be a 32x32-pixel .png file that has a transparent background.
		/// </summary>
		public virtual string IconUrl { get; set; }

		/// <summary>A link to the license that the package is under.</summary>
		public virtual string LicenseUrl { get; set; }

		/// <summary>
		/// A Boolean value that specifies whether the client needs to ensure that the 
		/// package license (described by licenseUrl) is accepted before the package is installed.
		/// </summary>
		public virtual bool RequiresLicenseAcceptance { get; set; }

		/// <summary>A comma-separated list of authors of the package code.</summary>
		public virtual string AuthorsText {
			get { return Authors.Join(",");    }
			set { Authors = value.ToTrimmedList(','); }
		}

		/// <summary>List representation of AuthorsText</summary>
		public virtual List<string> Authors {
			get { return this.Lazy(ref _authors); }
			set { _authors = value;               }
		}

		/// <summary>A comma-separated list of the package creators. This is often the same list as in authors.</summary>
		public virtual string OwnersText {
			get { return Owners.Join(",");    }
			set { Owners = value.ToTrimmedList(','); }
		}

		/// <summary>List representation of OwnersText</summary>
		public virtual List<string> Owners {
			get { return this.Lazy(ref _owners); }
			set { _owners = value;               }
		}

		/// <summary>The locale ID for the package, such as en-us.</summary>
		public virtual string Language {
			get { return _language;  }
			set { _language = value; }
		}

		/// <summary>The CultureInfo represented by the Language.</summary>
		public virtual CultureInfo Locale {
			get { return (Language == null) ? null : new CultureInfo(Language); }
			set { _language = (value == null) ? null : value.ToString(); }
		}

		/// <summary>
		/// A space-delimited list of tags and keywords that describe the package. 
		/// This information is used to help make sure users can find the package using searches 
		/// in the Add Package Reference dialog box or filtering in the Package Manager Console window.
		/// </summary>
		public virtual string TagsText {
			get { return Tags.Join(" ");    }
			set { Tags = value.ToList(' '); }
		}

		/// <summary>List representation of TagsText</summary>
		public virtual List<string> Tags {
			get { return this.Lazy(ref _tags); }
			set { _tags = value;               }
		}

		public virtual List<PackageDependency> Dependencies {
			get { return this.Lazy(ref _dependencies); }
			set { _dependencies = value;               }
		}
	}
}
