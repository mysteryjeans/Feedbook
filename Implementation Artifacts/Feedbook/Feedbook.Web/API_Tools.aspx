<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="API_Tools.aspx.cs" Inherits="Feedbook.Web.API_Tools" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Integration Code
    </h2>
    <asp:LoginView ID="RegistrationView" runat="server">
        <AnonymousTemplate>
            Click here to <asp:HyperLink ID="hlRegister" runat="server" NavigateUrl="~/Register.aspx" Text="register"/>, It will only take a second!
        </AnonymousTemplate>
    </asp:LoginView>
    <p class="hint">
        Note: All requests must be coming from the domain specified in registration, otherwise
        request will not be entertained.</p>
    <p class="teaser-content">
    </p>
</asp:Content>
