using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace Feedbook.Logging
{
    [DataContract]
    internal class LogException
    {
        [DataMember]
        public List<KeyValuePair<string, string>> Data { get; set; }

        [DataMember]
        public string ExceptionType { get; set; }

        [DataMember]
        public string HelpLink { get; set; }

        [DataMember]
        public LogException InnerException { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string ExceptionToString { get; set; }

        public override string ToString()
        {
            return this.ExceptionToString;
        }

        public static LogException From(Exception exception)
        {
            return new LogException
            {
                Data = From(exception.Data),
                HelpLink = exception.HelpLink,
                Message = exception.Message,
                Source = exception.Source,
                StackTrace = exception.StackTrace,
                ExceptionToString = exception.ToString(),
                ExceptionType = exception.GetType().FullName,
                InnerException = exception.InnerException != null ? From(exception.InnerException) : null
            };                      
        }

        private static List<KeyValuePair<string, string>> From(IDictionary data)
        {
            var dataList = new List<KeyValuePair<string, string>>();
            foreach (var key in data.Keys)
                dataList.Add(new KeyValuePair<string, string>(key.ToString(), ("" + data[key])));

            return dataList;
        }
    }
}
