using System;
using System.Diagnostics.Contracts;
using System.Text;
using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    /// <summary>
    /// Форматтер оборачивает текст лога в рамку. См. секцию example описания.
    /// </summary>
    /// <example>
    /// <code>
    /// ______{Name}______
    /// {LogText}
    /// ------------------
    /// </code>
    /// </example>
    public class DecoratedLogRecordFormatter : ILogRecordFormatter
    {
        private const int DefaultLogWidth = 80;
        private readonly ILogRecordFormatter _baseFormatter;
        private readonly string _template;

        public DecoratedLogRecordFormatter(string name)
            : this(name, DefaultLogWidth, new LogRecordFormatter())
        {
        }

        public DecoratedLogRecordFormatter(string name,
                                           ILogRecordFormatter baseFormatter)
            : this(name, DefaultLogWidth, baseFormatter)
        {
        }

        public DecoratedLogRecordFormatter(string name,
                                           int logWidth,
                                           ILogRecordFormatter baseFormatter)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentNullException>(baseFormatter != null);
            Contract.Requires<ArgumentException>(logWidth > 0);

            _baseFormatter = baseFormatter;
            _template = GenerateTemplate(name, logWidth);
        }

        public string GetString<T>(T record) where T : ILogRecord
        {
            return string.Format(_template, _baseFormatter.GetString(record));
        }

        private string GenerateTemplate(string componentName, int logWidth)
        {
            var padding = (logWidth - componentName.Length)/2;
            if (padding > 0)
            {
                componentName = componentName.PadLeft(padding + componentName.Length, '_');
            }
            if (componentName.Length < logWidth)
            {
                componentName = componentName.PadRight(logWidth, '_');
            }

            var sb = new StringBuilder(componentName)
                .AppendLine()
                .AppendLine("{0}")
                .AppendLine(new string('-', logWidth));

            return sb.ToString();
        }
    }
}