using System.Text;
using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Logging
{
    public static class LogRecordDataValueFormatter
    {
        public static string GetDataValuesString(this LogRecordDataValue[] dataValues)
        {
            if (dataValues == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            dataValues.WriteDataValuesTo(sb);

            return sb.ToString();
        }

        public static void WriteDataValuesTo(this LogRecordDataValue[] dataValues, StringBuilder sb)
        {
            if (dataValues == null)
            {
                return;
            }

            sb.AppendLine();
            for (int i = 0; i < dataValues.Length; i++)
            {
                var dataValue = dataValues[i];
                WriteDataValueTo(ref dataValue, sb);
            }
        }

        public static void WriteDataValueTo(ref LogRecordDataValue dataValue, StringBuilder sb)
        {
            sb.AppendLine(GetFormattedText(ref dataValue));
            if (dataValue.HasValues)
            {
                for (int i = 0; i < dataValue.Values.Length; i++)
                {
                    var childDataValue = dataValue.Values[i];
                    if (childDataValue.HasValues)
                    {
                        sb.AppendLine();
                    }
                    if (childDataValue.Type == LogRecordDataTypes.Exception)
                    {
                        sb.Append("Inner => ");
                    }
                    WriteDataValueTo(ref childDataValue, sb);
                }
            }
        }

        private static string GetFormattedText(ref LogRecordDataValue logRecordDataValue)
        {
            switch (logRecordDataValue.Type)
            {
                case LogRecordDataTypes.Large:
                case LogRecordDataTypes.StackTrace:
                    return string.Format("{0}:\r\n{1}", logRecordDataValue.Key, logRecordDataValue.Value);
                default:
                    return string.Format("{0} : {1}", logRecordDataValue.Key, logRecordDataValue.Value);
            }
        }
    }
}