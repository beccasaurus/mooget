using System;
using System.IO;
using System.Linq;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class FetchSpec : MooGetSpec {

		[TestFixture]
		public class API : MooGetSpec {
			
		}

		[TestFixture]
		public class Integration : MooGetSpec {

			// TODO need to see if WebClient DownloadFile will work from a local path so we can "download" a file
			[Test][Ignore]
			public void can_fetch_remote_package() {
				/*
				File.Exists(PathToTemp("working", "...")).ShouldBeFalse();

				moo();

				File.Exists().ShouldBeTrue();
				*/
			}
		}
	}
}
