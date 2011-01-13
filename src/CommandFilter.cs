using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {

	/// <summary>Represents a MooGet middleware-like filter for wrapping Commands</summary>
	public class CommandFilter {

		public CommandFilter InnerFilter { get; set; }

		public MethodInfo Method { get; set; }

		public CommandFilter(MethodInfo method) {
			Method = method;
		}

		public string Name { get { return Method.DeclaringType.FullName + "." + Method.Name; } }
		
		public string Description { get { return CommandFilterAttribute.Description; } }

		CommandFilterAttribute _commandAttribute;
		public CommandFilterAttribute CommandFilterAttribute {
			get {
				if (_commandAttribute == null)
					_commandAttribute = Method.GetCustomAttributes(typeof(CommandFilterAttribute), true)[0] as CommandFilterAttribute;
				return _commandAttribute;
			}
		}

		public override string ToString() {
			return Name;
		}

		public object Invoke(string[] args) {
			return Method.Invoke(null, new object[] { args, InnerFilter });
		}

		public object Invoke(string[] args, CommandFilter command) {
			return Method.Invoke(null, new object[] { args, command });
		}

		public static List<CommandFilter> GetFilters() {
			return GetFilters(Assembly.GetExecutingAssembly());
		}

		public static List<CommandFilter> GetFilters(Assembly assembly) {
			var attrType = typeof(CommandFilterAttribute);
			var commands = new List<CommandFilter>();
			foreach (var type in assembly.GetTypes())
				foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
					if (Attribute.IsDefined(method, attrType))
						commands.Add(new CommandFilter(method));
			return commands;
		}
	}
}
