using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feedbook.Web
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbErrorMessage.Visible = false;
            if (!this.IsPostBack && !string.IsNullOrWhiteSpace(this.Request.QueryString["message"]))
            {
                this.lbErrorMessage.Text = this.Request.QueryString["message"];
                this.lbErrorMessage.Visible = true;
            }
        }
    }
}