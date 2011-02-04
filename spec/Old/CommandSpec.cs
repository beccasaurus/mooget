using System;
using System.Linq;
using System.Reflection;
using MooGet;
using MooGet.Options;
using NUnit.Framework;

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
			public void option_parsing_works() {
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

			// moo foo -x hi there ...
			[Command(Description = "I am a description")]
			public static string FooCommand(string[] args) {
				return "you called foo with: " + string.Join(" ", args);
			}
			
			[Test]
			public void can_be_a_method() {
				var commands = Command.GetCommands(Assembly.GetExecutingAssembly());
				(commands.Count > 0).ShouldBeTrue();

				var command = commands.First(c => c.Name == "foo");
				command.Name.ShouldEqual("foo");
				command.Description.ShouldEqual("I am a description");
				command.Run(new string[] { "hi", "there" }).ShouldEqual("you called foo with: hi there");
			}
		}

		[TestFixture]
		public class Integration : MooGetSpec {

			[Test]
			public void moo_commands() {
				var output = moo("commands");
				output.ShouldContain("commands");
				output.ShouldContain("help");
				output.ShouldContain("Provide help on the 'moo' command");
			}

			[Test][Ignore]
			public void commands_may_be_abbreviated() {
			}
		}
	}
}
