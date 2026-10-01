using UnityEngine;

namespace JustTrack
{
    /// <summary>
    /// Handles Unity application exceptions for the justtrack SDK.
    /// </summary>
    public class ExceptionHandler
    {
        /// <summary>
        /// Delegate for handling exception events.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="stackTrace">The exception stack trace.</param>
        public delegate void OnReceiveExceptionHandler(string message, string stackTrace);

        /// <summary>
        /// Event raised when an exception is received.
        /// </summary>
        public event OnReceiveExceptionHandler? OnReceiveException;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        public ExceptionHandler()
        {
            Application.logMessageReceived += HandleLog;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        ~ExceptionHandler()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                OnReceiveException?.Invoke(logString, stackTrace);
            }
        }
    }
}
