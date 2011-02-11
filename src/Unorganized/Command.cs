using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {

	/// <summary>Represents a MooGet command.</summary>
	/// <remarks>This is not meant to be inherited from.  To implement a command, put the [Command] attribute on a class or method.</remarks>
	public class Command {
		public bool       Debug  = false;
		public string     Name   { get; set; }
		public MethodInfo Method { get; set; }

		public Command(MethodInfo method) {
			Method = method;
			Name   = CommandAttribute.Name ?? Regex.Replace(method.Name, "Command$", "").ToLower();
		}
		
		public string Description { get { return CommandAttribute.Description; } }

		CommandAttribute _commandAttribute;
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

		public static List<Command> GetCommands() {
			return GetCommands(Assembly.GetExecutingAssembly());
		}

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
