using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Feedbook.Web.Helper;
using System.Web.Security;

namespace Feedbook.Web.Account
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbMessage.Visible = this.lbErrorMessage.Visible = false;
            if (!this.IsPostBack && this.ValidateSession())
                this.LoadProfile();
        }

        protected void SaveUserButton_Click(object sender, EventArgs e)
        {
            try
            {
                var user = Model.User.Update(this.GetUserName(), this.Name.Text, this.Email.Text, this.Domain.Text, this.GetUserName());
                if (user.IsVerified)
                {
                    this.LoadUserProfile(user);
                    this.lbMessage.Text = "Profile has been updated!";
                    this.lbMessage.Visible = true;
                }
                else
                    FormsAuthentication.RedirectToLoginPage("?message=" + "You will shortly an activation email to verify your new email!");
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

        private void LoadProfile()
        {
            try
            {
                var userName = this.GetUserName();
                var user = Model.User.Query().First(u => u.UserName == userName);
                this.LoadUserProfile(user);
            }
            catch (Exception ex)
            {
                this.LogAndRedirectToErrorPage(ex);
            }
        }

        private void LoadUserProfile(Model.User user)
        {
            this.Name.Text = user.Name;
            this.Email.Text = user.Email;
            this.Domain.Text = user.Domain;
        }
    }
}