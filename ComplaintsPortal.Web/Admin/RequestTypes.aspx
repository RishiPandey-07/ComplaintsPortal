<%@ Page Title="Request Types" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="RequestTypes.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.RequestTypes" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-tags"></i> Request Type Management</h4>
            <p class="page-subtitle">Manage high-level request types mapped to services.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Add Request Type
        </button>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> All Request Types</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvRequestTypes" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="RequestTypeId" OnRowCommand="gvRequestTypes_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="ServiceName" HeaderText="Service" />
                                <asp:BoundField DataField="TypeCode" HeaderText="Code" />
                                <asp:BoundField DataField="TypeName" HeaderText="Name" />
                                <asp:BoundField DataField="IsFlowBased" HeaderText="Flow Based?" />
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
                                            <asp:LinkButton runat="server" CommandName="EditType" CommandArgument='<%# Eval("RequestTypeId") %>' 
                                                CssClass="btn-portal btn-outline-portal btn-sm-portal">
                                                <i class="bi bi-pencil"></i> Edit
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("RequestTypeId") + "|" + Eval("IsActive") %>'
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
            <div class="modal fade portal-modal" id="modalRequestType" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle" runat="server">Add Request Type</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfRequestTypeId" runat="server" Value="0" />
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Service <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlService" runat="server" CssClass="form-select" DataTextField="ServiceName" DataValueField="ServiceId" />
                            </div>

                            <div class="form-group-portal">
                                <label class="form-label-portal">Type Code <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control-portal" MaxLength="20" placeholder="e.g. NW_ISSUE" />
                            </div>
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Type Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control-portal" MaxLength="150" placeholder="e.g. Network Issue" />
                            </div>

                            <div class="form-group-portal mb-0">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkFlowBased" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label">Requires Approval Workflow</label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnSave" runat="server" Text="Save Request Type" CssClass="btn-portal btn-primary-portal" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let rtModal = null;
        
        function getModal() {
            if (!rtModal) rtModal = new bootstrap.Modal(document.getElementById('modalRequestType'));
            return rtModal;
        }

        function openAddModal() {
            document.getElementById('<%= hfRequestTypeId.ClientID %>').value = "0";
            document.getElementById('<%= txtCode.ClientID %>').value = "";
            document.getElementById('<%= txtName.ClientID %>').value = "";
            document.getElementById('<%= chkFlowBased.ClientID %>').checked = false;
            document.getElementById('<%= modalTitle.ClientID %>').innerText = "Add Request Type";
            getModal().show();
        }

        function openEditModal() {
            getModal().show();
        }

        function closeRequestTypeModal() {
            if (rtModal) rtModal.hide();
        }
    </script>
</asp:Content>
