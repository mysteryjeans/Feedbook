using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Feedbook.Web.Helper;

namespace Feedbook.Web
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbErrorMessage.Visible = false;
            if (!this.IsPostBack && this.User.Identity.IsAuthenticated)
                this.Response.Redirect("~/Login.aspx", false);
        }

        protected void RegisterUserButton_Click(object sender, EventArgs e)
        {
            try
            {
                Model.User.Register(this.UserName.Text, this.Password.Text, this.Name.Text, this.Email.Text, this.Domain.Text);
                this.Response.Redirect("~/Login.aspx?message=" + HttpUtility.UrlEncode(Constant.Message.RegistrationSucessful), false);
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
