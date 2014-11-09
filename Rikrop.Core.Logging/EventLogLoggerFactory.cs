using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    public class EventLogLoggerFactory : ILoggerFactory
    {
        private readonly string _eventLogName;
        private readonly string _eventLogSource;
        private readonly ILogRecordFormatter _logRecordFormatter;

        public EventLogLoggerFactory()
            : this(null, null, null)
        {
        }

        public EventLogLoggerFactory(string logAndSourceName)
            : this(logAndSourceName, logAndSourceName, null)
        {
        }

        public EventLogLoggerFactory(string logAndSourceName, ILogRecordFormatter logRecordFormatter)
            : this(logAndSourceName, logAndSourceName, logRecordFormatter)
        {
        }

        public EventLogLoggerFactory(ILogRecordFormatter logRecordFormatter)
            : this(null, null, logRecordFormatter)
        {
        }

        public EventLogLoggerFactory(string eventLogSource,
                                     string eventLogName,
                                     ILogRecordFormatter logRecordFormatter)
        {
            if (string.IsNullOrWhiteSpace(eventLogName))
            {
                eventLogName = eventLogSource;
            }

            _eventLogName = eventLogName;
            _eventLogSource = eventLogSource;
            _logRecordFormatter = logRecordFormatter ?? new LogRecordFormatter();
        }

        public ILogger CreateForSource(string logSource)
        {
            //≈сли наименовани€ лога не передали и не закрепили через конструктор, то создаЄм дефолтный
            if (string.IsNullOrWhiteSpace(logSource) &&
                string.IsNullOrWhiteSpace(_eventLogName) &&
                string.IsNullOrWhiteSpace(_eventLogSource))
            {
                return new EventLogLogger(_logRecordFormatter);
            }

            //≈сли наименование лога не передали, но через конструктор закрепили, используем имена конструктора
            if (string.IsNullOrWhiteSpace(logSource))
            {
                var eventLogSource = _eventLogSource;
                if (string.IsNullOrWhiteSpace(eventLogSource))
                {
                    eventLogSource = _eventLogName;
                }
                return new EventLogLogger(eventLogSource, _eventLogName, _logRecordFormatter);
            }

            //≈сли наименование лога передали, но не закрепили через конструктор источник, то создаЄм источник в закреплЄнном журнале
            if (string.IsNullOrWhiteSpace(_eventLogSource))
            {
                return new EventLogLogger(logSource, _eventLogName, _logRecordFormatter);
            }

            //≈сли наименование лога передали, но не закрепили через конструктор, то создаЄм журнал и источник с таким именем
            if (string.IsNullOrWhiteSpace(_eventLogName))
            {
                return new EventLogLogger(logSource, logSource, _logRecordFormatter);
            }

            //≈сли наименование лога передали и всЄ закреплено, вставл€ем декоратор, чтобы выделить переданное наименование.
            return new EventLogLogger(_eventLogSource, _eventLogName, new DecoratedLogRecordFormatter(logSource, _logRecordFormatter));
        }
    }
}