<%@ Page Title="Dynamic Field Builder" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="FieldBuilder.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.FieldBuilder" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-ui-radios"></i> Dynamic Field Builder</h4>
            <p class="page-subtitle">Configure custom input fields for specific request types and subtypes.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Add Field
        </button>
    </div>

    <!-- Filter row -->
    <div class="portal-card mb-4" style="max-width: 600px;">
        <div class="portal-card-body p-3">
            <div class="row g-2">
                <div class="col-md-6 form-group-portal mb-0">
                    <label class="form-label-portal">Filter by Request Type</label>
                    <asp:DropDownList ID="ddlFilterRequestType" runat="server" CssClass="form-select" DataTextField="TypeName" DataValueField="RequestTypeId"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlFilterRequestType_SelectedIndexChanged" />
                </div>
                <div class="col-md-6 form-group-portal mb-0">
                    <label class="form-label-portal">Filter by Sub-Type</label>
                    <div class="d-flex gap-2">
                        <asp:DropDownList ID="ddlFilterSubType" runat="server" CssClass="form-select" DataTextField="SubTypeName" DataValueField="SubTypeId" />
                        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn-portal btn-outline-portal" OnClick="btnFilter_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> Configured Fields</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvFields" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="FieldId" OnRowCommand="gvFields_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="DisplayOrder" HeaderText="Order" />
                                <asp:BoundField DataField="FieldCode" HeaderText="Code" />
                                <asp:BoundField DataField="FieldLabel" HeaderText="Label" />
                                <asp:BoundField DataField="FieldType" HeaderText="Type" />
                                <asp:TemplateField HeaderText="Mandatory?">
                                    <ItemTemplate>
                                        <span class='<%# (string)Eval("IsMandatory") == "Y" ? "badge-status badge-active" : "badge-status badge-inactive" %>'>
                                            <%# (string)Eval("IsMandatory") == "Y" ? "Yes" : "No" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
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
                                            <asp:LinkButton runat="server" CommandName="EditField" CommandArgument='<%# Eval("FieldId") %>' 
                                                CssClass="btn-portal btn-outline-portal btn-sm-portal">
                                                <i class="bi bi-pencil"></i> Edit
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("FieldId") + "|" + Eval("IsActive") %>'
                                                CssClass='<%# (string)Eval("IsActive") == "Y" ? "btn-portal btn-ghost btn-sm-portal text-danger-portal" : "btn-portal btn-ghost btn-sm-portal text-success-portal" %>'>
                                                <i class='<%# (string)Eval("IsActive") == "Y" ? "bi bi-x-circle" : "bi bi-check-circle" %>'></i> 
                                                <%# (string)Eval("IsActive") == "Y" ? "Deactivate" : "Activate" %>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center py-4 text-muted">
                                    <i class="bi bi-inbox fs-2 d-block mb-2"></i>
                                    No custom fields found for this selection.
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
            <div class="modal fade portal-modal" id="modalField" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle" runat="server">Add Custom Field</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfFieldId" runat="server" Value="0" />
                            
                            <div class="row">
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Request Type <span class="text-danger">*</span></label>
                                    <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-select" DataTextField="TypeName" DataValueField="RequestTypeId"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlRequestType_SelectedIndexChanged" />
                                </div>
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Sub-Type</label>
                                    <asp:DropDownList ID="ddlSubType" runat="server" CssClass="form-select" DataTextField="SubTypeName" DataValueField="SubTypeId" />
                                    <small class="text-muted d-block mt-1">Leave blank to apply to all subtypes.</small>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Field Code <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtCode" runat="server" CssClass="form-control-portal" MaxLength="50" placeholder="e.g. NODE_IP" />
                                </div>
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Field Label <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtLabel" runat="server" CssClass="form-control-portal" MaxLength="150" placeholder="e.g. Node IP Address" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Input Type <span class="text-danger">*</span></label>
                                    <asp:DropDownList ID="ddlFieldType" runat="server" CssClass="form-select" onchange="toggleDropdownOptions()">
                                        <asp:ListItem Text="Text (Single Line)" Value="TEXT" />
                                        <asp:ListItem Text="Text (Multi-Line)" Value="MULTILINE" />
                                        <asp:ListItem Text="Number" Value="NUMBER" />
                                        <asp:ListItem Text="Date" Value="DATE" />
                                        <asp:ListItem Text="Dropdown Selection" Value="DROPDOWN" />
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Display Order <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control-portal" TextMode="Number" Text="10" />
                                </div>
                            </div>

                            <div class="form-group-portal" id="divDropdownOptions" style="display:none;">
                                <label class="form-label-portal">Dropdown Options</label>
                                <asp:TextBox ID="txtDropdownOptions" runat="server" CssClass="form-control-portal" placeholder="Comma separated, e.g. Option A, Option B" />
                            </div>

                            <div class="form-group-portal mb-0">
                                <div class="form-check">
                                    <asp:CheckBox ID="chkMandatory" runat="server" CssClass="form-check-input" />
                                    <label class="form-check-label">Is Mandatory?</label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnSave" runat="server" Text="Save Field" CssClass="btn-portal btn-primary-portal" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let fieldModal = null;
        
        function getModal() {
            if (!fieldModal) fieldModal = new bootstrap.Modal(document.getElementById('modalField'));
            return fieldModal;
        }

        function openAddModal() {
            document.getElementById('<%= hfFieldId.ClientID %>').value = "0";
            document.getElementById('<%= txtCode.ClientID %>').value = "";
            document.getElementById('<%= txtLabel.ClientID %>').value = "";
            document.getElementById('<%= txtDropdownOptions.ClientID %>').value = "";
            document.getElementById('<%= chkMandatory.ClientID %>').checked = false;
            document.getElementById('<%= modalTitle.ClientID %>').innerText = "Add Custom Field";
            toggleDropdownOptions();
            getModal().show();
        }

        function openEditModal() {
            toggleDropdownOptions();
            getModal().show();
        }

        function closeFieldModal() {
            if (fieldModal) fieldModal.hide();
        }

        function toggleDropdownOptions() {
            var ddl = document.getElementById('<%= ddlFieldType.ClientID %>');
            var div = document.getElementById('divDropdownOptions');
            if(ddl && div) {
                div.style.display = ddl.value === 'DROPDOWN' ? 'block' : 'none';
            }
        }
    </script>
</asp:Content>
