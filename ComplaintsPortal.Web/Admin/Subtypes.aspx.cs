using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class Subtypes : BasePage
    {
        private readonly WorkflowAdminService _workflowAdminService = new WorkflowAdminService();
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRequestTypeDropdowns();
                BindGrid();
            }
        }

        private void BindRequestTypeDropdowns()
        {
            var reqTypes = _masterDataService.GetRequestTypes(activeOnly: true);
            
            ddlRequestType.DataSource = reqTypes;
            ddlRequestType.DataBind();

            ddlFilterRequestType.Items.Clear();
            ddlFilterRequestType.Items.Add(new System.Web.UI.WebControls.ListItem("All Request Types", "0"));
            foreach (var rt in reqTypes)
                ddlFilterRequestType.Items.Add(new System.Web.UI.WebControls.ListItem(rt.TypeName, rt.RequestTypeId.ToString()));
        }

        private void BindGrid()
        {
            int filterId = int.Parse(ddlFilterRequestType.SelectedValue);
            gvSubtypes.DataSource = _workflowAdminService.GetAllSubTypes(filterId == 0 ? (int?)null : filterId);
            gvSubtypes.DataBind();
            upGrid.Update();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowToast("error", "Sub-Type code and name are required.");
                return;
            }

            var st = new RequestSubType
            {
                SubTypeId = int.Parse(hfSubtypeId.Value),
                RequestTypeId = int.Parse(ddlRequestType.SelectedValue),
                SubTypeCode = txtCode.Text.Trim(),
                SubTypeName = txtName.Text.Trim()
            };

            string error = _workflowAdminService.SaveSubType(st, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                return;
            }

            ShowToast("success", st.SubTypeId == 0 ? "Sub-Type added successfully." : "Sub-Type updated successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeSubtypeModal();", true);
            BindGrid();
        }

        protected void gvSubtypes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditType")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                // We fetch all and find it since there's no GetSubTypeById yet
                var st = _workflowAdminService.GetAllSubTypes().Find(x => x.SubTypeId == id);
                if (st != null)
                {
                    hfSubtypeId.Value = st.SubTypeId.ToString();
                    ddlRequestType.SelectedValue = st.RequestTypeId.ToString();
                    txtCode.Text = st.SubTypeCode;
                    txtName.Text = st.SubTypeName;
                    modalTitle.InnerText = "Edit Sub-Type";
                    
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "openEditModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                bool currentlyActive = parts[1] == "Y";
                _workflowAdminService.SetSubTypeActive(id, !currentlyActive, CurrentPcno, CurrentIp);
                ShowToast("success", currentlyActive ? "Sub-Type deactivated." : "Sub-Type activated.");
                BindGrid();
            }
        }
    }
}
