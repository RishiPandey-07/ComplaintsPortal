<%@ Page Title="Connection Register" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="ConnectionRegister.aspx.cs" Inherits="ComplaintsPortal.Web.TechExpert.ConnectionRegister" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-hdd-network-fill"></i> Connection Register</h4>
            <p class="page-subtitle">Track and manage active IT connections for employees.</p>
        </div>
        <button type="button" class="btn-portal btn-primary-portal" onclick="openAddModal()">
            <i class="bi bi-plus-lg"></i> Add Connection
        </button>
    </div>

    <div class="portal-card mb-4" style="max-width: 400px;">
        <div class="portal-card-body p-3">
            <div class="form-group-portal mb-0">
                <label class="form-label-portal">Search by PC NO</label>
                <div class="d-flex gap-2">
                    <asp:TextBox ID="txtSearchPcno" runat="server" CssClass="form-control-portal" placeholder="Enter PC NO" />
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-portal btn-outline-portal" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="upGrid" runat="server">
        <ContentTemplate>
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-list-ul"></i> Connection Records</h5>
                </div>
                <div class="portal-card-body p-0">
                    <div class="portal-table-wrap border-0 shadow-none">
                        <asp:GridView ID="gvConnections" runat="server" CssClass="table portal-table mb-0" AutoGenerateColumns="false"
                            DataKeyNames="ConnId" OnRowCommand="gvConnections_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="Pcno" HeaderText="PC NO" />
                                <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" />
                                <asp:BoundField DataField="ConnectionType" HeaderText="Type" />
                                <asp:BoundField DataField="IpAddress" HeaderText="IP Address" />
                                <asp:BoundField DataField="MacAddress" HeaderText="MAC Address" />
                                <asp:BoundField DataField="PortNo" HeaderText="Port" />
                                <asp:BoundField DataField="SwitchName" HeaderText="Switch" />
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <span class='<%# (string)Eval("Status") == "ACTIVE" ? "badge-status badge-active" : "badge-status badge-inactive" %>'>
                                            <%# Eval("Status") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <div class="d-flex gap-2">
                                            <asp:LinkButton runat="server" CommandName="EditConn" CommandArgument='<%# Eval("ConnId") %>' 
                                                CssClass="btn-portal btn-outline-portal btn-sm-portal">
                                                <i class="bi bi-pencil"></i> Edit
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("ConnId") + "|" + Eval("Status") %>'
                                                CssClass='<%# (string)Eval("Status") == "ACTIVE" ? "btn-portal btn-ghost btn-sm-portal text-danger-portal" : "btn-portal btn-ghost btn-sm-portal text-success-portal" %>'>
                                                <i class='<%# (string)Eval("Status") == "ACTIVE" ? "bi bi-x-circle" : "bi bi-check-circle" %>'></i> 
                                                <%# (string)Eval("Status") == "ACTIVE" ? "Deactivate" : "Activate" %>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center py-4 text-muted">
                                    <i class="bi bi-inbox fs-2 d-block mb-2"></i>
                                    No connection records found.
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
            <div class="modal fade portal-modal" id="modalConn" tabindex="-1" data-bs-backdrop="static">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalTitle" runat="server">Add Connection Record</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfConnId" runat="server" Value="0" />
                            
                            <div class="row">
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">PC NO <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtPcno" runat="server" CssClass="form-control-portal" MaxLength="20" placeholder="e.g. 123456" />
                                </div>
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Connection Type <span class="text-danger">*</span></label>
                                    <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="INTERNET" Value="INTERNET" />
                                        <asp:ListItem Text="INTRANET" Value="INTRANET" />
                                        <asp:ListItem Text="PRINTER" Value="PRINTER" />
                                        <asp:ListItem Text="WIFI" Value="WIFI" />
                                        <asp:ListItem Text="OTHER" Value="OTHER" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">IP Address</label>
                                    <asp:TextBox ID="txtIp" runat="server" CssClass="form-control-portal" MaxLength="50" placeholder="e.g. 192.168.1.50" />
                                </div>
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">MAC Address</label>
                                    <asp:TextBox ID="txtMac" runat="server" CssClass="form-control-portal" MaxLength="50" placeholder="e.g. 00:1A:2B:3C:4D:5E" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6 form-group-portal mb-0">
                                    <label class="form-label-portal">Switch Name / Room</label>
                                    <asp:TextBox ID="txtSwitch" runat="server" CssClass="form-control-portal" MaxLength="100" placeholder="e.g. SW-Floor-1" />
                                </div>
                                <div class="col-md-6 form-group-portal mb-0">
                                    <label class="form-label-portal">Port Number</label>
                                    <asp:TextBox ID="txtPort" runat="server" CssClass="form-control-portal" MaxLength="50" placeholder="e.g. Fa0/12" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn-portal btn-ghost" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnSave" runat="server" Text="Save Record" CssClass="btn-portal btn-primary-portal" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        let connModal = null;
        
        function getModal() {
            if (!connModal) connModal = new bootstrap.Modal(document.getElementById('modalConn'));
            return connModal;
        }

        function openAddModal() {
            document.getElementById('<%= hfConnId.ClientID %>').value = "0";
            document.getElementById('<%= txtPcno.ClientID %>').value = "";
            document.getElementById('<%= txtPcno.ClientID %>').removeAttribute('readonly');
            document.getElementById('<%= ddlType.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= txtIp.ClientID %>').value = "";
            document.getElementById('<%= txtMac.ClientID %>').value = "";
            document.getElementById('<%= txtSwitch.ClientID %>').value = "";
            document.getElementById('<%= txtPort.ClientID %>').value = "";
            document.getElementById('<%= modalTitle.ClientID %>').innerText = "Add Connection Record";
            getModal().show();
        }

        function openEditModal() {
            document.getElementById('<%= txtPcno.ClientID %>').setAttribute('readonly', 'readonly');
            getModal().show();
        }

        function closeConnModal() {
            if (connModal) connModal.hide();
        }
    </script>
</asp:Content>
