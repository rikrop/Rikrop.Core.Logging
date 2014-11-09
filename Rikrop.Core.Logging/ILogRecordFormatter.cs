using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    public interface ILogRecordFormatter
    {
        string GetString<T>(T record) where T : ILogRecord;
    }
}