using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Feedbook.Helper;

namespace Feedbook.Logging
{
    [DataContract]
    internal class LogReport
    {
        [DataMember]
        public string App { get; set; }

        [DataMember]
        public string AppAssembly { get; set; }

        [DataMember]
        public string OSVersion { get; set; }

        [DataMember]
        public string CLRVersion { get; set; }

        [DataMember]
        public DateTime ReportDate { get; set; }

        [DataMember]
        public LogEvent[] LogEvents { get; set; }

        public static LogReport CreateReport(LogEvent[] logEvents)
        {
            return new LogReport
            {
                App = Constants.APPLICATION_NAME,
                AppAssembly = Util.ApplicationAssembly(),
                OSVersion = Environment.OSVersion.ToString(),
                CLRVersion = Environment.Version.ToString(),
                ReportDate = DateTime.Now,
                LogEvents = logEvents
            };
        }

        public static string CreateReport()
        {
            return Util.Serialize(CreateReport(LogAppender.ClearLogs()));
        }
    }
}
