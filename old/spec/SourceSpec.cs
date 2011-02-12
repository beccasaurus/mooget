using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.Core {

	public class MySource : Source, ISource {
		public override List<IPackage> Packages {
			get {
				return new List<IPackage> {
					new NewPackage { Id = "NUnit",      VersionText = "1.0"     },
					new NewPackage { Id = "NUnit",      VersionText = "1.0.1"   },
					new NewPackage { Id = "NUnit",      VersionText = "2.0"     },
					new NewPackage { Id = "NHibernate", VersionText = "0.0.0.1" },
					new NewPackage { Id = "NHibernate", VersionText = "0.0.2.0" },
					new NewPackage { Id = "FooBar",     VersionText = "0.0.2.0" },
					new NewPackage { Id = "my.package", VersionText = "0.0.2.0" },
					new NewPackage { Id = "my.package", VersionText = "0.1.2.2" },
					new NewPackage { Id = "Cool-Beans", VersionText = "9"       },
					new NewPackage { Id = "Cool-Beans", VersionText = "1.0"     },
					new NewPackage { Id = "Cool-Beans", VersionText = "10.2.6"  }
				};
			}
		}

		public override IPackageFile Fetch(PackageDependency dep, string dir){ return null; }
		public override IPackage     Push(IPackageFile file){ return null; }
		public override bool         Yank(PackageDependency dep){ return false; }
	}

	/*
	 * Source is an optional base class for all MooGet.ISource.
	 *
	 * If your source inherits from Source, you don't need to provide any custom filtering 
	 * logic if you don't want to.  You can just provide a way to get all packages and 
	 * everything else will be done for you.
	 *
	 * A source can have many Package, regardless of whether they're Local or Remote, etc.
	 */
	// Tests the base Source implementation of ISource
	[TestFixture]
	public class SourceSpec : Spec {

		ISource source = new MySource();

		[Test]
		public void Get() {
			source.Get(new PackageDependency("NUnit"      )).IdAndVersion().ShouldEqual("NUnit-2.0");
			source.Get(new PackageDependency("NUnit < 2.0")).IdAndVersion().ShouldEqual("NUnit-1.0.1");
			source.Get(new PackageDependency("NUnit 1.0"  )).IdAndVersion().ShouldEqual("NUnit-1.0");
		}

		[Test]
		public void Packages() {
			source.Packages.Count.ShouldEqual(11);
		}

		[Test]
		public void LatestPackages() {
			source.LatestPackages.Count.ShouldEqual(5);
			source.LatestPackages.ToStrings().ShouldEqual(new List<string>{
				"NUnit-2.0", "NHibernate-0.0.2.0", "FooBar-0.0.2.0", "my.package-0.1.2.2", "Cool-Beans-10.2.6"
			});
		}

		[Test]
		public void GetPackagesWithId() {
			source.GetPackagesWithId("FooBar").Count.ShouldEqual(1);
			source.GetPackagesWithId("FooBar").ToStrings().ShouldEqual(new List<string>{ "FooBar-0.0.2.0" });
			source.GetPackagesWithId("FooBar").Oldest().ToString().ShouldEqual("FooBar-0.0.2.0");
			source.GetPackagesWithId("FooBar").Latest().ToString().ShouldEqual("FooBar-0.0.2.0");

			source.GetPackagesWithId("my.package").Count.ShouldEqual(2);
			source.GetPackagesWithId("my.package").ToStrings().ShouldEqual(new List<string>{ "my.package-0.0.2.0", "my.package-0.1.2.2" });
			source.GetPackagesWithId("my.package").Oldest().ToString().ShouldEqual("my.package-0.0.2.0");
			source.GetPackagesWithId("my.package").Latest().ToString().ShouldEqual("my.package-0.1.2.2");

			source.GetPackagesWithId("NUnit").Count.ShouldEqual(3);
			source.GetPackagesWithId("NUnit").ToStrings().ShouldEqual(new List<string>{ "NUnit-1.0", "NUnit-1.0.1", "NUnit-2.0" });
			source.GetPackagesWithId("NUnit").Oldest().ToString().ShouldEqual("NUnit-1.0");
			source.GetPackagesWithId("NUnit").Latest().ToString().ShouldEqual("NUnit-2.0");
		}

		[Test]
		public void GetPackagesMatchingDependency() {
			source.GetPackagesMatchingDependencies(new PackageDependency("Cool-Beans")).ToStrings().ShouldEqual(new List<string>{ "Cool-Beans-9", "Cool-Beans-1.0", "Cool-Beans-10.2.6" });
			source.GetPackagesMatchingDependencies(new PackageDependency("Cool-Beans > 5")).ToStrings().ShouldEqual(new List<string>{ "Cool-Beans-9", "Cool-Beans-10.2.6" });
			source.GetPackagesMatchingDependencies(new PackageDependency("Cool-Beans > 9")).ToStrings().ShouldEqual(new List<string>{ "Cool-Beans-10.2.6" });
			source.GetPackagesMatchingDependencies(new PackageDependency("Cool-Beans < 10 > 3")).ToStrings().ShouldEqual(new List<string>{ "Cool-Beans-9" });
			source.GetPackagesMatchingDependencies(new PackageDependency("Cool-Beans < 10"), new PackageDependency("Cool-Beans > 3")).ToStrings().ShouldEqual(new List<string>{ "Cool-Beans-9" });
			source.GetPackagesMatchingDependencies(new PackageDependency("NUnit ~> 1")).ToStrings().ShouldEqual(new List<string>{ "NUnit-1.0", "NUnit-1.0.1" });
			source.GetPackagesMatchingDependencies(new PackageDependency("NUnit ~> 1.0")).ToStrings().ShouldEqual(new List<string>{ "NUnit-1.0", "NUnit-1.0.1" });
			source.GetPackagesMatchingDependencies(new PackageDependency("NUnit >= 1.0")).ToStrings().ShouldEqual(new List<string>{ "NUnit-1.0", "NUnit-1.0.1", "NUnit-2.0" });
			source.GetPackagesMatchingDependencies(new PackageDependency("NUnit >= 1.0"), new PackageDependency("NUnit > 1.0")).ToStrings().ShouldEqual(new List<string>{ "NUnit-1.0.1", "NUnit-2.0" });
		}
	}
}
