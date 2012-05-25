using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Feedbook.Model
{
    [DataContract]
    internal enum PersonRole
    {
        [EnumMember]
        [Description("Managing Editor")]
        ManagingEditor,

        [EnumMember]
        [Description("Webmaster")]
        WebMaster,

        [EnumMember]
        [Description("Author")]
        Author,

        [EnumMember]
        [Description("Contributor")]
        Contributor
    }
}
