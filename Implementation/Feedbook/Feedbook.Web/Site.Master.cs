using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Feedbook.Web.Helper;

namespace Feedbook.Web
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = this.Page.Title + " | " + Constant.Default.ApplicationName;
        }
    }
}
