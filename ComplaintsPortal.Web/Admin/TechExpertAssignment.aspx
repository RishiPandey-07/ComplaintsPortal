<%@ Page Title="Tech Expert Assignment" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="TechExpertAssignment.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.TechExpertAssignment" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-tools"></i> Technical Expert Assignment</h4>
            <p class="page-subtitle">Assign IT experts to handle specific service request pools.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Assign Expert
        </button>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> Active Experts</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvExperts" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="MappingId" OnRowCommand="gvExperts_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="Pcno" HeaderText="PC NO" />
                                <asp:BoundField DataField="ExpertName" HeaderText="Expert Name" />
                                <asp:BoundField DataField="ServiceName" HeaderText="Service Pool" />
                                <asp:BoundField DataField="RequestTypeName" HeaderText="Specific Request Type" NullDisplayText="All Types" />
                                <asp:BoundField DataField="AssignedDate" HeaderText="Assigned On" DataFormatString="{0:dd-MMM-yyyy}" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CommandName="Deactivate" CommandArgument='<%# Eval("MappingId") %>'
                                            CssClass="btn-portal btn-ghost btn-sm-portal text-danger-portal" Visible='<%# (string)Eval("IsActive") == "Y" %>'
                                            OnClientClick="return confirm('Are you sure you want to deactivate this expert from this pool?');">
                                            <i class="bi bi-trash"></i> Remove
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center py-4 text-muted">
                                    <i class="bi bi-person-x fs-2 d-block mb-2"></i>
                                    No technical experts assigned.
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
            <div class="modal fade portal-modal" id="modalExpert" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle">Assign Tech Expert</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group-portal">
                                <label class="form-label-portal">Expert PC NO <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtPcno" runat="server" CssClass="form-control-portal" placeholder="Enter PC NO" />
                            </div>
                            
                            <div class="form-group-portal">
                                <label class="form-label-portal">Service Category <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlService" runat="server" CssClass="form-select" DataTextField="ServiceName" DataValueField="ServiceId"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlService_SelectedIndexChanged" />
                            </div>

                            <div class="form-group-portal mb-0">
                                <label class="form-label-portal">Specific Request Type (Optional)</label>
                                <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-select" DataTextField="TypeName" DataValueField="RequestTypeId" />
                                <small class="text-muted mt-1 d-block">Leave blank to grant access to ALL requests under this Service.</small>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAssign" runat="server" Text="Assign Expert" CssClass="btn-portal btn-primary-portal" OnClick="btnAssign_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let expertModal = null;
        
        function getModal() {
            if (!expertModal) expertModal = new bootstrap.Modal(document.getElementById('modalExpert'));
            return expertModal;
        }

        function openAddModal() {
            document.getElementById('<%= txtPcno.ClientID %>').value = "";
            getModal().show();
        }

        function keepModalOpen() {
            getModal().show();
        }

        function closeExpertModal() {
            if (expertModal) expertModal.hide();
        }
    </script>
</asp:Content>
