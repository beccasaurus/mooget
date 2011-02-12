using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MooGet {

	/// <summary>Represents a MooGet middleware-like filter for wrapping Commands</summary>
	/// <remarks>
	/// This is not meant to be inherited from.  To implement a command filter, put the [CommandFilter] attribute on a method.
	/// </remarks>
	public class CommandFilter {

		public CommandFilter(MethodInfo method) {
			Method = method;
		}

		CommandFilterAttribute _commandAttribute;
		static List<CommandFilter> _filters;

		public CommandFilter InnerFilter  { get; set; }
		public MethodInfo    Method       { get; set; }
		public AssemblyName  AssemblyName { get { return Method.DeclaringType.Assembly.GetName(); } }
		public string        Name         { get { return Method.Name; } }
		public string        FullName     { get { return Method.DeclaringType.FullName + "." + Method.Name; } }
		public string        Description  { get { return CommandFilterAttribute.Description; } }

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

		/// <summary>Returns a List of CommandFilter found in the executing assembly (moo.exe)</summary>
		public static List<CommandFilter> GetFilters() {
			return GetFilters(Assembly.GetExecutingAssembly());
		}

		/// <summary>Returns a List of CommandFilter found in the given Assembly</summary>
		public static List<CommandFilter> GetFilters(Assembly assembly) {
			var attrType = typeof(CommandFilterAttribute);
			var commands = new List<CommandFilter>();
			foreach (var type in assembly.GetTypes())
				foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
					if (Attribute.IsDefined(method, attrType))
						commands.Add(new CommandFilter(method));
			return commands;
		}

		/// <summary>Returns a list of *ALL* CommandFilter for moo.  Loops thru Moo.Extensions to find and load filters.</summary>
		public static List<CommandFilter> Filters {
			get {
				if (_filters == null) {
					_filters = new List<CommandFilter>();
					foreach (var assembly in Moo.Extensions)
						_filters.AddRange(CommandFilter.GetFilters(assembly));
					_filters.AddRange(CommandFilter.GetFilters()); // currently executing assembly
				}
				return _filters;
			}
			set { _filters = value; }
		}
	}
}
