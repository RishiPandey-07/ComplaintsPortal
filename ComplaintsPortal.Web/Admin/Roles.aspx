<%@ Page Title="Roles" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.Roles" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-person-badge"></i> Role Management</h4>
            <p class="page-subtitle">Manage system roles and their permissions.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Add Role
        </button>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> All Roles</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvRoles" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="RoleId" OnRowCommand="gvRoles_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RoleName" HeaderText="Name" />
                                <asp:BoundField DataField="RoleCode" HeaderText="Code" />
                                <asp:BoundField DataField="RoleCategory" HeaderText="Category" />
                                <asp:BoundField DataField="RequiresDivision" HeaderText="Req. Div?" />
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <span class='<%# (string)Eval("IsActive") == "Y" ? "badge-status badge-active" : "badge-status badge-inactive" %>'>
                                            <%# (string)Eval("IsActive") == "Y" ? "Active" : "Inactive" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <div class="d-flex gap-2">
                                            <asp:LinkButton runat="server" CommandName="EditRole" CommandArgument='<%# Eval("RoleId") %>' 
                                                CssClass="btn-portal btn-outline-portal btn-sm-portal">
                                                <i class="bi bi-pencil"></i> Edit
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("RoleId") + "|" + Eval("IsActive") %>'
                                                CssClass='<%# (string)Eval("IsActive") == "Y" ? "btn-portal btn-ghost btn-sm-portal text-danger-portal" : "btn-portal btn-ghost btn-sm-portal text-success-portal" %>'>
                                                <i class='<%# (string)Eval("IsActive") == "Y" ? "bi bi-x-circle" : "bi bi-check-circle" %>'></i> 
                                                <%# (string)Eval("IsActive") == "Y" ? "Deactivate" : "Activate" %>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upModal" runat="server">
        <ContentTemplate>
            <div class="modal fade portal-modal" id="modalRole" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle" runat="server">Add Role</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfRoleId" runat="server" Value="0" />
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Role Code <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control-portal" MaxLength="50" placeholder="e.g. OIC_IT" />
                            </div>
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Role Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control-portal" MaxLength="150" placeholder="e.g. Officer In-Charge IT" />
                            </div>

                            <div class="form-group-portal">
                                <label class="form-label-portal">Category <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="EMPLOYEE" Value="EMPLOYEE" />
                                    <asp:ListItem Text="DIVISION" Value="DIVISION" />
                                    <asp:ListItem Text="AD" Value="AD" />
                                    <asp:ListItem Text="IT" Value="IT" />
                                    <asp:ListItem Text="ESTABLISHMENT" Value="ESTABLISHMENT" />
                                </asp:DropDownList>
                            </div>

                            <div class="form-group-portal mb-0">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkReqDiv" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label">Requires Division Assignment</label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnSave" runat="server" Text="Save Role" CssClass="btn-portal btn-primary-portal" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let roleModal = null;
        
        function getModal() {
            if (!roleModal) roleModal = new bootstrap.Modal(document.getElementById('modalRole'));
            return roleModal;
        }

        function openAddModal() {
            document.getElementById('<%= hfRoleId.ClientID %>').value = "0";
            document.getElementById('<%= txtCode.ClientID %>').value = "";
            document.getElementById('<%= txtName.ClientID %>').value = "";
            document.getElementById('<%= ddlCategory.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= chkReqDiv.ClientID %>').checked = false;
            document.getElementById('<%= modalTitle.ClientID %>').innerText = "Add Role";
            getModal().show();
        }

        function openEditModal() {
            getModal().show();
        }

        function closeRoleModal() {
            if (roleModal) roleModal.hide();
        }
    </script>
</asp:Content>
