<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ComplaintsPortal.Web.Common.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login - Complaints Portal</title>
    <link href="https://rishi-pandey.lrde.com/passout/Static/bootstrap5.3.2/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://rishi-pandey.lrde.com/passout/Static/bootstrap-icons-1.13.1/bootstrap-icons.min.css" rel="stylesheet" />
    <link href="~/Content/portal.css" rel="stylesheet" runat="server" />
</head>
<body class="login-page">
    <!-- Animated background shapes -->
    <div class="login-bg-shapes">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
        <div class="shape shape-3"></div>
    </div>

    <div class="login-panel">
        <div class="login-card">
            <div class="login-logo-wrap">
                <div class="login-logo-circle">
                    <i class="bi bi-shield-check"></i>
                </div>
                <div class="login-org-name"><%= System.Configuration.ConfigurationManager.AppSettings["OrgName"] ?? "Organization" %></div>
                <div class="login-tagline"><%= System.Configuration.ConfigurationManager.AppSettings["AppTagline"] ?? "IT Services Portal" %></div>
            </div>

            <h5 class="login-heading text-center">Sign in to your account</h5>

            <form id="form1" runat="server">
                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="login-error">
                    <i class="bi bi-exclamation-octagon-fill"></i>
                    <asp:Label ID="lblMessage" runat="server" />
                </asp:Panel>

                <div class="login-input-wrap">
                    <i class="bi bi-person login-input-icon"></i>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="login-input" placeholder="Username (PC NO)" autocomplete="off" />
                </div>

                <div class="login-input-wrap mb-4">
                    <i class="bi bi-key login-input-icon"></i>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="login-input" TextMode="Password" placeholder="Password" />
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn-login" OnClick="btnLogin_Click" />
                
                <div class="login-footer-text">
                    Secure AD/LDAP Authentication &middot; Authorized Access Only
                </div>
            </form>
        </div>
    </div>
</body>
</html>
