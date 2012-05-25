using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Feedbook.Logging
{
    internal class LogAppender : log4net.Appender.AppenderSkeleton
    {
        private const string LOGS_KEY = "LOGS";

        private static readonly Queue<LogEvent> LogEvents;

        public static int LogCount { get { return LogEvents.Count; } }

        static LogAppender()
        {
            LogEvents = DataStore.GetObject<Queue<LogEvent>>(LOGS_KEY) ?? new Queue<LogEvent>();
            Application.Current.Exit += (sender, e) => DataStore.SaveObject<Queue<LogEvent>>(LOGS_KEY, LogEvents);
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try { Log(LogEvent.From(loggingEvent)); }
            catch { }
        }

        private static void Log(LogEvent logEvent)
        {
            lock (LogEvents)
            {
                LogEvents.Enqueue(logEvent);
                if (LogEvents.Count > Constants.SysConfig.MaxLogs)
                    LogEvents.Dequeue();
            }
        }

        public static LogEvent[] ClearLogs()
        {
            lock (LogEvents)
            {
                var events = LogEvents.ToArray();
                LogEvents.Clear();
                return events;
            }
        }
    }
}
