using System;

namespace MooGet {
	public static class ExceptionExtensions {

		// Get the first exception that's not a TargetInvocationException (which we get because we Invoke() our commands)
		public static Exception GetNonInvokationException(this Exception ex) {
			return ex.FirstInnerExceptionThatsNot(typeof(System.Reflection.TargetInvocationException));
		}

		public static Exception FirstInnerExceptionThatsNot(this Exception ex, Type exceptionType) {
			Exception inner = ex;
			while (inner.InnerException != null && inner.GetType() == exceptionType)
				inner = inner.InnerException;
			return inner;
		}
	}
}
