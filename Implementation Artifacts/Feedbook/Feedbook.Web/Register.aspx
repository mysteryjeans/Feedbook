<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="Feedbook.Web.Register" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            checkChanged();
        });

        function checkChanged() {
            document.getElementById('<%= this.RegisterUserButton.ClientID %>').disabled = !(document.getElementById('termAgreeCheckBox').checked && document.getElementById('allowEmailCheckbox').checked);
        }

    </script>
    <div>
        <h2>
            Register
        </h2>
        <p class="hint">
            Passwords are required to be a minimum of
            <%= Feedbook.Web.Helper.Constant.Default.PasswordLength %>
            characters in length.
        </p>
        <asp:Label ID="lbErrorMessage" runat="server" EnableViewState="false" Visible="false" CssClass="errorNotification" />
        <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification"
            ValidationGroup="RegisterUserValidationGroup" />
        <div>
            <fieldset class="accountInfo">
                <legend>Account Information</legend>
                <p>
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                    <asp:TextBox ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                        CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User name is required."
                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="NameLanel" runat="server" AssociatedControlID="Name">Name:</asp:Label>
                    <asp:TextBox ID="Name" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Name"
                        CssClass="failureNotification" ErrorMessage="Your name is required." ToolTip="User name is required."
                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                    <asp:TextBox ID="Email" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                        CssClass="failureNotification" ErrorMessage="E-mail is required." ToolTip="E-mail is required."
                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator runat="server" ErrorMessage="Please enter valid email address."
                        ID="RegularExpressionValidator1" ControlToValidate="Email" CssClass="failureNotification"
                        Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ValidationGroup="RegisterUserValidationGroup"></asp:RegularExpressionValidator>
                    <br />
                </p>
                <p>
                    <asp:Label ID="DomainLable" runat="server" AssociatedControlID="Name">Domain:</asp:Label>
                    <asp:TextBox ID="Domain" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Domain"
                        CssClass="failureNotification" ErrorMessage="Please enter domain name of your website, requests coming from only this domain will be served."
                        ToolTip="Domain name of your website." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                    <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                        CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required."
                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Confirm Password:</asp:Label>
                    <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" CssClass="failureNotification"
                        Display="Dynamic" ErrorMessage="Confirm Password is required." ID="ConfirmPasswordRequired"
                        runat="server" ToolTip="Confirm Password is required." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
                        ControlToValidate="ConfirmPassword" CssClass="failureNotification" Display="Dynamic"
                        ErrorMessage="The Password and Confirmation Password must match." ValidationGroup="RegisterUserValidationGroup">*</asp:CompareValidator>
                </p>
            </fieldset>
            <div>
                <div>
                    <input id="termAgreeCheckBox" type="checkbox" onclick="javascript:checkChanged();" />
                    <label for="termAgreeCheckBox">
                        I have read and agree with terms &amp; conditions.</label>
                </div>
                <div style="margin-top: 5px">
                    <input id="allowEmailCheckbox" type="checkbox" onclick="javascript:checkChanged();" />
                    <label for="allowEmailCheckbox">
                        You can contact me for important news or announcements.</label>
                </div>
            </div>
            <p class="submitButton" style="width: 375px; margin-top: 5px">
                <asp:Button ID="RegisterUserButton" runat="server" Text="Register" ValidationGroup="RegisterUserValidationGroup"
                    OnClick="RegisterUserButton_Click" />
            </p>
        </div>
    </div>
</asp:Content>
