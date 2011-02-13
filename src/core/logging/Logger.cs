using System;
using System.IO;

namespace MooGet {

	/// <summary>Little class for writing logs to Console.Out if Moo.Verbose is set to true</summary>
	/// <remarks>If necessary, we might eventually swap this out with a "real" logger, but this works for now.</remarks>
	public class Logger : ILogger {

		public Logger() : this(Console.Out) {}

		public Logger(TextWriter logWriter) {
			Out = logWriter;
		}

		/// <summary>The TextWriter that our logs write to.  Defaults to Console.Out.</summary>
		public TextWriter Out { get; set; }

		public void Debug(string message, params object[] objects){ Log("DEBUG", message, objects); }
		public void Info( string message, params object[] objects){ Log("INFO",  message, objects); }
		public void Warn( string message, params object[] objects){ Log("WARN",  message, objects); }
		public void Error(string message, params object[] objects){ Log("ERROR", message, objects); }
		public void Fatal(string message, params object[] objects){ Log("FATAL", message, objects); }

		public void Log(string level, string message, params object[] objects) {
			if (Moo.Verbose)
				Out.WriteLine(string.Format("[{0}] ", level) + message, objects);
		}
	}
}
