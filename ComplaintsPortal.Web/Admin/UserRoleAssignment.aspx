<%@ Page Title="User Role Assignment" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="UserRoleAssignment.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.UserRoleAssignment" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-person-lines-fill"></i> User Role Assignment</h4>
            <p class="page-subtitle">Assign system roles and divisions to specific employees.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Assign Role
        </button>
    </div>

    <!-- Filter row -->
    <div class="portal-card mb-4" style="max-width: 400px;">
        <div class="portal-card-body p-3">
            <div class="form-group-portal mb-0">
                <label class="form-label-portal">Filter by PC NO</label>
                <div class="d-flex gap-2">
                    <asp:TextBox ID="txtSearchPcno" runat="server" CssClass="form-control-portal" placeholder="Enter PC NO" />
                    <asp:Button ID="btnSearch" runat="server" Text="Filter" CssClass="btn-portal btn-outline-portal" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> Active Assignments</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvAssignments" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="UserRoleId" OnRowCommand="gvAssignments_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="Pcno" HeaderText="PC NO" />
                                <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" />
                                <asp:TemplateField HeaderText="Role">
                                    <ItemTemplate>
                                        <span class="badge-status badge-active"><%# Eval("RoleName") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="DivisionName" HeaderText="Division" />
                                <asp:BoundField DataField="EffectiveFrom" HeaderText="Effective From" DataFormatString="{0:dd-MMM-yyyy}" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CommandName="End" CommandArgument='<%# Eval("UserRoleId") %>'
                                            CssClass="btn-portal btn-ghost btn-sm-portal text-danger-portal" Visible='<%# (string)Eval("IsActive") == "Y" %>'
                                            OnClientClick="return confirm('Are you sure you want to end this role assignment?');">
                                            <i class="bi bi-trash"></i> Revoke Role
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center py-4 text-muted">
                                    <i class="bi bi-inbox fs-2 d-block mb-2"></i>
                                    No role assignments found.
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
            <div class="modal fade portal-modal" id="modalAssign" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle">Assign New Role</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group-portal">
                                <label class="form-label-portal">Employee PC NO <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtPcno" runat="server" CssClass="form-control-portal" placeholder="Enter PC NO" />
                                <small class="text-muted mt-1 d-block"><i class="bi bi-info-circle"></i> Phase 2 can add a name-search picker here.</small>
                            </div>
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Role <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select" DataTextField="RoleName" DataValueField="RoleId"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged" />
                            </div>

                            <div class="form-group-portal mb-0" id="divDivision" runat="server">
                                <label class="form-label-portal">Division <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select" DataTextField="DivisionName" DataValueField="DivisionId" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAssign" runat="server" Text="Assign Role" CssClass="btn-portal btn-primary-portal" OnClick="btnAssign_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let assignModal = null;
        
        function getModal() {
            if (!assignModal) assignModal = new bootstrap.Modal(document.getElementById('modalAssign'));
            return assignModal;
        }

        function openAddModal() {
            document.getElementById('<%= txtPcno.ClientID %>').value = "";
            document.getElementById('<%= ddlRole.ClientID %>').selectedIndex = 0;
            // trigger postback to handle division visibility normally?
            // Since it's an UpdatePanel we'll just show it and let the server handle on change.
            getModal().show();
        }

        function keepModalOpen() {
            getModal().show();
        }

        function closeAssignModal() {
            if (assignModal) assignModal.hide();
        }
    </script>
</asp:Content>
