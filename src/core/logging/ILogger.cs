using System;

namespace MooGet {

	/// <summary>Typical ILogger interface</summary>
	public interface ILogger {
		void Debug(string message, params object[] objects);
		void Info( string message, params object[] objects);
		void Warn( string message, params object[] objects);
		void Error(string message, params object[] objects);
		void Fatal(string message, params object[] objects);
	}
}
