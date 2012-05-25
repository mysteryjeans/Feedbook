<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs" Inherits="Feedbook.Web.Account.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h2>
            Profile
        </h2>
        <asp:Label ID="lbMessage" runat="server" EnableViewState="false" Visible="false"
            CssClass="messageNotification" />
        <asp:Label ID="lbErrorMessage" runat="server" EnableViewState="false" Visible="false"
            CssClass="errorNotification" />
        <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification"
            ValidationGroup="RegisterUserValidationGroup" />
        <div>
            <fieldset class="accountInfo">
                <legend>Account Information</legend>
                <p>
                    <asp:Label ID="NameLanel" runat="server" AssociatedControlID="Name">Name:</asp:Label>
                    <asp:TextBox ID="Name" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Name"
                        CssClass="failureNotification" ErrorMessage="Your name is required." ToolTip="User name is required."
                        ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail: </asp:Label>
                    <span class="hint">Changing email address requires verification.</span>
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
            </fieldset>
            <p class="submitButton" style="width: 375px; margin-top: 5px">
                <asp:Button ID="SaveUserButton" runat="server" Text="Save" 
                    ValidationGroup="RegisterUserValidationGroup" onclick="SaveUserButton_Click"
                     />
            </p>
        </div>
    </div>
</asp:Content>
