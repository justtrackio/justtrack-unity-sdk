using NUnit.Framework;
using JustTrack;
using UnityEngine;
using UnityEngine.TestTools;

namespace JustTrack.Tests.Editor
{
    public class ExceptionHandlerTests
    {
        [Test]
        public void OnReceiveException_InvokedOnException()
        {
            var handler = new ExceptionHandler();
            string? receivedMessage = null;

            handler.OnReceiveException += (message, stackTrace) =>
            {
                receivedMessage = message;
            };

            LogAssert.Expect(LogType.Exception, "Exception: test exception");
            Debug.LogException(new System.Exception("test exception"));

            Assert.That(receivedMessage, Is.Not.Null);
            Assert.That(receivedMessage, Does.Contain("test exception"));
        }

        [Test]
        public void OnReceiveException_NotInvokedOnNonException()
        {
            var handler = new ExceptionHandler();
            bool wasInvoked = false;

            handler.OnReceiveException += (message, stackTrace) =>
            {
                wasInvoked = true;
            };

            Debug.Log("regular log message");

            Assert.That(wasInvoked, Is.False);
        }

        [Test]
        public void OnReceiveException_NotInvokedWhenNoSubscribers()
        {
            var handler = new ExceptionHandler();
            LogAssert.Expect(LogType.Exception, "Exception: no subscriber");
            Assert.DoesNotThrow(() => Debug.LogException(new System.Exception("no subscriber")));
        }
    }
}
