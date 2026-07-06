<%@ Page Title="New Request" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="NewRequest.aspx.cs" Inherits="ComplaintsPortal.Web.Employee.NewRequest" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div>
            <h4 class="page-title"><i class="bi bi-plus-circle"></i> New Service Request</h4>
            <p class="page-subtitle">Submit a new complaint or request for IT/Infrastructure services.</p>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-8">
            <div class="portal-card">
                <div class="portal-card-header">
                    <h5 class="portal-card-title"><i class="bi bi-pencil-square"></i> Request Details</h5>
                </div>
                <div class="portal-card-body">
                    <asp:UpdatePanel ID="upRequest" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Service Category <span class="text-danger">*</span></label>
                                    <asp:DropDownList ID="ddlService" runat="server" CssClass="form-select" DataTextField="ServiceName" DataValueField="ServiceId"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlService_SelectedIndexChanged" />
                                </div>
                                <div class="col-md-6 form-group-portal">
                                    <label class="form-label-portal">Request Type <span class="text-danger">*</span></label>
                                    <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-select" DataTextField="TypeName" DataValueField="RequestTypeId"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlRequestType_SelectedIndexChanged" />
                                </div>
                            </div>

                            <div class="form-group-portal" id="divSubType" runat="server">
                                <label class="form-label-portal">Sub-Type <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlSubType" runat="server" CssClass="form-select" DataTextField="SubTypeName" DataValueField="SubTypeId"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlSubType_SelectedIndexChanged" />
                            </div>

                            <!-- Visual Flow Preview -->
                            <div class="flow-preview mt-3 mb-4">
                                <span class="flow-label">Approval Workflow Route:</span>
                                <div class="d-flex align-items-center flex-wrap gap-2">
                                    <asp:Literal ID="litFlowPreview" runat="server" />
                                </div>
                            </div>

                            <h6 class="fw-600 mb-3 text-secondary border-bottom pb-2">Location & Details</h6>

                            <asp:Repeater ID="rptCustomFields" runat="server" OnItemDataBound="rptCustomFields_ItemDataBound">
                                <ItemTemplate>
                                    <div class="form-group-portal mb-3">
                                        <label class="form-label-portal">
                                            <%# Eval("FieldLabel") %> 
                                            <asp:Literal runat="server" Visible='<%# (string)Eval("IsMandatory") == "Y" %>' Text="<span class='text-danger'>*</span>" />
                                        </label>
                                        <asp:HiddenField ID="hfFieldId" runat="server" Value='<%# Eval("FieldId") %>' />
                                        <asp:HiddenField ID="hfIsMandatory" runat="server" Value='<%# Eval("IsMandatory") %>' />
                                        <asp:HiddenField ID="hfFieldType" runat="server" Value='<%# Eval("FieldType") %>' />

                                        <!-- Hidden controls, only the matching type is shown in code-behind -->
                                        <asp:TextBox ID="txtSingle" runat="server" CssClass="form-control-portal" Visible="false" />
                                        <asp:TextBox ID="txtMulti" runat="server" CssClass="form-control-portal" TextMode="MultiLine" Rows="3" Visible="false" />
                                        <asp:TextBox ID="txtNumber" runat="server" CssClass="form-control-portal" TextMode="Number" Visible="false" />
                                        <asp:TextBox ID="txtDate" runat="server" CssClass="form-control-portal" TextMode="Date" Visible="false" />
                                        <asp:DropDownList ID="ddlDropdown" runat="server" CssClass="form-select" Visible="false" />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <div class="row">
                                <div class="col-md-4 form-group-portal">
                                    <label class="form-label-portal">Building</label>
                                    <asp:TextBox ID="txtBuilding" runat="server" CssClass="form-control-portal" MaxLength="100" placeholder="e.g. Main Bldg" />
                                </div>
                                <div class="col-md-4 form-group-portal">
                                    <label class="form-label-portal">Floor</label>
                                    <asp:TextBox ID="txtFloor" runat="server" CssClass="form-control-portal" MaxLength="50" placeholder="e.g. Ground" />
                                </div>
                                <div class="col-md-4 form-group-portal">
                                    <label class="form-label-portal">Room No.</label>
                                    <asp:TextBox ID="txtRoomNo" runat="server" CssClass="form-control-portal" MaxLength="50" placeholder="e.g. 101" />
                                </div>
                            </div>

                            <div class="form-group-portal mb-4">
                                <label class="form-label-portal">Description <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control-portal" TextMode="MultiLine" Rows="4" placeholder="Please provide detailed information about your request..." />
                            </div>

                            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger-portal fw-500 d-block mb-3" />
                            
                            <div class="d-flex justify-content-end gap-2">
                                <asp:Button ID="btnSubmit" runat="server" Text="Submit Request" CssClass="btn-portal btn-primary-portal" OnClick="btnSubmit_Click" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <div class="col-lg-4">
            <div class="alert-portal alert-info">
                <i class="bi bi-info-circle-fill"></i>
                <div>
                    <strong>Need Help?</strong><br/>
                    Select the appropriate Service Category and Request Type. The workflow path will automatically adjust based on your selection.
                </div>
            </div>
        </div>
    </div>
</asp:Content>
