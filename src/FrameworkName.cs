using System;
using System.Text.RegularExpressions;

namespace MooGet {

	/// <summary>Represents the name of a version of the .NET framework, eg. .NET 1.1 or Silverlight 4.0</summary>
	public class FrameworkName {

		public static string DefaultName     = ".NETFramework";
		public static string SilverlightName = "Silverlight";

		public FrameworkName() {}
		public FrameworkName(string name, string version) {
			_name    = name;
			_version = version;
		}

		string _name;
		string _version;

		public string Name {
			get {
				if (_name == null) return DefaultName;

				var name = _name.ToLower().Replace(" ", "").Replace(".", "");
				if (name.Length == 0 || name.Contains("net"))
					return DefaultName;
				if (name.Contains("sl") || name.Contains("silverlight"))
					return SilverlightName;
				throw new Exception("Unknown FrameworkName Name: " + _name);
			}
		}
		public string Version {
			get {
				var version = _version.ToLower().Replace(" ", "").Replace(".", "");
				switch (version.Length) {
					case 1:
						return version + ".0";
						break;
					case 2:
						return version.Substring(0,1) + "." + version.Substring(1);
						break;
					default:
						throw new Exception("Unknown FrameworkName Version: " + _version);
				}
			}
		}

		public string FullName { get { return string.Format("{0} {1}", Name, Version); } }

		public static FrameworkName Parse(string name) {
			var match = Regex.Match(name.Replace(".", "").Replace(" ", "").ToLower(), @"^([a-z]*)([0-9]*)$");
			return new FrameworkName(match.Groups[1].Value, match.Groups[2].Value);
		}
	}
}
