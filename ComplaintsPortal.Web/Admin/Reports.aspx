<%@ Page Title="Reports" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.Reports" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-file-earmark-spreadsheet"></i> Reports</h4>
            <p class="page-subtitle">Generate and export request reports.</p>
        </div>
    </div>

    <div class="portal-card mb-4">
        <div class="portal-card-header">
            <h5 class="portal-card-title">Filter Criteria</h5>
        </div>
        <div class="portal-card-body">
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label-portal">From Date</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control-portal" TextMode="Date" />
                </div>
                <div class="col-md-3">
                    <label class="form-label-portal">To Date</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control-portal" TextMode="Date" />
                </div>
                <div class="col-md-3">
                    <label class="form-label-portal">Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                        <asp:ListItem Text="All Statuses" Value="" />
                        <asp:ListItem Text="Submitted" Value="SUBMITTED" />
                        <asp:ListItem Text="In Progress" Value="IN_PROGRESS" />
                        <asp:ListItem Text="Completed" Value="COMPLETED" />
                        <asp:ListItem Text="Closed" Value="CLOSED" />
                        <asp:ListItem Text="Rejected" Value="REJECTED" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-portal btn-primary-portal w-100" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="portal-card">
        <div class="portal-card-header d-flex justify-content-between align-items-center">
            <h5 class="portal-card-title mb-0">Results</h5>
            <asp:Button ID="btnExport" runat="server" Text="Export to CSV" CssClass="btn-portal btn-success" OnClick="btnExport_Click" />
        </div>
        <div class="portal-card-body p-0">
            <div class="portal-table-wrap border-0 shadow-none">
                <asp:GridView ID="gvReports" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="RequestNumber" HeaderText="Request No." />
                        <asp:BoundField DataField="RequestTypeName" HeaderText="Type" />
                        <asp:BoundField DataField="DivisionName" HeaderText="Division" />
                        <asp:BoundField DataField="RequesterName" HeaderText="Requester" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
                        <asp:BoundField DataField="ClosedDate" HeaderText="Closed" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
                        <asp:TemplateField HeaderText="SLA">
                            <ItemTemplate>
                                <span class='<%# IsSlaBreached(Eval("SlaDueDate"), Eval("Status")) ? "badge bg-danger" : "" %>'>
                                    <%# IsSlaBreached(Eval("SlaDueDate"), Eval("Status")) ? "Breached" : "" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="p-3 text-center text-muted">No requests found matching the criteria.</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
