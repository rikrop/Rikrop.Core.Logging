using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    public class EventLogLogger : ILogger
    {
        //private const int MaxEventLogItemSize = 0x7ffe; //32766
        private const int MaxEventLogItemSize = 20000; //Максимум - 32766, но это размер целого EventLogEntry, с текстом и параметрами. Т.к. параметры мы посчитать не можем, берём с запасом.
        private readonly ILogRecordFormatter _logRecordFormatter;
        private readonly string _logSourceName;

        public EventLogLogger()
            :this(new LogRecordFormatter())
        {
        }

        public EventLogLogger(ILogRecordFormatter logRecordFormatter)
            : this("Application", "Application", logRecordFormatter)
        {
        }

        public EventLogLogger(string logAndSourceName)
            :this(logAndSourceName, logAndSourceName)
        {
            
        }

        public EventLogLogger(string logAndSourceName, ILogRecordFormatter logRecordFormatter)
            : this(logAndSourceName, logAndSourceName, logRecordFormatter)
        {

        }

        public EventLogLogger(string logSourceName,
                              string logName)
            : this(logSourceName, logName, new LogRecordFormatter())
        {
        }

        public EventLogLogger(string logSourceName,
                              string logName,
                              ILogRecordFormatter logRecordFormatter)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(logSourceName));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(logName));
            Contract.Requires<ArgumentNullException>(logRecordFormatter != null);

            _logSourceName = logSourceName;
            _logRecordFormatter = logRecordFormatter;

            if (!string.IsNullOrWhiteSpace(logName) && !string.IsNullOrWhiteSpace(logSourceName))
            {
                CheckEventLogExists(logSourceName, logName);
            }
        }

        public void Log<TRecord>(TRecord record) where TRecord : ILogRecord
        {
            var messages = GetMessages(record);

            foreach (var message in messages)
            {
                EventLog.WriteEntry(_logSourceName, message, GetEventLogType(record.LogLevel));
            }
        }

        private IEnumerable<string> GetMessages<TRecord>(TRecord record) where TRecord : ILogRecord
        {
            var message = _logRecordFormatter.GetString(record);

            if (message.Length < MaxEventLogItemSize)
                return new[] {message};

            var startIndex = 0;
            bool hasSomethingToCut;
            var messageList = new List<string>();
            do
            {
                var amountToCut = GetAmountToCut(message, startIndex);
                var part = message.Substring(startIndex, amountToCut);

                hasSomethingToCut = !string.IsNullOrWhiteSpace(part);
                if (hasSomethingToCut)
                {
                    messageList.Add(part);
                }

                startIndex += amountToCut;
            } while (hasSomethingToCut);

            return messageList.ToArray();
        }

        private int GetAmountToCut(string message, int startIndex)
        {
            var leftoverLength = message.Length - startIndex;
            return Math.Min(leftoverLength, MaxEventLogItemSize);
        }

        private static void CheckEventLogExists(string logSourceName, string logName)
        {
            if (logName.ToLower() == "application")
            {
                return;
            }

            var associatedLogName = EventLog.LogNameFromSourceName(logSourceName, ".");

            if (EventLog.SourceExists(logSourceName) && 
                (!EventLog.Exists(logName) || associatedLogName.ToLower() != logName.ToLower()))
            {
                //Если имя источника совпадает с именем лога, то необходимо удалить сам лог.
                if (EventLog.Exists(logSourceName))
                {
                    EventLog.Delete(logSourceName);
                }
                else
                {
                    EventLog.DeleteEventSource(logSourceName);
                }
                EventLog.CreateEventSource(logSourceName, logName);
            }

            if (!EventLog.SourceExists(logSourceName))
            {
                EventLog.CreateEventSource(logSourceName, logName);
            }
        }

        private EventLogEntryType GetEventLogType(LogRecordLevel logLevel)
        {
            switch (logLevel)
            {
                case LogRecordLevel.Info:
                    return EventLogEntryType.Information;
                case LogRecordLevel.Warning:
                    return EventLogEntryType.Warning;
                case LogRecordLevel.Error:
                    return EventLogEntryType.Error;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}