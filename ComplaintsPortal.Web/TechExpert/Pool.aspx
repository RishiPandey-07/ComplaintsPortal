<%@ Page Title="Complaint Pool" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeBehind="Pool.aspx.cs" Inherits="ComplaintsPortal.Web.TechExpert.Pool" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h4>Complaint Pool</h4>
    <p class="text-muted">Unpicked complaints matching your assigned services, plus anything you've already picked up.</p>

    <asp:GridView ID="gvPool" runat="server" CssClass="table table-striped" AutoGenerateColumns="false"
        DataKeyNames="RequestId" OnRowCommand="gvPool_RowCommand">
        <Columns>
            <asp:BoundField DataField="RequestNumber" HeaderText="Request No." />
            <asp:BoundField DataField="RequestTypeName" HeaderText="Type" />
            <asp:BoundField DataField="RequesterName" HeaderText="Raised By" />
            <asp:BoundField DataField="Building" HeaderText="Building" />
            <asp:BoundField DataField="RoomNo" HeaderText="Room" />
            <asp:BoundField DataField="Description" HeaderText="Description" />
            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:TemplateField HeaderText="SLA Status">
                <ItemTemplate>
                    <span class='<%# IsSlaBreached(Eval("SlaDueDate")) ? "badge bg-danger" : "" %>'>
                        <%# IsSlaBreached(Eval("SlaDueDate")) ? "SLA Breached" : "" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton runat="server" CommandName="PickUp" CommandArgument='<%# Eval("RequestId") %>'
                        Text="Pick Up" CssClass="btn btn-sm btn-primary"
                        Visible='<%# string.IsNullOrEmpty((string)Eval("PickedByPcno")) %>' />

                    <asp:Panel runat="server" Visible='<%# (string)Eval("Status") == "IN_PROGRESS" %>'>
                        <asp:TextBox runat="server" ID="txtResolution" CssClass="form-control form-control-sm mt-1" placeholder="Resolution remarks" />
                        <asp:LinkButton runat="server" CommandName="Resolve" CommandArgument='<%# Eval("RequestId") %>'
                            Text="Mark Resolved" CssClass="btn btn-sm btn-success mt-1" />
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No complaints pending in your pool right now.
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
