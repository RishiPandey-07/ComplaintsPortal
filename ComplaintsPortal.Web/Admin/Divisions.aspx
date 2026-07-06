<%@ Page Title="Divisions" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="Divisions.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.Divisions" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-diagram-3"></i> Division Management</h4>
            <p class="page-subtitle">Manage organization divisions and their active status.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Add Division
        </button>
    </div>

    <!-- UpdatePanel for the grid -->
    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> All Divisions</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvDivisions" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="DivisionId" OnRowCommand="gvDivisions_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="DivisionCode" HeaderText="Code" />
                                <asp:BoundField DataField="DivisionName" HeaderText="Name" />
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
                                            <asp:LinkButton runat="server" CommandName="EditDiv" CommandArgument='<%# Eval("DivisionId") %>' 
                                                CssClass="btn-portal btn-outline-portal btn-sm-portal">
                                                <i class="bi bi-pencil"></i> Edit
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("DivisionId") + "|" + Eval("IsActive") %>'
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

    <!-- Modal for Add/Edit (Outside UpdatePanel to let it be updated from inside) -->
    <asp:UpdatePanel ID="upModal" runat="server">
        <ContentTemplate>
            <div class="modal fade portal-modal" id="modalDivision" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle" runat="server">Add Division</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfDivisionId" runat="server" Value="0" />
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Division Code <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control-portal" MaxLength="20" placeholder="e.g. AD, ESTT, IT" />
                            </div>
                            
                            <div class="form-group-portal mb-0">
                                <label class="form-label-portal">Division Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control-portal" MaxLength="150" placeholder="Full name of the division" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnSave" runat="server" Text="Save Division" CssClass="btn-portal btn-primary-portal" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let divModal = null;
        
        function getModal() {
            if (!divModal) divModal = new bootstrap.Modal(document.getElementById('modalDivision'));
            return divModal;
        }

        function openAddModal() {
            document.getElementById('<%= hfDivisionId.ClientID %>').value = "0";
            document.getElementById('<%= txtCode.ClientID %>').value = "";
            document.getElementById('<%= txtName.ClientID %>').value = "";
            document.getElementById('<%= modalTitle.ClientID %>').innerText = "Add Division";
            getModal().show();
        }

        function openEditModal() {
            getModal().show();
        }

        function closeDivisionModal() {
            if (divModal) divModal.hide();
        }
    </script>
</asp:Content>
