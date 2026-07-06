<%@ Page Title="Request Detail" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="ApprovalDetail.aspx.cs" Inherits="ComplaintsPortal.Web.Approvals.ApprovalDetail" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-card-checklist"></i> <asp:Literal ID="litRequestNumber" runat="server" /></h4>
            <p class="page-subtitle"><asp:Literal ID="litRequestSummary" runat="server" /></p>
        </div>
        <div>
            <asp:HyperLink runat="server" NavigateUrl="~/Approvals/PendingApprovals.aspx" CssClass="btn-portal btn-outline-portal">
                <i class="bi bi-arrow-left"></i> Back to Pending
            </asp:HyperLink>
        </div>
    </div>

    <div class="row">
        <!-- Request Info Column -->
        <div class="col-lg-7">
            <div class="portal-card mb-4">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-info-circle"></i> Request Details</h5>
                </div>
                <div class="portal-card-body">
                    <asp:Literal ID="litBasicDetails" runat="server" />
                    
                    <asp:Repeater ID="rptCustomFields" runat="server">
                        <HeaderTemplate><hr/><h6 class="fw-bold mb-3 text-secondary">Custom Form Fields</h6><table class="table table-sm borderless"></HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="text-muted" style="width: 30%;"><%# Eval("FieldLabel") %></td>
                                <td class="fw-500">
                                    <%# Eval("ValueText") %>
                                    <%# Eval("ValueNumber") %>
                                    <%# Eval("ValueDate", "{0:dd-MMM-yyyy}") %>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate></table></FooterTemplate>
                    </asp:Repeater>
                    
                    <asp:Panel ID="pnlAttachment" runat="server" Visible="false" CssClass="mt-3 p-3 bg-light rounded border">
                        <h6 class="fw-bold text-secondary mb-2"><i class="bi bi-paperclip"></i> Attached File</h6>
                        <asp:HyperLink ID="hlAttachment" runat="server" Target="_blank" CssClass="btn btn-sm btn-outline-primary">
                            <i class="bi bi-download"></i> Download Attachment
                        </asp:HyperLink>
                    </asp:Panel>
                </div>
            </div>
            
            <asp:Panel ID="pnlAction" runat="server" Visible="false" CssClass="portal-card">
                <div class="portal-card-header bg-light">
                    <h5 class="portal-card-title text-primary"><i class="bi bi-check2-square"></i> Your Action Required</h5>
                </div>
                <div class="portal-card-body">
                    <div class="form-group-portal">
                        <label class="form-label-portal">Remarks</label>
                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control-portal" TextMode="MultiLine" Rows="3" placeholder="Add a remark before forwarding or rejecting..." />
                    </div>
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger-portal fw-500 d-block mb-3" />
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnForward" runat="server" Text="Approve / Forward" CssClass="btn-portal btn-primary-portal" OnClick="btnForward_Click" />
                        <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="btn-portal btn-outline-portal text-danger" OnClick="btnReject_Click" CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>
        </div>

        <!-- Workflow Timeline Column -->
        <div class="col-lg-5">
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-signpost-split"></i> Approval Timeline</h5>
                </div>
                <div class="portal-card-body">
                    <div class="workflow-timeline">
                        <asp:Repeater ID="rptTimeline" runat="server">
                            <ItemTemplate>
                                <div class="d-flex mb-3 position-relative">
                                    <!-- Simple vertical line connecting icons if we wanted, for now just margins -->
                                    <div class="me-3 fs-4" style="margin-top: -3px;">
                                        <i class='<%# GetIconClass((string)Eval("Status")) %>'></i>
                                    </div>
                                    <div>
                                        <div class='<%# (string)Eval("Status") == "CURRENT" ? "fw-bold text-primary" : "fw-bold" %>'>
                                            <%# Eval("StageName") %>
                                        </div>
                                        
                                        <asp:Panel runat="server" Visible='<%# Eval("ActedByName") != null %>' CssClass="mt-1">
                                            <div class="text-muted" style="font-size: 13px;">
                                                <i class="bi bi-person"></i> <%# Eval("Action") %> by <%# Eval("ActedByName") %>
                                                <br/>
                                                <i class="bi bi-clock"></i> <%# Eval("ActionDate") != null ? string.Format("{0:dd-MMM-yyyy hh:mm tt}", Eval("ActionDate")) : "" %>
                                            </div>
                                            <asp:Literal runat="server" Visible='<%# !string.IsNullOrEmpty((string)Eval("Remarks")) %>'
                                                Text='<%# "<div class=\"p-2 mt-2 bg-light rounded text-secondary fst-italic\" style=\"font-size:13px; border-left: 3px solid #ccc;\">\"" + Eval("Remarks") + "\"</div>" %>' />
                                        </asp:Panel>
                                        
                                        <asp:Literal runat="server" Visible='<%# (string)Eval("Status") == "CURRENT" %>' Text="<div class='badge bg-warning text-dark mt-1'>Pending</div>" />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
