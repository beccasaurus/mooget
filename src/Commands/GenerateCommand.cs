using System;
using System.IO;

namespace MooGet.Commands {

	public class GenerateCommand {

		[Command(Name = "generate", Description = "Generated a template")]
		public static object Run(string[] args) {
			// TODO later, we'll make it so this is easy to extend.  for now, we just support generating nuspec files
			if (args[0] != "nuspec")
				return string.Format("Unknown template: {0}", args[0]);

			var dirname     = Path.GetFileName(Directory.GetCurrentDirectory());
			var filename    = dirname + ".nuspec";
			var path        = Path.Combine(Directory.GetCurrentDirectory(), filename);
			var now         = Feed.Format(DateTime.Now);
			var version     = "1.0.0.0";
			var description = "";
			var author      = "me";
			var xml         = new XmlBuilder();

			xml.StartElement("package").
				StartElement("metadata").	
					WriteElement("id",          dirname).
					WriteElement("version",     version).
					WriteElement("description", description).
					StartElement("authors").
						WriteElement("author", author).
					EndElement().
					WriteElement("created",  now).
					WriteElement("modified", now).
				EndElement().
			EndElement();

			Util.WriteFile(path, xml.ToString());
			return string.Format("Generated {0}", filename);
		}
	}
}
