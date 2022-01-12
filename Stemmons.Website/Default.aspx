<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Stemmons.Website._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <% if (User.Identity.IsAuthenticated)
        {%>
    <div class="jumbotron">

            <asp:Repeater runat="server" ID="tenantsRepeater">
                <HeaderTemplate><ul></HeaderTemplate>
                <ItemTemplate>
                    <li><asp:LinkButton OnClick="SelectTenant_Click" CommandArgument='<%# Eval("TenantID") %>' runat="server"><%# Eval("TenantName") %></asp:LinkButton></li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
                
            </asp:Repeater>
    </div>
    <%}
        else
        { %>
    <div>Please login to access your data</div>
    <%} %>
  
</asp:Content>
