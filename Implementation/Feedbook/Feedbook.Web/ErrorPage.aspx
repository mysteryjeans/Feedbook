<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ErrorPage.aspx.cs" Inherits="Feedbook.Web.ErrorPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Site is experiencing some technical difficulties. We apologize for any inconvenience!.
    </p>
    <asp:Label ID="lbErrorMessage" runat="server" EnableViewState="false" Visible="false" CssClass="errorNotification"/>
</asp:Content>
