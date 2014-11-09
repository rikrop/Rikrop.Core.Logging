using System.Text;
using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    public class LogRecordFormatter : ILogRecordFormatter
    {
        public string GetString<T>(T record) where T : ILogRecord
        {
            if (!record.HasDataValues)
            {
                return record.Message;
            }

            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(record.Message))
            {
                sb.AppendFormat("{0}\r\n", record.Message);
            }

            record.DataValues.WriteDataValuesTo(sb);

            return sb.ToString();
        }
    }
}