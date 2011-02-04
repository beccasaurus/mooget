using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	/*
	public class MySource : Source, ISource {
		public override List<Package> Packages {
			get {
				return new List<Package> {
					new Package { Id = "NUnit",      VersionString = "1.0"     },
					new Package { Id = "NUnit",      VersionString = "1.0.1"   },
					new Package { Id = "NUnit",      VersionString = "2.0"     },
					new Package { Id = "NHibernate", VersionString = "0.0.0.1" },
					new Package { Id = "NHibernate", VersionString = "0.0.2.0" },
					new Package { Id = "FooBar",     VersionString = "0.0.2.0" },
					new Package { Id = "my.package", VersionString = "0.0.2.0" },
					new Package { Id = "my.package", VersionString = "0.1.2.2" },
					new Package { Id = "Cool-Beans", VersionString = "9"       },
					new Package { Id = "Cool-Beans", VersionString = "1.0"     },
					new Package { Id = "Cool-Beans", VersionString = "10.2.6"  }
				};
			}
		}
	}
	*/

	/*
	 * Source is an optional base class for all MooGet.ISource.
	 *
	 * If your source inherits from Source, you don't need to provide any custom filtering 
	 * logic if you don't want to.  You can just provide a way to get all packages and 
	 * everything else will be done for you.
	 *
	 * A source can have many Package, regardless of whether they're Local or Remote, etc.
	 */
	[TestFixture]
	public class SourceSpec : MooGetSpec {

		// Tests the base Source implementation of ISource
		[TestFixture]
		public class BaseImplementation {

			//ISource source = new MySource();

			[Test][Ignore]
			public void Get() {
			}

			[Test][Ignore]
			public void Packages() {
			}

			[Test][Ignore]
			public void LatestPackages() {
			}

			[Test][Ignore]
			public void GetPackagesWithId() {
			}

			[Test][Ignore]
			public void GetPackagesMatchingDependency() {
			}
		}
	}
}
