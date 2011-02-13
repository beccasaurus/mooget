using System;

namespace MooGet {

	/// <summary>Our commands throw this with a message to report a fatal exception that we handled.  We print the message.</summary>
	public class HandledException : Exception {
		public HandledException(string message, params object[] objects) : base(string.Format(message, objects)){}
	}
}
