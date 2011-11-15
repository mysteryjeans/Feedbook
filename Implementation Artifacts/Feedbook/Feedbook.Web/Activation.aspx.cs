using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Feedbook.Web.Helper;

namespace Feedbook.Web
{
    public partial class Activation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbMessage.Visible = this.lbErrorMessage.Visible = false;
            if (!this.IsPostBack)
            {
                try
                {
                    Model.User.Activate(this.Request.QueryString["username"], this.Request.QueryString["code"]);
                    this.lbMessage.Text = "Your account is successfully activated! please click <a href=\"Login.aspx\">here</a> to login.";
                    this.lbMessage.Visible = true;
                }
                catch (FBException ex)
                {
                    this.lbErrorMessage.Text = ex.Message;
                    this.lbErrorMessage.Visible = true;
                }
                catch (Exception ex)
                {
                    this.LogAndRedirectToErrorPage(ex);
                }
            }
        }
    }
}