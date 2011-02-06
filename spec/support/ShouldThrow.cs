using System;

namespace NUnit.Framework {

	// This should probably be in NUnit.Should ...
    public class Should {

        // Should.Throw<SpecialException>(() => { ... })
        public static void Throw<T>(Action action) {
            Throw<T>(null, action);
        }

        // Should.Throw<SpecialException>("BOOM!", () => { ... })
        public static void Throw<T>(string messagePart, Action action) {
            Throw(action, messagePart, typeof(T));
        }

        // Should.Throw("BOOM!", () => { ... })
        public static void Throw(string messagePart, Action action) {
            Throw(action, messagePart);
        }

        // Should.Throw(() => { ... })
        // Should.Throw(() => { ... }, "BOOM!")                           // <--- Throw(Message, Action) is preferred
        // Should.Throw(() => { ... }, "BOOM!", typeof(SpecialException)) // <--- Throw<T>(Message) is preferred
        public static void Throw(Action action, string messagePart = null, Type exceptionType = null) {
            try {
                action.Invoke();
                Assert.Fail("Expected Exception to be thrown, but none was.");
            } catch (Exception ex) {
                // NOTE: Sometimes, this might be a TargetInvocationException, in which case 
                //       the *actual* exception thrown will be ex.InnerException.
                //       If I run into that circumstance again, I'll update the code to reflect this.

                // check exception type, if provided
                if (exceptionType != null)
                    if (!exceptionType.IsAssignableFrom(ex.GetType()))
                        Assert.Fail("Expected Exception of type {0} to be thrown, but got an Exception of type {1}", exceptionType, ex.GetType());

                // check exception message part, if provided
                if (messagePart != null)
                    if (! ex.Message.Contains(messagePart))
                        Assert.Fail("Expected {0} Exception to be thrown with a message containing {1}, but message was: {2}",
                            exceptionType, messagePart, ex.Message);
            }
        }
    }
}
