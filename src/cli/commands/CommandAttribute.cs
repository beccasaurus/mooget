using System;

namespace MooGet {

	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute {

		/// <summary>[Command]</summary>
		public CommandAttribute() {
			Debug = false;
		}

		/// <summary>[Command("my command")] is an easy way to mark a method as a command and give it a description</summary>
		public CommandAttribute(string description) : this() {
			Description = description;
		}

		/// <summary>[Command("foo", "An awesome command")] let's you easily pass in a custom name and description.</summary>
		public CommandAttribute(string name, string description) : this(description) {
			Name = name;
		}

		/// <summary>Whether or not this is a command meant only for debugging / MooGet development</summary>
		/// <remarks>If set to true, `moo commands` will NOT list this command.  `moo --debug command` will though.</remarks>
		public bool Debug { get; set; }

		/// <summary>The name to use for this Command.  If null, it will be derived from the method name.</summary>
		public string Name { get; set; }

		/// <summary>Simply description of this command.  Printed out when you run `moo commands`</summary>
		public string Description { get; set; }
	}
}
