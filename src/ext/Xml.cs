using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>Xml-related extension methods</summary>
	public static class XmlExtensions {

		public static string ToXml(this XmlDocument doc) {
			if (doc == null) return null;

			var stream = new MemoryStream();
			var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true });
			doc.WriteTo(writer);
			writer.Flush();
			var buffer = stream.ToArray();
			var xml    = System.Text.Encoding.UTF8.GetString(buffer).Trim();

			return xml;
		}

		public static XmlNode Node(this XmlDocument doc, string tag) {
			if (doc == null) return null;
			var nodes = doc.GetElementsByTagName(tag);
			return (nodes.Count > 0) ? nodes[0] : null;
		}

        public static List<XmlNode> Nodes(this XmlNode node, string tag) {
            var nodes = new List<XmlNode>();
			if (node == null) return nodes;
            foreach (XmlNode child in node.ChildNodes) {
                nodes.AddRange(child.Nodes(tag));
                if (child.Name.ToLower() == tag.ToLower())
                    nodes.Add(child);
            }
            return nodes;
        } 	

		public static XmlNode Node(this XmlNode node, string tag) {
			if (node == null) return null;
			var tags = node.Nodes(tag);
			return (tags != null && tags.Count > 0) ? tags[0] : null;
		}

		// Will find or create the tag
		public static XmlNode NodeOrNew(this XmlNode node, string tag) {
			if (node == null) return null;
			return node.Node(tag) ?? node.NewNode(tag);
		}

		public static XmlNode NewNode(this XmlNode node, string tag) {
			if (node == null) return null;
			var child = node.OwnerDocument.CreateElement(tag);
			node.AppendChild(child);
			return child;
		}

		public static string Text(this XmlNode node) {
			if (node == null) return null;
			return node.InnerText.Trim();
		}

		public static XmlNode Text(this XmlNode node, string value) {
			if (node != null) node.InnerText = value;
			return node;
		}

		public static string Attr(this XmlNode node, string attr) {
			if (node == null)                       return null;
			if (node.Attributes[attr] == null) return null;
			return node.Attributes[attr].Value;
		}

		public static XmlNode Attr(this XmlNode node, string attr, string value) {
			if (node == null) return null;
			var attribute = node.Attributes[attr];
			if (attribute == null) {
				attribute = node.OwnerDocument.CreateAttribute(attr);
				node.Attributes.Append(attribute);
			}
			attribute.Value = value;
			return node;
		}

		public static PackageDependency ToDependency(this XmlNode node) {
			return new PackageDependency(string.Format("{0} {1}", node.Attr("id"), node.Attr("version")).Trim());
		}

		public static NuspecFileSource ToFileSource(this XmlNode node) {
			return new NuspecFileSource { Source = node.Attr("src"), Target = node.Attr("target") };
		}
	}
}
