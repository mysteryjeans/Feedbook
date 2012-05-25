using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreSystem.Util;
using System.IO;

namespace Feedbook.Services.Security
{
    internal class PostData
    {
        public Stream Binary { get; private set; }

        public string Content { get; private set; }

        public string ContentType { get; private set; }

        public Dictionary<string, string> Headers { get; private set; }

        public bool IsBinary { get { return string.IsNullOrEmpty(Content); } }

        public long ContentLength
        {
            get
            {
                return (this.Binary != null) ? this.Binary.Length : this.Content.Length;
            }
        }

        private PostData()
        {
            this.Headers = new Dictionary<string, string>();
        }

        public PostData(string content, string contentType)
            : this()
        {
            Guard.CheckNullOrEmpty(content, "PostData(content)");
            Guard.CheckNullOrEmpty(contentType, "PostData(contentType)");

            this.Content = content;
            this.ContentType = contentType;
        }

        public PostData(Stream dataStream, string contentType)
            : this()
        {
            Guard.CheckNull(dataStream, "PostData(dataStream)");
            Guard.CheckNullOrEmpty(contentType, "PostData(contentType)");

            this.Binary = dataStream;
            this.ContentType = contentType;
        }
    }
}
