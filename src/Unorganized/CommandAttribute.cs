using System;

namespace MooGet {

	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute {
		public bool   Debug       = false;
		public string Name        { get; set; }
		public string Description { get; set; }

		public CommandAttribute(){}
		public CommandAttribute(string description){
			Description = description;
		}
	}
}
