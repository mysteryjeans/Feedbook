<%@ Page Title="Account Activation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Activation.aspx.cs" Inherits="Feedbook.Web.Activation" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Account Activation</h2>
    <asp:Label ID="lbMessage" runat="server" EnableViewState="false" Visible="false"
        CssClass="messageNotification" />
    <asp:Label ID="lbErrorMessage" runat="server" EnableViewState="false" Visible="false"
        CssClass="errorNotification" />
</asp:Content>
