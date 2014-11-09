using System;
using System.Diagnostics.Contracts;
using System.Text;
using Rikrop.Core.Framework;
using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    public class RichLogRecordFormatter : ILogRecordFormatter
    {
        private readonly bool _writeTimestamp;
        private readonly bool _writeSeverity;
        private readonly bool _writeEnvStackTrace;
        private readonly bool _writeExtendedProperties;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RichLogRecordFormatter(bool writeSeverity = true,
                                      bool writeTimestamp = true,
                                      bool writeEnvStackTrace = false,
                                      bool writeExtendedProperties = true)
            :this(new DateTimeProvider(), writeSeverity, writeTimestamp, writeEnvStackTrace, writeExtendedProperties)
        {
        }

        public RichLogRecordFormatter(IDateTimeProvider dateTimeProvider,
                                      bool writeSeverity = true,
                                      bool writeTimestamp = true,
                                      bool writeEnvStackTrace = false,
                                      bool writeExtendedProperties = true)
        {
            Contract.Requires<ArgumentNullException>(dateTimeProvider != null);

            _writeTimestamp = writeTimestamp;
            _writeSeverity = writeSeverity;
            _writeExtendedProperties = writeExtendedProperties;
            _writeEnvStackTrace = writeEnvStackTrace;
            _dateTimeProvider = dateTimeProvider;
        }

        public string GetString<T>(T record) where T : ILogRecord
        {
            var message = record.Message;
            var level = record.LogLevel;

            var stringBuilder = new StringBuilder();

            WriteTimestamp(stringBuilder);

            WriteMessage(stringBuilder, message);

            WriteSeverity(stringBuilder, level);
            WriteExtendedProperties(stringBuilder, record);
            WriteStackTrace(level, stringBuilder);

            return stringBuilder.ToString();
        }

        private void WriteTimestamp(StringBuilder stringBuilder)
        {
            if (_writeTimestamp)
            {
                stringBuilder.AppendFormat("Timestamp: {0}", _dateTimeProvider.Now)
                             .AppendLine();
            }
        }

        private void WriteMessage(StringBuilder stringBuilder, string message)
        {
            stringBuilder.AppendFormat("Message: {0}", message)
                         .AppendLine();
        }

        private void WriteStackTrace(LogRecordLevel level, StringBuilder stringBuilder)
        {
            if (_writeEnvStackTrace)
            {
                if (level == LogRecordLevel.Error || level == LogRecordLevel.Warning)
                {
                    stringBuilder.AppendLine()
                                 .AppendLine()
                                 .AppendFormat("EnvironmentStackTrace:\r\n{0}", GetStackTrace());
                }
            }
        }

        private void WriteSeverity(StringBuilder stringBuilder, LogRecordLevel level)
        {
            if (_writeSeverity)
            {
                stringBuilder.AppendFormat("Severity: {0}", level)
                             .AppendLine();
            }
        }

        private string GetStackTrace()
        {
            var stackTrace = Environment.StackTrace;

            return stackTrace;
        }

        private void WriteExtendedProperties<T>(StringBuilder sb, T data) where T : ILogRecord
        {
            if (_writeExtendedProperties)
            {
                if (!data.HasDataValues)
                {
                    return;
                }

                data.DataValues.WriteDataValuesTo(sb);
            }
        }
    }
}