using System;

namespace MooGet {

	/// <summary>Represents a Package that another Package depends on</summary>
	public class PackageDependency : Package {
		public PackageVersion MinVersion { get; set; }
		public PackageVersion MaxVersion { get; set; }

		public string MinVersionString {
			get { return MinVersion.ToString(); }
			set { MinVersion = new PackageVersion(value); }
		}
		public string MaxVersionString {
			get { return MaxVersion.ToString(); }
			set { MaxVersion = new PackageVersion(value); }
		}
	}
}
