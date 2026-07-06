<%@ Page Title="Pending Approvals" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="PendingApprovals.aspx.cs" Inherits="ComplaintsPortal.Web.Approvals.PendingApprovals" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h4>Pending Approvals</h4>
    <p class="text-muted">Includes anything pending with you directly, and anything pending as your standby duty (marked below).</p>

    <asp:GridView ID="gvPending" runat="server" CssClass="table table-striped" AutoGenerateColumns="false" DataKeyNames="RequestId">
        <Columns>
            <asp:BoundField DataField="RequestNumber" HeaderText="Request No." />
            <asp:BoundField DataField="RequestTypeName" HeaderText="Type" />
            <asp:BoundField DataField="DivisionName" HeaderText="Division" />
            <asp:BoundField DataField="RequesterName" HeaderText="Raised By" />
            <asp:BoundField DataField="CurrentStageName" HeaderText="Stage" />
            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
            <asp:TemplateField HeaderText="Status / SLA">
                <ItemTemplate>
                    <span class='<%# (bool)Eval("IsStandbyItem") ? "badge bg-warning text-dark me-1" : "" %>'>
                        <%# (bool)Eval("IsStandbyItem") ? "Standby" : "" %>
                    </span>
                    <span class='<%# IsSlaBreached(Eval("SlaDueDate")) ? "badge bg-danger" : "" %>'>
                        <%# IsSlaBreached(Eval("SlaDueDate")) ? "SLA Breached" : "" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <a class="btn btn-sm btn-primary" href='<%# "ApprovalDetail.aspx?id=" + Eval("RequestId") + "&standbyFor=" + Eval("StandbyForUserRoleId") %>'>Review</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            Nothing pending with you right now.
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
