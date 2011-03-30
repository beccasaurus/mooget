using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	/// <summary>Testing some generic Source stuff</summary>
	[TestFixture]
	public class SourceSpec : Spec {
		
		[Test]
		public void FromPath_recognizes_DirectoryOfNupkg() {
			var source = Source.ForPath(Directory.GetCurrentDirectory());

			source.Should(Be.TypeOf<DirectoryOfNupkg>());
			source.Path.ShouldEqual(Directory.GetCurrentDirectory());
		}

		[Test]
		public void FromPath_recognizes_NuGet_OData_sources() {
			var source = Source.ForPath("http://whatever");

			source.Should(Be.TypeOf<NuGetOData>());
			source.Path.ShouldEqual("http://whatever");
		}
	}
}
