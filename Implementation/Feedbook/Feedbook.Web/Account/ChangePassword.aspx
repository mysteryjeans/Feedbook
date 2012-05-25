<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="Feedbook.Web.Account.ChangePassword" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script language="javascript" type="text/javascript">

        function hideServerError() {
            $get('<%= this.lbErrorMessage.ClientID %>').style.display = 'none';
        }

    </script>
    <h2>
        Change Password
    </h2>
    <p class="hint">
        New passwords are required to be a minimum of <%= Feedbook.Web.Helper.Constant.Default.PasswordLength %> characters in length.
    </p>
    <asp:Label ID="lbErrorMessage" runat="server" EnableViewState="False" CssClass="msgWarning"
        Visible="false"></asp:Label>
    <asp:Label ID="lbMessage" runat="server" EnableViewState="False"></asp:Label>
    <asp:ValidationSummary ID="ChangeUserPasswordValidationSummary" runat="server" CssClass="failureNotification"
        ValidationGroup="ChangeUserPasswordValidationGroup" />
    <fieldset class="accountInfo">
        <legend>Change Password</legend>
        <p>
            <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Current Password:</asp:Label>
            <asp:TextBox ID="CurrentPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                ErrorMessage="Password is required." ToolTip="Current Password is required."
                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
        </p>
        <p>
            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
            <asp:TextBox ID="NewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                ErrorMessage="New Password is required." ToolTip="New Password is required."
                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
        </p>
        <p>
            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
            <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                Display="Dynamic" ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required."
                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                ControlToValidate="ConfirmNewPassword" CssClass="failureNotification" Display="Dynamic"
                ErrorMessage="The Confirm New Password must match the New Password entry." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:CompareValidator>
        </p>
    </fieldset>
    <p class="submitButton">
        <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
            Text="Change Password" ValidationGroup="ChangeUserPasswordValidationGroup" Width="150px"
             OnClientClick="javascript:hideServerError();" 
            onclick="ChangePasswordPushButton_Click" />
    </p>
</asp:Content>
