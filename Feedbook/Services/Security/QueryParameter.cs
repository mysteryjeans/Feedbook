using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Feedbook.Services.Security
{
    internal class QueryParameter : IComparable<QueryParameter>
    {
        private string name = null;
        private string value = null;

        public QueryParameter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
        }

        int IComparable<QueryParameter>.CompareTo(QueryParameter other)
        {
            return this.name == other.name ? string.Compare(this.value, other.value) : string.Compare(this.name, other.name);
        }
    }
}
