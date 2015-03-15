using System;
using System.Diagnostics;

namespace ANIDataAggregationLibrary.Util
{
    /// <summary>
    /// A logging class that outputs to the event log if configured or the console otherwise.
    /// </summary>
    public class ServiceLogger
    {
        private readonly EventLog _eventLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        public ServiceLogger() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="eventLog">The event log.</param>
        public ServiceLogger(EventLog eventLog)
        {
            this._eventLog = eventLog;
        }

        /// <summary>
        /// Logs the specified message to the event log.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string message)
        {
            if (_eventLog != null)
            {
                _eventLog.WriteEntry(message, EventLogEntryType.Information);
            }
            else
            {
                Console.WriteLine("INFO: {0}", message);
            }
        }

        /// <summary>
        /// Logs a warning message to the event log.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            if (_eventLog != null)
            {
                _eventLog.WriteEntry(message, EventLogEntryType.Warning);
            }
            else
            {
                Console.WriteLine("WARN: {0}", message);
            }
        }

        /// <summary>
        /// Logs the specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            if (_eventLog != null)
            {
                _eventLog.WriteEntry(message, EventLogEntryType.Error);
            }
            else
            {
                Console.WriteLine("ERROR: {0}", message);
            }
        }
    }
}