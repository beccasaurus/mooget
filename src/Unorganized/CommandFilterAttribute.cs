using System;

namespace MooGet {

	[AttributeUsage(AttributeTargets.Method)]
	public class CommandFilterAttribute : Attribute {
		public string Description { get; set; }

		public CommandFilterAttribute(){}
		public CommandFilterAttribute(string description){
			Description = description;
		}
	}
}
