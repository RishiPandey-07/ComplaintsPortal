<%@ Page Title="Workflow Builder" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="WorkflowBuilder.aspx.cs" Inherits="ComplaintsPortal.Web.Admin.WorkflowBuilder" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h4>Workflow Builder</h4>

    <div class="card mb-3" style="max-width:600px;">
        <div class="card-header">1. Create a workflow for a request type</div>
        <div class="card-body">
            <div class="mb-2">
                <label>Request Type</label>
                <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-control" DataTextField="TypeName" DataValueField="RequestTypeId" />
            </div>
            <div class="mb-2">
                <label>Sub-Type (optional - leave blank if this workflow applies to the whole request type)</label>
                <asp:DropDownList ID="ddlSubType" runat="server" CssClass="form-control" DataTextField="SubTypeName" DataValueField="SubTypeId" />
            </div>
            <div class="mb-2">
                <label>Workflow Name</label>
                <asp:TextBox ID="txtWorkflowName" runat="server" CssClass="form-control" MaxLength="150" placeholder="e.g. Internet New Connection - General" />
            </div>
            <asp:Label ID="lblCreateMessage" runat="server" CssClass="text-danger d-block mb-2" />
            <asp:Button ID="btnCreateWorkflow" runat="server" Text="Create Workflow" CssClass="btn btn-primary" OnClick="btnCreateWorkflow_Click" />
        </div>
    </div>

    <div class="card mb-3" style="max-width:900px;">
        <div class="card-header">2. Existing workflows - select one to add/view stages</div>
        <div class="card-body">
            <asp:DropDownList ID="ddlExistingWorkflow" runat="server" CssClass="form-control mb-2" AutoPostBack="true"
                OnSelectedIndexChanged="ddlExistingWorkflow_SelectedIndexChanged" DataTextField="WorkflowName" DataValueField="WorkflowId" />

            <asp:Panel ID="pnlStages" runat="server" Visible="false">
                <hr />
                <h6>Add a stage</h6>
                <div class="row g-2 mb-2">
                    <div class="col-md-2"><asp:TextBox ID="txtStageSeq" runat="server" CssClass="form-control" placeholder="Seq (1,2,3...)" /></div>
                    <div class="col-md-3"><asp:TextBox ID="txtStageName" runat="server" CssClass="form-control" placeholder="Stage name" /></div>
                    <div class="col-md-2">
                        <asp:DropDownList ID="ddlApproverRole" runat="server" CssClass="form-control" DataTextField="RoleName" DataValueField="RoleId" />
                    </div>
                    <div class="col-md-2">
                        <asp:DropDownList ID="ddlAssignmentMode" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Specific Person" Value="SPECIFIC_PERSON" />
                            <asp:ListItem Text="Pool" Value="POOL" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <asp:DropDownList ID="ddlRejectTarget" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Reject to Previous Stage" Value="PREVIOUS_STAGE" />
                            <asp:ListItem Text="Reject to Employee" Value="EMPLOYEE" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row g-2 mb-2">
                    <div class="col-md-3 form-check">
                        <asp:CheckBox ID="chkRequiresPrintout" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label">Requires Printout</label>
                    </div>
                    <div class="col-md-3 form-check">
                        <asp:CheckBox ID="chkRequiresAssetSubmission" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label">Requires Asset Submission</label>
                    </div>
                    <div class="col-md-3 form-check">
                        <asp:CheckBox ID="chkRequiresAssetAck" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label">Requires Asset Received Ack.</label>
                    </div>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtSlaHours" runat="server" CssClass="form-control" placeholder="SLA hours (optional)" />
                    </div>
                </div>
                <asp:Label ID="lblStageMessage" runat="server" CssClass="text-danger d-block mb-2" />
                <asp:Button ID="btnAddStage" runat="server" Text="Add Stage" CssClass="btn btn-primary mb-3" OnClick="btnAddStage_Click" />

                <h6>Stages in this workflow (in order)</h6>
                <asp:GridView ID="gvStages" runat="server" CssClass="table table-striped" AutoGenerateColumns="false"
                    DataKeyNames="StageId" OnRowCommand="gvStages_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="StageSeq" HeaderText="Seq" />
                        <asp:BoundField DataField="StageName" HeaderText="Stage" />
                        <asp:BoundField DataField="ApproverRoleName" HeaderText="Approver Role" />
                        <asp:BoundField DataField="AssignmentMode" HeaderText="Assignment" />
                        <asp:BoundField DataField="RejectTarget" HeaderText="Reject Target" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="Remove" CommandArgument='<%# Eval("StageId") %>' Text="Remove" CssClass="btn btn-sm btn-link text-danger" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
