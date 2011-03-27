using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using FluentXml;

namespace MooGet {

	/// <summary>Xml-related extension methods</summary>
	public static class XmlExtensions {

		public static PackageDependency ToDependency(this XmlNode node) {
			return new PackageDependency(string.Format("{0} {1}", node.Attr("id"), node.Attr("version")).Trim());
		}

		public static NuspecFileSource ToFileSource(this XmlNode node) {
			return new NuspecFileSource { Source = node.Attr("src"), Target = node.Attr("target") };
		}
	}
}
