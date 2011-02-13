using System;
using System.Linq;
using System.Reflection;
using MooGet;
using Mono.Options;
using NUnit.Framework;

namespace MooGet.Specs.CLI {

	[TestFixture]
	public class CommandFilterSpec : MooGetSpec {

		[CommandFilter]
		public static object HeaderAndFooterFilter(string[] args, CommandFilter command) {
			return "-----\n" + command.Invoke(args, command).ToString() + "\n-------\n";
		}

		[CommandFilter("I am the description ...")]
		public static object FindAndRunCommand(string[] args, CommandFilter command) {
			return "I am the result of running the app";
		}

		[Test]
		public void can_find_command_filters() {
			var filters = CommandFilter.GetFilters(Assembly.GetExecutingAssembly());
			(filters.Count > 1).ShouldBeTrue();
			
			filters.Select(f => f.FullName).ShouldContain("MooGet.Specs.CLI.CommandFilterSpec.FindAndRunCommand");
			filters.Select(f => f.FullName).ShouldContain("MooGet.Specs.CLI.CommandFilterSpec.HeaderAndFooterFilter");
			filters.Select(f => f.Name).ShouldContain("FindAndRunCommand");
			filters.Select(f => f.Name).ShouldContain("HeaderAndFooterFilter");
			filters.Select(f => f.Description).ShouldContain("I am the description ...");
		}

		[Test]
		public void can_invoke_command_filters() {
			var filters      = CommandFilter.GetFilters(Assembly.GetExecutingAssembly());
			var findAndRun   = filters.First(f => f.Name == "FindAndRunCommand");
			var headerFooter = filters.First(f => f.Name == "HeaderAndFooterFilter");

			findAndRun.Invoke(null, null).ToString().ShouldEqual("I am the result of running the app");
			headerFooter.Invoke(null, findAndRun).ToString().ShouldEqual("-----\nI am the result of running the app\n-------\n");
		}
	}
}
