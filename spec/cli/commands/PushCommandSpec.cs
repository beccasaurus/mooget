using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class PushCommandSpec : Spec {

		DirectoryOfNupkg remoteSource = new DirectoryOfNupkg(PathToTemp("MyRemotePackages"));

		[SetUp]
		public void Before() {
			base.BeforeEach();
			remoteSource.Path.AsDir().Create();
		}

		[Test][Description("moo help push")]
		public void help() {
			var help = moo("help push");
			help.ShouldContain("Usage: moo push foo.nupkg --source URL");
		}

		[Test][Description("moo push foo.nupkg --source URL")]
		public void can_push_Nupkg_to_source_using_source_path() {
			remoteSource.Packages.Count.ShouldEqual(0);

			var output = moo("push {0} --source {1}", PathToContent("packages", "MarkdownSharp.1.13.0.0.nupkg"), remoteSource.Path);
			output.ShouldContain("Pushed MarkdownSharp-1.13.0.0 to " + remoteSource.Path);

			remoteSource.Packages.Count.ShouldEqual(1);
			remoteSource.Packages.ToStrings().ShouldEqual(new List<string>{ "MarkdownSharp-1.13.0.0" });
		}

		[Test][Description("moo push foo.nupkg --source Foo")]
		public void can_push_Nupkg_to_source_using_source_name() {
			moo("source add Foo {0}", remoteSource.Path);
			remoteSource.Packages.Count.ShouldEqual(0);

			var output = moo("push {0} --source Foo", PathToContent("packages", "MarkdownSharp.1.13.0.0.nupkg"));
			output.ShouldContain("Pushed MarkdownSharp-1.13.0.0 to " + remoteSource.Path);

			remoteSource.Packages.Count.ShouldEqual(1);
			remoteSource.Packages.ToStrings().ShouldEqual(new List<string>{ "MarkdownSharp-1.13.0.0" });
		}

		[Test][Description("moo push foo.nupk")][Ignore]
		public void can_push_Nupkg_to_default_source() {
		}
	}
}
