using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using EasyOData;
using FluentXml;

namespace MooGet {

	// TODO Should :Package instead of :NewPackage after we refactor ...
	
	/// <summary>Represents a NuGet package from the NuGet OData source</summary>
	/// <remarks>
	/// NOTE: This is read-only!  Setting properties on NuGetODataPackage (or Details) does NOTHING!
	/// </remarks>
	public class NuGetODataPackage : NewPackage, IPackage {

		public NuGetODataPackage() : base() {}
		public NuGetODataPackage(Entity entity) : this() {
			Entity = entity;
		}
		public NuGetODataPackage(Entity entity, NuGetOData source) : this(entity) {
			Source = source;
		}

		/// <summary>The OData Entity that this wraps</summary>
		public virtual Entity Entity { get; set; }

		/// <summary>Uses NuGetODataPackageDetails to return details from our OData Entity</summary>
		public override PackageDetails Details { get { return new NuGetODataPackageDetails(this); }}

		public override string Id {
			get { return Prop("Id"); }
		}

		public override PackageVersion Version {
			get { return new PackageVersion(Prop("Version")); }
		}

		/// <summary>Returns the url to download the nupkg (zip) file for this package</summary>
		public virtual string DownloadUrl {
			get { return Doc.Node("Content").Attr("src"); }
		}

		/// <summary>Returns an XmlDocument for the Entity's XML</summary>
		public XmlDocument Doc {
			get { return (Entity == null) ? null : FluentXmlDocument.FromString(Entity.Xml); }
		}

		/// <summary>Given the name of a property, returns the value of that property on the entity</summary>
		public virtual string Prop(string name) {
			if (Entity == null) return null;

			var value = Entity[name];
			if (value == null)
				return null;
			else
				return value.ToString();
		}
	}

	// TODO move this class to its own file ...

	/// <summary>PackageDetails that wraps a NuGetODataPackage and uses the OData (properties/xml) to get this package's details</summary>
	public class NuGetODataPackageDetails : PackageDetails {
		public NuGetODataPackageDetails() : base() {}
		public NuGetODataPackageDetails(NuGetODataPackage package) : this() {
			Package = package;
			Update();
		}

		/// <summary>The NuGetODataPackage that this reads from to create the package details.  The package must have an Entity.</summary>
		public virtual NuGetODataPackage Package { get; set; }

		public virtual string Prop(string name) { return Package.Prop(name); }

		/// <summary>Updates all of our properties using the provided Package (and its Entity)</summary>
		public virtual void Update() {
			if (Package == null || Package.Entity == null) return;

			Title        = Prop("Title");
			Description  = Prop("Description");
			Summary      = Prop("Summary");
			ProjectUrl   = Prop("ProjectUrl");
			LicenseUrl   = Prop("LicenseUrl");
			IconUrl      = Prop("IconUrl");
			AuthorsText  = Prop("Authors");
			TagsText     = Prop("Tags");
			Dependencies = Prop("Dependencies").Split('|').Select(str => ToDependency(str)).ToList();
		}

		/// <summary>Parses the given string from the OData NuGet service and returns a PackageDependency</summary>
		/// <remarks>
		/// The NuGet OData feed displays dependencies like this: Castle.Windsor:2.5.2|Castle.Core-log4net:2.5.2|log4net:1.2.10
		/// </remarks>
		public virtual PackageDependency ToDependency(string str) {
			var parts = str.Split(':');
			return new PackageDependency(string.Format("{0} >= {1}", parts.First(), parts.Last()));
		}
	}
}
