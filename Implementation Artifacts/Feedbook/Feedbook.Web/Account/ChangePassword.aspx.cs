using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Feedbook.Web.Helper;

namespace Feedbook.Web.Account
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbMessage.Visible = false;

            if (!this.IsPostBack)
                this.ValidateSession();
        }

        protected void ChangePasswordPushButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.NewPassword.Text != this.ConfirmNewPassword.Text)
                    throw new FBException("Current password did not match!");

                Model.UserCredential.ChangePassword(this.GetUserName(), this.CurrentPassword.Text, this.NewPassword.Text);
                this.lbMessage.Text = "Your password has been successfully changed!";
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
