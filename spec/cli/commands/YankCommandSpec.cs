using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class YankCommandSpec : Spec {

		DirectoryOfNupkg remoteSource = new DirectoryOfNupkg(PathToTemp("MyRemotePackages"));

		[SetUp]
		public void Before() {
			base.BeforeEach();
			PathToContent("more_packages").AsDir().Copy(remoteSource.Path);
			remoteSource.Packages.Count.ShouldEqual(13);
		}

		[Test][Description("moo help yank")]
		public void help() {
			var help = moo("help yank");
			help.ShouldContain("Usage: moo yank PACKAGE -v VERSION --source URL");
		}

		[Test][Description("moo yank foo -v 1.0 --source URL|NAME")]
		public void can_yank_package_from_source() {
			remoteSource.Packages.Count.ShouldEqual(13);
			remoteSource.Packages.ToStrings().ShouldEqual(new List<string>{ "Antlr-3.1.1", "Apache.NMS-1.4.0", "Apache.NMS.ActiveMQ-1.4.1", "AttributeRouting-0.5.3967", "Castle.Core-1.1.0", "FluentNHibernate-1.1.0.694", "MarkdownSharp-1.13.0.0", "NHibernate-2.1.2.4000", "NuGet.CommandLine-1.0.11220.26", "NuGet.CommandLine-1.1.2120.134", "NuGet.CommandLine-1.1.2120.136", "WebActivator-1.0.0.0", "log4net-1.2.10" });

			moo("yank NuGet.CommandLine").ShouldContain("Usage: moo yank PACKAGE -v VERSION --source URL"); // need to specify version
			moo("yank NuGet.CommandLine -v 1.1.2120.134").ShouldContain("Usage: moo yank PACKAGE -v VERSION --source URL"); // need to specify source
			remoteSource.Packages.Count.ShouldEqual(13);

			moo("yank NuGet.CommandLine -v 1.1.2120.134 -s {0}", remoteSource.Path).ShouldContain("Yanked NuGet.CommandLine = 1.1.2120.134 from " + remoteSource.Path);
			remoteSource.Packages.Count.ShouldEqual(12);
			remoteSource.Packages.ToStrings().ShouldEqual(new List<string>{ "Antlr-3.1.1", "Apache.NMS-1.4.0", "Apache.NMS.ActiveMQ-1.4.1", "AttributeRouting-0.5.3967", "Castle.Core-1.1.0", "FluentNHibernate-1.1.0.694", "MarkdownSharp-1.13.0.0", "NHibernate-2.1.2.4000", "NuGet.CommandLine-1.0.11220.26", "NuGet.CommandLine-1.1.2120.136", "WebActivator-1.0.0.0", "log4net-1.2.10" });

			// can't yank if you already did!
			moo("yank NuGet.CommandLine -v 1.1.2120.134 -s {0}", remoteSource.Path).ShouldContain("NuGet.CommandLine = 1.1.2120.134 could not be yanked from " + remoteSource.Path);
			remoteSource.Packages.Count.ShouldEqual(12);

			// try yanking abunch ...
			moo("yank Antlr -v 3.1.1 --source {0}", remoteSource.Path);
			moo("yank -s {0} NHibernate --version 2.1.2.4000", remoteSource.Path);
			moo("yank -s {0} --v 1.1.0.694 FluentNHibernate", remoteSource.Path);
			remoteSource.Packages.Count.ShouldEqual(9);
			remoteSource.Packages.ToStrings().ShouldEqual(new List<string>{ "Apache.NMS-1.4.0", "Apache.NMS.ActiveMQ-1.4.1", "AttributeRouting-0.5.3967", "Castle.Core-1.1.0", "MarkdownSharp-1.13.0.0", "NuGet.CommandLine-1.0.11220.26", "NuGet.CommandLine-1.1.2120.136", "WebActivator-1.0.0.0", "log4net-1.2.10" });

			// use name ...
			moo("source add \"Foo Bar\" {0}", remoteSource.Path);

			moo("yank Castle.Core --version 1.1.0 -s \"Foo Bar\"");
			remoteSource.Packages.Count.ShouldEqual(8);
			remoteSource.Packages.ToStrings().ShouldEqual(new List<string>{ "Apache.NMS-1.4.0", "Apache.NMS.ActiveMQ-1.4.1", "AttributeRouting-0.5.3967", "MarkdownSharp-1.13.0.0", "NuGet.CommandLine-1.0.11220.26", "NuGet.CommandLine-1.1.2120.136", "WebActivator-1.0.0.0", "log4net-1.2.10" });
		}

		[Test][Description("moo yank MyPackage")][Ignore]
		public void can_yank_package_from_default_source() {
		}
	}
}
