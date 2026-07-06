<%@ Page Title="My Requests" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="MyRequests.aspx.cs" Inherits="ComplaintsPortal.Web.Employee.MyRequests" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h4>My Requests</h4>

    <asp:GridView ID="gvMyRequests" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="RequestNumber" HeaderText="Request No." />
            <asp:BoundField DataField="RequestTypeName" HeaderText="Type" />
            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <span class='<%# GetStatusBadgeClass((string)Eval("Status")) %>'><%# Eval("Status") %></span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="PickedByName" HeaderText="Handled By" />
            <asp:BoundField DataField="ResolutionRemarks" HeaderText="Resolution Notes" />
            <asp:TemplateField HeaderText="Details">
                <ItemTemplate>
                    <a class="btn btn-sm btn-outline-secondary" href='<%# "../Approvals/ApprovalDetail.aspx?id=" + Eval("RequestId") %>'>View</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            You haven't submitted any requests yet.
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
