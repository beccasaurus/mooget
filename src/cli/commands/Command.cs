using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {

	// TODO if Description isn't provided, it should grab the Summary: from the command's help documentation ... ideally ... to keep it DRY ?

	/// <summary>Represents a MooGet command.</summary>
	/// <remarks>This is not meant to be inherited from.  To implement a command, put the [Command] attribute on a method.</remarks>
	public class Command {

		/// <summary>Constructor.  A command *must* be associated with a MethodInfo.</summary>
		public Command(MethodInfo method) {
			Method = method;
			Name   = CommandAttribute.Name ?? Regex.Replace(method.Name, "Command$", "").ToLower();
		}

		CommandAttribute _commandAttribute;

		/// <summary>Whether or not this is a command meant only for debugging / MooGet development</summary>
		/// <remarks>If set to true, `moo commands` will NOT list this command.  `moo --debug command` will though.</remarks>
		public bool Debug { get { return CommandAttribute.Debug; } }

		/// <summary>This command's name (which is used to execute it)</summary>
		public string Name { get; set; }

		/// <summary>The MethodInfo implementation associated with this command</summary>
		public MethodInfo Method { get; set; }
		
		/// <summary>Simply description of this command.  Printed out when you run `moo commands`</summary>
		public string Description { get { return CommandAttribute.Description; } }

		/// <summary>The [Command] attribute instance that is on this Command's Method</summary>
		public CommandAttribute CommandAttribute {
			get {
				if (_commandAttribute == null)
					_commandAttribute = Method.GetCustomAttributes(typeof(CommandAttribute), true)[0] as CommandAttribute;
				return _commandAttribute;
			}
		}

		// TODO rename to Invoke
		public object Run(string[] args) {
			return Method.Invoke(null, new object[] { args });
		}

		public override string ToString() {
			return string.Format("Command(name:{0}, description:{1})", Name, Description);
		}

		/// <summary>Helper methods.  Returns all commands in the currently executing assembly.</summary>
		public static List<Command> GetCommands() {
			return GetCommands(Assembly.GetExecutingAssembly());
		}

		/// <summary>Get all commands defined in the given Assembly</summary>
		public static List<Command> GetCommands(Assembly assembly) {
			var attrType = typeof(CommandAttribute);
			var commands = new List<Command>();
			foreach (var type in assembly.GetTypes())
				foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
					if (Attribute.IsDefined(method, attrType))
						commands.Add(new Command(method));
			return commands;
		}
	}
}
