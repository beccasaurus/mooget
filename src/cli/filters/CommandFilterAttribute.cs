using System;

namespace MooGet {

	[AttributeUsage(AttributeTargets.Method)]
	public class CommandFilterAttribute : Attribute {

		/// <summary>[CommandFilter]</summary>
		public CommandFilterAttribute(){}

		/// <summary>[CommandFilter("my filter")] lets you add a description that will be printed out when running `moo filters`</summary>
		public CommandFilterAttribute(string description) : this() {
			Description = description;
		}

		/// <summary>Short description (optional) that will be printed out when running `moo filters`</summary>
		public string Description { get; set; }
	}
}
