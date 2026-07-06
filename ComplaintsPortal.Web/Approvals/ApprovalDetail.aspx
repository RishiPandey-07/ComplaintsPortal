<%@ Page Title="Request Detail" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="ApprovalDetail.aspx.cs" Inherits="ComplaintsPortal.Web.Approvals.ApprovalDetail" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h4><asp:Literal ID="litRequestNumber" runat="server" /></h4>
    <p class="text-muted"><asp:Literal ID="litRequestSummary" runat="server" /></p>

    <div class="card" style="max-width:600px;">
        <div class="card-body">
            <asp:Repeater ID="rptTimeline" runat="server">
                <ItemTemplate>
                    <div class="d-flex mb-3">
                        <div class="me-2">
                            <i class='<%# GetIconClass((string)Eval("Status")) %>'></i>
                        </div>
                        <div>
                            <div style="font-weight:500;"><%# Eval("StageName") %></div>
                            <asp:Panel runat="server" Visible='<%# Eval("ActedByName") != null %>'>
                                <div class="text-muted" style="font-size:12px;">
                                    <%# Eval("Action") %> by <%# Eval("ActedByName") %>
                                    <%# Eval("ActionDate") != null ? "&middot; " + string.Format("{0:dd-MMM-yyyy hh:mm tt}", Eval("ActionDate")) : "" %>
                                </div>
                                <asp:Literal runat="server" Visible='<%# !string.IsNullOrEmpty((string)Eval("Remarks")) %>'
                                    Text='<%# "<div class=\"text-secondary fst-italic\" style=\"font-size:12px;\">\"" + Eval("Remarks") + "\"</div>" %>' />
                            </asp:Panel>
                            <asp:Literal runat="server" Visible='<%# (string)Eval("Status") == "CURRENT" %>' Text="<div class='text-primary' style='font-size:12px;'>Pending</div>" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlAction" runat="server" Visible="false">
                <hr />
                <label>Remarks</label>
                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Add a remark before forwarding or rejecting" />
                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger d-block mt-2" />
                <div class="mt-2">
                    <asp:Button ID="btnForward" runat="server" Text="Forward" CssClass="btn btn-primary" OnClick="btnForward_Click" />
                    <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="btn btn-outline-danger" OnClick="btnReject_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
