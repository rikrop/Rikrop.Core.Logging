using System;
using System.Diagnostics.Contracts;
using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        private static readonly object Locker = new object();
        private readonly ILogRecordFormatter _logRecordFormatter;

        public ConsoleLogger()
            : this(new LogRecordFormatter())
        {
        }

        public ConsoleLogger(ILogRecordFormatter logRecordFormatter)
        {
            Contract.Requires<ArgumentNullException>(logRecordFormatter != null);

            _logRecordFormatter = logRecordFormatter;
        }

        public void Log<TRecord>(TRecord record) where TRecord : ILogRecord
        {
            lock (Locker)
            {
                var savedColor = Console.ForegroundColor;

                Console.ForegroundColor = GetColor(record);
                Console.Write(_logRecordFormatter.GetString(record));
                Console.ForegroundColor = savedColor;
            }
        }

        private ConsoleColor GetColor<T>(T record) where T : ILogRecord
        {
            switch (record.LogLevel)
            {
                case LogRecordLevel.Info:
                    return ConsoleColor.Gray;
                case LogRecordLevel.Warning:
                    return ConsoleColor.DarkYellow;
                case LogRecordLevel.Error:
                    return ConsoleColor.Red;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}