<%@ Page Title="Standby Assignment" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="StandbyAssignment.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.StandbyAssignment" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-people-fill"></i> Standby Assignment</h4>
            <p class="page-subtitle">Temporarily assign another employee to act on your behalf during absences.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Delegate Role
        </button>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> Active Delegations</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvStandby" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="StandbyMappingId" OnRowCommand="gvStandby_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="OriginalPcno" HeaderText="Delegator PC NO" />
                                <asp:BoundField DataField="StandbyPcno" HeaderText="Standby PC NO" />
                                <asp:BoundField DataField="StandbyName" HeaderText="Standby Name" />
                                <asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MMM-yyyy}" />
                                <asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MMM-yyyy}" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CommandName="Revoke" CommandArgument='<%# Eval("StandbyMappingId") %>'
                                            CssClass="btn-portal btn-ghost btn-sm-portal text-danger-portal" Visible='<%# (string)Eval("IsActive") == "Y" %>'
                                            OnClientClick="return confirm('Are you sure you want to revoke this delegation?');">
                                            <i class="bi bi-x-circle"></i> Revoke
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center py-4 text-muted">
                                    <i class="bi bi-person-bounding-box fs-2 d-block mb-2"></i>
                                    No standby delegations active.
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upModal" runat="server">
        <ContentTemplate>
            <div class="modal fade portal-modal" id="modalStandby" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle">Delegate Role to Standby</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="alert-portal alert-warning mb-3">
                                <i class="bi bi-exclamation-triangle-fill"></i>
                                <div>
                                    <strong>Important:</strong> The assigned standby will receive <b>all</b> of your system roles and permissions until the end date.
                                </div>
                            </div>

                            <div class="form-group-portal">
                                <label class="form-label-portal">Delegator PC NO <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtOriginalPcno" runat="server" CssClass="form-control-portal" placeholder="Your PC NO" />
                                <small class="text-muted d-block mt-1">If you are an admin, you can set this for someone else.</small>
                            </div>
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Standby PC NO <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtStandbyPcno" runat="server" CssClass="form-control-portal" placeholder="Standby Person's PC NO" />
                            </div>

                            <div class="row">
                                <div class="col-md-6 form-group-portal mb-0">
                                    <label class="form-label-portal">Start Date <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control-portal" TextMode="Date" />
                                </div>
                                <div class="col-md-6 form-group-portal mb-0">
                                    <label class="form-label-portal">End Date <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control-portal" TextMode="Date" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAssign" runat="server" Text="Delegate" CssClass="btn-portal btn-primary-portal" OnClick="btnAssign_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let standbyModal = null;
        
        function getModal() {
            if (!standbyModal) standbyModal = new bootstrap.Modal(document.getElementById('modalStandby'));
            return standbyModal;
        }

        function openAddModal() {
            document.getElementById('<%= txtOriginalPcno.ClientID %>').value = "<%= CurrentPcno %>";
            document.getElementById('<%= txtStandbyPcno.ClientID %>').value = "";
            document.getElementById('<%= txtFromDate.ClientID %>').value = new Date().toISOString().split('T')[0];
            getModal().show();
        }

        function keepModalOpen() {
            getModal().show();
        }

        function closeStandbyModal() {
            if (standbyModal) standbyModal.hide();
        }
    </script>
</asp:Content>
