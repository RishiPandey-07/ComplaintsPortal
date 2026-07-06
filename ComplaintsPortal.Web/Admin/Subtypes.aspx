<%@ Page Title="Sub-Types" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="Subtypes.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.Subtypes" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-tag"></i> Sub-Type Management</h4>
            <p class="page-subtitle">Manage granular sub-types mapped to request types.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Add Sub-Type
        </button>
    </div>

    <!-- Filter row -->
    <div class="portal-card mb-4" style="max-width: 500px;">
        <div class="portal-card-body p-3">
            <div class="form-group-portal mb-0">
                <label class="form-label-portal">Filter by Request Type</label>
                <div class="d-flex gap-2">
                    <asp:DropDownList ID="ddlFilterRequestType" runat="server" CssClass="form-select" DataTextField="TypeName" DataValueField="RequestTypeId" />
                    <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn-portal btn-outline-portal" OnClick="btnFilter_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> All Sub-Types</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvSubtypes" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="SubTypeId" OnRowCommand="gvSubtypes_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RequestTypeName" HeaderText="Request Type" />
                                <asp:BoundField DataField="SubTypeCode" HeaderText="Code" />
                                <asp:BoundField DataField="SubTypeName" HeaderText="Name" />
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
                                            <asp:LinkButton runat="server" CommandName="EditType" CommandArgument='<%# Eval("SubTypeId") %>' 
                                                CssClass="btn-portal btn-outline-portal btn-sm-portal">
                                                <i class="bi bi-pencil"></i> Edit
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("SubTypeId") + "|" + Eval("IsActive") %>'
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
            <div class="modal fade portal-modal" id="modalSubtype" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle" runat="server">Add Sub-Type</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfSubtypeId" runat="server" Value="0" />
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Request Type <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-select" DataTextField="TypeName" DataValueField="RequestTypeId" />
                            </div>

                            <div class="form-group-portal">
                                <label class="form-label-portal">Sub-Type Code <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control-portal" MaxLength="20" placeholder="e.g. SLOW_NW" />
                            </div>
                            
                            <div class="form-group-portal mb-0">
                                <label class="form-label-portal">Sub-Type Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control-portal" MaxLength="150" placeholder="e.g. Slow Network Speed" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnSave" runat="server" Text="Save Sub-Type" CssClass="btn-portal btn-primary-portal" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let stModal = null;
        
        function getModal() {
            if (!stModal) stModal = new bootstrap.Modal(document.getElementById('modalSubtype'));
            return stModal;
        }

        function openAddModal() {
            document.getElementById('<%= hfSubtypeId.ClientID %>').value = "0";
            document.getElementById('<%= txtCode.ClientID %>').value = "";
            document.getElementById('<%= txtName.ClientID %>').value = "";
            document.getElementById('<%= modalTitle.ClientID %>').innerText = "Add Sub-Type";
            getModal().show();
        }

        function openEditModal() {
            getModal().show();
        }

        function closeSubtypeModal() {
            if (stModal) stModal.hide();
        }
    </script>
</asp:Content>
