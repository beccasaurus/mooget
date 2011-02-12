using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {

	public class MissingDependencyException : Exception {
		public List<PackageDependency> Dependencies { get; set; }

		public MissingDependencyException(List<PackageDependency> missingDependencies)
			: base("No packages were found that satisfy these dependencies: " + 
					string.Join(", ", missingDependencies.Select(dep => dep.ToString()).ToArray())) {

			Dependencies = new List<PackageDependency>(missingDependencies);
		}
	}

	/// <summary>Represents a Package Id and Version(s) that a Package depends on</summary>
	public class PackageDependency {

		string _rawVersionText;
		List<VersionAndOperator> _versions = new List<VersionAndOperator>();

		public PackageDependency() {}
		public PackageDependency(string nameAndVersions) {
			var parts = new List<string>(nameAndVersions.Split(' '));

			if (parts.Count > 0) {
				PackageId = parts.First().Trim();
				parts.RemoveAt(0);
			}

			if (parts.Count > 0)
				if (parts.Count == 1 && ! Regex.IsMatch(parts.First(), "[=><~]"))
					VersionText = "=" + parts.First();
				else
					VersionText = string.Join(" ", parts.ToArray());
		}

		public List<VersionAndOperator> Versions {
			get { return _versions;  }
			set { _versions = value; }
		}

		/// <summary>The Id of the Package this dependency is for</summary>
		public string PackageId { get; set; }

		/// <summary>Alias for PackageId</summary>
		public string Id {
			get { return PackageId;  }
			set { PackageId = value; }
		}

		public bool Matches(IPackage package) {
			if (package.Id != Id)
				return false;
			else
				return Matches(package.Version);
		}

		public bool Matches(string version) {
			return Matches(new PackageVersion(version));
		}

		public bool Matches(PackageVersion version) {
			foreach (var versionDependency in Versions)
				if (! versionDependency.Matches(version))
					return false;
			return true;
		}

		public static bool MatchesAll(string version, params PackageDependency[] dependencies) {
			return MatchesAll(new PackageVersion(version), dependencies);
		}

		public static bool MatchesAll(IPackage package, params PackageDependency[] dependencies) {
			foreach (var dependency in dependencies)
				if (! dependency.Matches(package))
					return false;
			return true;
		}

		public static bool MatchesAll(PackageVersion version, params PackageDependency[] dependencies) {
			foreach (var dependency in dependencies)
				if (! dependency.Matches(version))
					return false;
			return true;
		}

		/// <summary>String representation of this PackageDependency showing the PackageId and all Versions</summary>
		public override string ToString() {
			return string.Format("{0} {1}", PackageId, VersionsString).Trim();
		}

		public string VersionsString {
			get { return string.Join(" ", Versions.Select(v => v.ToString()).ToArray()).Trim(); }
		}

		public static bool operator != (PackageDependency a, PackageDependency b) { return ! (a == b);  }

		public static bool operator == (PackageDependency a, PackageDependency b) {
			if ((object) b == null)
				return ((object) a == null);
			else
				return a.Equals(b);
		}

		public override bool Equals(object o) {
			if (o == null) return false;

			if (this.GetType() != o.GetType()) return false;

			return this.ToString() == ((PackageDependency) o).ToString();
		}

		/// <summary>Version string can be a specific version (eg. 1.0) or a matcher (eg. &gt;= 1.0)</summary>
		/// <remarks>
		///	Setting the VersionText to something like "&gt;=1.0" will parse the string and set MinVersion.
		///
		///	Getting the VersionText returns the raw string that was set, eg. "&gt;=1.0"
		/// </remarks>
		public string VersionText {
			get { return _rawVersionText; }
			set {
				_rawVersionText = value;
				Parse();
			}
		}

		/// <summary>Represents an *exact* version for this dependency, eg. 1.0</summary>
		public PackageVersion Version {
			get {
				var versionOperator = Versions.FirstOrDefault(v => v.Operator == Operators.EqualTo);
				return (versionOperator == null) ? null : versionOperator.Version;
			}
			set { AddVersion("=", value.ToString()); }
		}

		/// <summary>Represents an *exact* minumum version for this dependency, eg. 1.0</summary>
		public PackageVersion MinVersion {
			get {
				var versionOperator = Versions.FirstOrDefault(v => v.Operator == Operators.GreaterThanOrEqualTo);
				return (versionOperator == null) ? null : versionOperator.Version;
			}
			set { AddVersion(">=", value.ToString()); }
		}

		/// <summary>Represents an *exact* "sorta" minumum version for this dependency, eg. 1.0</summary>
		/// <remarks>
		/// If you set the VersionText to "~&gt;1.1.0" or "~&gt;1.1" that sets the SortaMinVersion to 
		/// 1.1.0 or 1.1.
		///
		/// Wheat does that mean?
		///
		/// If the SortaMinVersion is 1.1 or 1.1.0, that says that this dependency can install any version 
		/// of this package greater than or equal to 1.1.0 BUT less than 1.2.  It will not install the next 
		/// "minor" version of this package, but will allow for the installation of additional builds/revisions.
		/// </remarks>
		public PackageVersion SortaMinVersion {
			get {
				var versionOperator = Versions.FirstOrDefault(v => v.Operator == Operators.SortaGreaterThan);
				return (versionOperator == null) ? null : versionOperator.Version;
			}
			set { AddVersion("~>", value.ToString()); }
		}

		/// <summary>Represents an *exact* maximum version for this dependency, eg. 1.0</summary>
		public PackageVersion MaxVersion {
			get {
				var versionOperator = Versions.FirstOrDefault(v => v.Operator == Operators.LessThanOrEqualTo);
				return (versionOperator == null) ? null : versionOperator.Version;
			}
			set { AddVersion("<=", value.ToString()); }
		}

		/// <summary>Get or set string representing the *exact* MinVersion, eg. 1.0</summary>
		public string MinVersionText {
			get {
				if (MinVersion == null) return null;
				return MinVersion.ToString();
			}
			set { MinVersion = new PackageVersion(value); }
		}

		/// <summary>Get or set string representing the *exact* SortaMinVersion, eg. 1.0</summary>
		public string SortaMinVersionText {
			get {
				if (SortaMinVersion == null) return null;
				return SortaMinVersion.ToString();
			}
			set { SortaMinVersion = new PackageVersion(value); }
		}

		/// <summary>Get or set string representing the *exact* MaxVersion, eg. 1.0</summary>
		public string MaxVersionText {
			get {
				if (MaxVersion == null) return null;
				return MaxVersion.ToString();
			}
			set { MaxVersion = new PackageVersion(value); }
		}

		// parses the VersionText
		//
		// NOTE do *NOT* set VersionText from this method, or you'll end up in an infinite loop!
		void Parse() {
			if (VersionText == null) return;

			// Parse all groups of (>=~<) operators followed by version numbers (1.2.3.4)
			foreach (Match match in Regex.Matches(VersionText, @"([><=~]+)\s*([\d\.]+)"))
				AddVersion(match.Groups[1].Value, match.Groups[2].Value);

			// If no matchers were found, no operator was provided (eg. "1.2") so we say that we must be equal to this version
			if (Versions.Count == 0)
				AddVersion("=", VersionText);
		}

		void AddVersion(string operatorString, string version) {
			Versions.Add(new VersionAndOperator { OperatorString = operatorString, VersionText = version });
		}

		public static Operators ParseOperator(string op) {
			if (op == null)
				return Operators.EqualTo;

			op = op.Trim().Replace(" ","");

			if (op.Contains(">"))

				if (op.Contains("="))
					return Operators.GreaterThanOrEqualTo;
				else if (op.Contains("~"))
					return Operators.SortaGreaterThan;
				else
					return Operators.GreaterThan;

			else if (op.Contains("<"))

				if (op.Contains("="))
					return Operators.LessThanOrEqualTo;
				else if (op.Contains("~"))
					return Operators.SortaLessThan;
				else
					return Operators.LessThan;

			else
				return Operators.EqualTo;
		}

		public static string GetOperatorString(Operators op) {
			switch (op) {
				case Operators.EqualTo:              return "=";  break;
				case Operators.GreaterThan:          return ">";  break;
				case Operators.GreaterThanOrEqualTo: return ">="; break;
				case Operators.SortaGreaterThan:     return "~>"; break;
				case Operators.LessThan:             return "<";  break;
				case Operators.LessThanOrEqualTo:    return "<="; break;
				case Operators.SortaLessThan:        return "<~"; break;
			}
			return null;
		}

		public enum Operators {
			EqualTo,
			GreaterThan,
			GreaterThanOrEqualTo,
			SortaGreaterThan,
			LessThan,
			LessThanOrEqualTo,
			SortaLessThan
		};

		/// <summary>Represents a paired Version string (eg. 1.0) and Operator (eg. GreaterThan)</summary>
		public class VersionAndOperator {
			public PackageVersion Version  { get; set; }
			public Operators      Operator { get; set; }

			public string VersionText {
				get {
					if (Version == null) return null;
					return Version.ToString();
				}
				set { Version = new PackageVersion(value); }
			}

			public string OperatorString {
				get { return PackageDependency.GetOperatorString(Operator); }
				set { Operator = PackageDependency.ParseOperator(value);    }
			}

			public override string ToString() {
				return string.Format("{0} {1}", PackageDependency.GetOperatorString(Operator), Version);
			}

			public bool Matches(string version) {
				return Matches(new PackageVersion(version));
			}

			public bool Matches(PackageVersion version) {
				switch (Operator) {
					case Operators.EqualTo:
						return version == Version;
					case Operators.LessThan:
						return version < Version;
					case Operators.LessThanOrEqualTo:
						return version <= Version;
					case Operators.SortaLessThan:
						return version.SortaLessThan(Version);
					case Operators.GreaterThan:
						return version > Version;
					case Operators.GreaterThanOrEqualTo:
						return version >= Version;
					case Operators.SortaGreaterThan:
						return version.SortaGreaterThan(Version);
					default:
						throw new Exception("Unknown operator: " + Operator.ToString());
				}
			}
		}
	}
}
