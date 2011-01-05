using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class FrameworkNameSpec : MooGetSpec {

		[Test]
		public void parses_lots_of_different_ways_to_write_the_name_of_a_dotnet_framework() {
			foreach (var item in new Dictionary<string, string> {
				{ ".NETFramework 1.1" , ".NETFramework 1.1" },
				{ "NETFramework 1.1"  , ".NETFramework 1.1" },
				{ ".NET 1.1"          , ".NETFramework 1.1" },
				{ "NET 1.1"           , ".NETFramework 1.1" },
				{ "NET1.1"            , ".NETFramework 1.1" },
				{ "Net 1.1"           , ".NETFramework 1.1" },
				{ "Net1.1"            , ".NETFramework 1.1" },
				{ "net1.1"            , ".NETFramework 1.1" },
				{ "1.1"               , ".NETFramework 1.1" },
				{ "11"                , ".NETFramework 1.1" },
				{ "20"                , ".NETFramework 2.0" },
				{ "35"                , ".NETFramework 3.5" },
				{ "40"                , ".NETFramework 4.0" },
				{ "4"                 , ".NETFramework 4.0" },
				{ "SL4"               , "Silverlight 4.0"   },
				{ "SL 4"              , "Silverlight 4.0"   },
				{ "Silverlight 4"     , "Silverlight 4.0"   }
			})
				FrameworkName.Parse(item.Key).FullName.ShouldEqual(item.Value);
		}
	}
}
