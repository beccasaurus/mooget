using System;
using System.Linq;
using MooGet;
using NUnit.Framework;
using MooGet.Options;

namespace MooGet.Specs {

	[TestFixture]
	public class CommandSpec : MooGetSpec {

		[TestFixture]
		public class API : MooGetSpec {

			class SomeCommand {
				public SomeCommand() {}
				public SomeCommand(string[] args) {
					var opts = new OptionSet() {
						{ "d|doption",  v => DOption = v != null },
						{ "s|source=",  v => Source = v          }
					};
					var extra = opts.Parse(args);
					Query = extra.First();
				}

				public bool DOption { get; set; }
				public string Source { get; set; }
				public string Query { get; set; }
			}

			[Test]
			public void some_shit_works() {
				SomeCommand defaults = new SomeCommand();
				defaults.DOption.ShouldBeFalse();
				defaults.Source.Should(Be.Null);
				defaults.Query.Should(Be.Null);

				// moo search foo -d --source http://this.that
				SomeCommand cmd = new SomeCommand(new string[]{ "foo", "-d", "--source", "http://this.that" });
				cmd.DOption.ShouldBeTrue();
				cmd.Source.ShouldEqual("http://this.that");
				cmd.Query.ShouldEqual("foo");
			}
			
			[Test][Ignore]
			public void can_be_run() {
			}
			
			[Test][Ignore]
			public void can_take_an_argument() {
			}
			
			[Test][Ignore]
			public void can_take_arguments() {
			}
			
			[Test][Ignore]
			public void can_parse_options_without_values() {
			}
			
			[Test][Ignore]
			public void can_parse_options_with_values() {
			}
		}

		[TestFixture]
		public class Integration : MooGetSpec {

		}
	}
}
