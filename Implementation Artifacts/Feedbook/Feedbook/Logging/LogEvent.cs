using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using log4net.Core;

namespace Feedbook.Logging
{
    [DataContract]
    internal class LogEvent
    {
        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public LogException ExceptionObject { get; set; }

        [DataMember]
        public FixFlags Fix { get; set; }

        [DataMember]
        public string Identity { get; set; }

        [DataMember]
        public Level Level { get; set; }

        [DataMember]
        public LocationInfo LocationInformation { get; set; }

        [DataMember]
        public string LoggerName { get; set; }

        [DataMember]
        public string MessageObject { get; set; }

        [DataMember]
        public string RenderedMessage { get; set; }

        [DataMember]
        public string ThreadName { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        public string UserName { get; set; }

        public static LogEvent From(LoggingEvent loggingEvent)
        {
            return new LogEvent
            {
                Domain = loggingEvent.Domain,
                ExceptionObject = LogException.From(loggingEvent.ExceptionObject),
                Fix = loggingEvent.Fix,
                Identity = loggingEvent.Identity,
                Level = loggingEvent.Level,
                LocationInformation = loggingEvent.LocationInformation,
                LoggerName = loggingEvent.LoggerName,                
                RenderedMessage = loggingEvent.RenderedMessage,
                ThreadName = loggingEvent.ThreadName,
                TimeStamp = loggingEvent.TimeStamp,
                UserName = loggingEvent.UserName,
                MessageObject = loggingEvent.MessageObject != null ? loggingEvent.MessageObject.ToString() : null
            };
        }
    }
}
