<%@ Page Title="Audit Log" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="AuditLog.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.AuditLog" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h4>Audit Log</h4>

    <div class="d-flex gap-2 mb-3">
        <asp:TextBox ID="txtPcno" runat="server" CssClass="form-control" placeholder="PCNO" style="max-width:150px;" />
        <asp:TextBox ID="txtModule" runat="server" CssClass="form-control" placeholder="Module (e.g. REQUEST)" style="max-width:180px;" />
        <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" placeholder="From (dd-MMM-yyyy)" style="max-width:170px;" />
        <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" placeholder="To (dd-MMM-yyyy)" style="max-width:170px;" />
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-outline-secondary" OnClick="btnSearch_Click" />
    </div>

    <asp:GridView ID="gvAudit" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="ActionDate" HeaderText="Date/Time" DataFormatString="{0:dd-MMM-yyyy hh:mm:ss tt}" />
            <asp:BoundField DataField="Pcno" HeaderText="PCNO" />
            <asp:BoundField DataField="ModuleName" HeaderText="Module" />
            <asp:BoundField DataField="ActionType" HeaderText="Action" />
            <asp:BoundField DataField="EntityId" HeaderText="Entity" />
            <asp:BoundField DataField="ActionDesc" HeaderText="Description" />
            <asp:BoundField DataField="IpAddress" HeaderText="IP Address" />
        </Columns>
        <EmptyDataTemplate>
            No matching audit entries.
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
