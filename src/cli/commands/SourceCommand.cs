using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet.Commands {

	///<summary>moo source</summary>
	public class SourceCommand : MooGetCommand {

		[Command("source", "Manage the sources Moo uses to search for packages")]
		public static object Run(string[] args) { return new SourceCommand(args).Run(); }

		public SourceCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments    = false;
			RunFirstArgumentAsCommand = true;
		}

		/// <summary>moo source</summary>
		public override object RunDefault() {
			var sources = Moo.Dir.Sources;
			if (sources == null || sources.Count == 0) {
				Output.Line("No sources");
			} else {
				var spaces = Moo.Dir.Sources.Select(src => src.Name.SafeString().Length).Max();
				spaces     = (spaces == 0) ? 0 : spaces + 2;

				Output.Line("*** CURRENT SOURCES ***\n");
				foreach(var source in Moo.Dir.Sources)
					Output.Line("{0}{1}", source.Name.WithSpaces(spaces), source.Path);
			}
			return Output;
		}

		/// <summary>moo source [add|rm]</summary>
		public override object RunCommand(string commandName) {
			if (Args.Count == 0 || Args.Count > 2) return Help;
			switch (commandName) {
				case "add": return Add();    break;
				case "rm":  return Remove(); break;
				default:    throw new Exception("Unknown command: " + commandName);
			}
		}

		public object Add() {
			var sourceName = (Args.Count == 1) ? null         : Args.First();
			var sourceUrl  = (Args.Count == 1) ? Args.First() : Args.Last();

			var source = Moo.Dir.Initialize().SourceFile.Add(sourceName, sourceUrl);

			return (source == null)
				? string.Format("Problem adding source: {0}\n", sourceUrl)
				: string.Format("Added source: {0}\n", source.Path);
		}

		public object Remove() {
			var sourceCount = Moo.Dir.Sources.Count;
			var source      = Args.First();

			Moo.Dir.SourceFile.Remove(source);

			return (sourceCount == Moo.Dir.Sources.Count)
				? string.Format("Source not found: {0}\n", source)
				: string.Format("Removed source: {0}\n",   source);
		}
	}
}
