using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Feedbook.Web.Helper;

namespace Feedbook.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbMessage.Visible = this.lbErrorMessage.Visible = false;
            RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);

            if (!this.IsPostBack)
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    this.Session[Constant.Session.UserName] = this.User.Identity.Name;
                    FormsAuthentication.RedirectFromLoginPage(this.User.Identity.Name, false);
                }
                else if (this.Session[Constant.Session.UserName] != null)
                {
                    this.lbMessage.Text = Constant.Message.SessionExpired;
                    this.lbMessage.Visible = true;
                }

                if (!string.IsNullOrWhiteSpace(this.Request.QueryString["message"]))
                {
                    this.lbMessage.Text = this.Request.QueryString["message"];
                    this.lbMessage.Visible = true;
                }
            }
        }

        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            try
            {
                e.Authenticated = Model.UserCredential.Authenticate(this.LoginUser.UserName, this.LoginUser.Password);             
                this.Session[Constant.Session.UserName] = this.LoginUser.UserName.ToLower();                
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
