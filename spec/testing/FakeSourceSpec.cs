using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;
using MooGet.Test;

namespace MooGet.Specs.Testing {

	[TestFixture]
	public class FakeSourceSpec : Spec {
		
		[Test]
		public void Get() {
			var source = new FakeSource();
			source.Get(new PackageDependency("Foo")).Should(Be.Null);

			source.Add("Foo 1.2.3", "Dep > 1.0");

			source.Get(new PackageDependency("Foo")).ShouldNot(Be.Null);
			source.Get(new PackageDependency("Foo")).Id.ShouldEqual("Foo");
			source.Get(new PackageDependency("Foo")).Version.ToString().ShouldEqual("1.2.3");
			source.Get(new PackageDependency("Foo")).Details.Dependencies.ToStrings().ShouldEqual(new List<string>{ "Dep > 1.0" });
		}
	}
}
