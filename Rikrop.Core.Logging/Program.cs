using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using Rikrop.Core.Framework;
using Rikrop.Core.Framework.Logging;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main()
        {
            //var max = Math.Pow(10, 7) * 5;
            //var step = (int)max/10;
            //var logger = new Logger();
            //var sw = Stopwatch.StartNew();
            //for (int i = 0; i < max; i++)
            //{
            //    if (i%step == 0)
            //    {
            //        Console.WriteLine(i);
            //    }
            //    logger.Log(LogRecord.CreateInfo("Hello world!",
            //                                    LogRecordDataValue.CreateSimple("Key", "Value"),
            //                                    LogRecordDataValue.CreateSimple("Key", "Value", LogRecordDataValue.CreateSimple("Key", "Value"))));
            //}
            //Exception exception = null;

            var logger = new ConsoleLogger();
            try
            {
                Test();
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                logger.LogWarning(ex.ToString());
            }

            //LogException(exception);
            
            //sw.Stop();
            //Console.WriteLine("Done! " + sw.Elapsed);

            //sw.Restart();
            //GC.Collect();
            //Console.WriteLine("GC: " + sw.Elapsed);

            Console.ReadLine();
        }

        private static void LogException(Exception exception)
        {
            
        }

        static void Test()
        {
            try
            {
                Test2();
            }
            catch (Exception ex)
            {
                throw new Exception("Fuck!", ex);
            }
        }

        static void Test2()
        {
            try
            {
                Test3();
            }
            catch (Exception ex)
            {
                throw new Exception("This early morning!", ex);
            }
        }

        static void Test3()
        {
            throw new Exception("Hello world");
        }
    }

    public class Logger : ILogger
    {
        private ConcurrentQueue<LogRecord> _logRecords = new ConcurrentQueue<LogRecord>();
        private RepeatableExecutor _repeatableExecutor;

        public Logger()
        {
            _repeatableExecutor = new RepeatableExecutor(()=>Flush(), TimeSpan.FromSeconds(1));
            _repeatableExecutor.Start();
        }

        private void Flush()
        {
            LogRecord logRecord;
            while (_logRecords.TryDequeue(out logRecord))
            {
                
            }
        }

        public void Log<TRecord>(TRecord record) where TRecord : ILogRecord
        {
            _logRecords.Enqueue(new LogRecord(record.Message, record.LogLevel, record.DataValues));
        }
    }
}
