using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class FetchCommandSpec : Spec {

		DirectoryOfNupkg remoteSource = new DirectoryOfNupkg(PathToTemp("MyRemotePackages"));

		[SetUp]
		public void Before() {
			base.BeforeEach();
		}

		[Test][Description("moo help fetch")][Ignore]
		public void help() {
		}

		[Test][Description("moo fetch MyPackage --source URL")][Ignore]
		public void can_fetch_package_from_source() {
		}

		[Test][Description("moo fetch MyPackage")][Ignore]
		public void can_fetch_package_from_default_source() {
		}
	}
}
