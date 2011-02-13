using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Options;

namespace MooGet {

	/// <summary>Useful baseclass that we use for implementing our commands</summary>
	public abstract class MooGetCommand {

		/// <summary>Private default MooGetCommand constructor.  Sets property defaults.</summary>
		MooGetCommand() {
			PrintHelpIfNoArguments = false;
		}

		public MooGetCommand(string[] args) : this() {
			Args = new List<string>(args);
		}

		StringBuilder _output;

		/// <summary>
		/// If true, we print Help when no arguments are called. 
		/// If false, we call RunDefault() when called with no arguments.
		/// </summary>
		public virtual bool PrintHelpIfNoArguments { get; set; }

		/// <summary>Command line arguments passed to this command</summary>
		public virtual List<string> Args { get; set; }

		/// <summary>A default StringBuilder that you can use for your command output</summary>
		public virtual StringBuilder Output {
			get { return this.Lazy(ref _output); }
			set { _output = value;               }
		}

		/// <summary>Gets the command name from the class name, eg. SourceCommand will be "source".</summary>
		public virtual string CommandName {
			get { return Regex.Replace(this.GetType().Name.ToLower(), @"command$", ""); }
		}

		/// <summary>This command's Help documentations (from resources)</summary>
		public virtual object Help { get { return Util.HelpForCommand(CommandName); } }

		/// <summary>Runs this command, printing out the help documentation if no arguments or --help passed</summary>
		public virtual object Run() {
			if (Args.Count == 0)
				return PrintHelpIfNoArguments ? Help : RunDefault();
			else if (Args.First() == "--help")
				return Help;
			else {
				var command = Args.First();
				Args.RemoveAt(0);
				return RunCommand(command);
			}
		}

		/// <summary>Override this to have 'moo X foo' call RunCommand('foo') on your XCommand</summary>
		public virtual object RunCommand(string commandName) {
			throw new NotImplementedException("You need to override RunCommand in your MooGetCommand");
		}

		/// <summary>Override this to have 'moo X' call RunDefault() on your XCommand</summary>
		public virtual object RunDefault() {
			throw new NotImplementedException("You need to override RunDefault in your MooGetCommand");
		}
	}
}

		/*
		var opts = new OptionSet() {
			{ "d|doption",  v => DOption = v != null },
			{ "s|source=",  v => Source = v          }
		};
		var extra = opts.Parse(args);
		Query = extra.First();
		 */
