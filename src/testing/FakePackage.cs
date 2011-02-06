using System;
using System.Linq;
using System.Collections.Generic;
using MooGet;

namespace MooGet.Test {

	/// <summary>Implementation of IPackage geared towards testing</summary>
	public class FakePackage : NewPackage, IPackage {
		public static string         DefaultId          = "MyPackage";
		public static string         DefaultVersionText = "1.0.0.0";
		public static PackageDetails DefaultDetails     = new PackageDetails {
			Description = "My description",
			AuthorsText = "remi,Wanda"
		};

		public FakePackage() {
			Id          = DefaultId;
			VersionText = DefaultVersionText;
			Details     = DefaultDetails.DeepClone();
		}
		public FakePackage(string nameAndVersion) : this() {
			var parts = new List<string>(nameAndVersion.Split(' '));
			Id = parts.First();
			if (parts.Count == 2) VersionText = parts.Last();
		}
		public FakePackage(string nameAndVersion, params string[] dependencies) : this(nameAndVersion) {
			foreach (var dep in dependencies)
				Details.Dependencies.Add(new PackageDependency(dep));
		}
	}
}
