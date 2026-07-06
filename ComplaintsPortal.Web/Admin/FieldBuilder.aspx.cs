using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class FieldBuilder : BasePage
    {
        private readonly MasterDataService _masterDataService = new MasterDataService();
        private readonly WorkflowAdminService _workflowAdminService = new WorkflowAdminService();
        private readonly FieldDefinitionService _fieldService = new FieldDefinitionService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRequestTypeDropdowns();
                BindSubTypeDropdowns(true);
            }
        }

        private void BindRequestTypeDropdowns()
        {
            var types = _masterDataService.GetRequestTypes(activeOnly: true);
            
            ddlFilterRequestType.DataSource = types;
            ddlFilterRequestType.DataBind();
            
            ddlRequestType.DataSource = types;
            ddlRequestType.DataBind();
        }

        private void BindSubTypeDropdowns(bool initialLoad = false)
        {
            ddlFilterSubType.Items.Clear();
            ddlFilterSubType.Items.Add(new System.Web.UI.WebControls.ListItem("(All Sub-Types)", ""));
            
            ddlSubType.Items.Clear();
            ddlSubType.Items.Add(new System.Web.UI.WebControls.ListItem("(Applies to all Sub-Types)", ""));

            if (ddlFilterRequestType.Items.Count > 0)
            {
                int reqTypeId = int.Parse(ddlFilterRequestType.SelectedValue);
                var subtypes = _workflowAdminService.GetSubTypesByRequestType(reqTypeId);
                
                foreach (var st in subtypes)
                {
                    ddlFilterSubType.Items.Add(new System.Web.UI.WebControls.ListItem(st.SubTypeName, st.SubTypeId.ToString()));
                    ddlSubType.Items.Add(new System.Web.UI.WebControls.ListItem(st.SubTypeName, st.SubTypeId.ToString()));
                }
            }

            if (initialLoad) BindGrid();
        }

        protected void ddlFilterRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Sync the modal dropdown to match the filter just as a convenience
            if (ddlRequestType.Items.FindByValue(ddlFilterRequestType.SelectedValue) != null)
                ddlRequestType.SelectedValue = ddlFilterRequestType.SelectedValue;
                
            BindSubTypeDropdowns();
            BindGrid();
        }

        protected void ddlRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Sync back so sub-types load correctly inside the modal
            if (ddlFilterRequestType.Items.FindByValue(ddlRequestType.SelectedValue) != null)
                ddlFilterRequestType.SelectedValue = ddlRequestType.SelectedValue;
            
            BindSubTypeDropdowns();
            ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "openEditModal();", true);
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            if (ddlFilterRequestType.Items.Count == 0) return;

            int reqTypeId = int.Parse(ddlFilterRequestType.SelectedValue);
            int? subTypeId = string.IsNullOrEmpty(ddlFilterSubType.SelectedValue) ? (int?)null : int.Parse(ddlFilterSubType.SelectedValue);

            gvFields.DataSource = _fieldService.GetFieldsByRequestType(reqTypeId, subTypeId);
            gvFields.DataBind();
            upGrid.Update();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtLabel.Text))
            {
                ShowToast("error", "Code and Label are required.");
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "openEditModal();", true);
                return;
            }

            int order = 10;
            int.TryParse(txtDisplayOrder.Text, out order);

            var f = new FieldDefinition
            {
                FieldId = int.Parse(hfFieldId.Value),
                RequestTypeId = int.Parse(ddlRequestType.SelectedValue),
                SubTypeId = string.IsNullOrEmpty(ddlSubType.SelectedValue) ? (int?)null : int.Parse(ddlSubType.SelectedValue),
                FieldCode = txtCode.Text.Trim(),
                FieldLabel = txtLabel.Text.Trim(),
                FieldType = ddlFieldType.SelectedValue,
                DropdownOptions = ddlFieldType.SelectedValue == "DROPDOWN" ? txtDropdownOptions.Text.Trim() : null,
                IsMandatory = chkMandatory.Checked ? "Y" : "N",
                DisplayOrder = order
            };

            string error = _fieldService.SaveField(f, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "openEditModal();", true);
                return;
            }

            ShowToast("success", "Field saved successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeFieldModal();", true);
            BindGrid();
        }

        protected void gvFields_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditField")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                int reqTypeId = int.Parse(ddlFilterRequestType.SelectedValue);
                int? subTypeId = string.IsNullOrEmpty(ddlFilterSubType.SelectedValue) ? (int?)null : int.Parse(ddlFilterSubType.SelectedValue);
                
                var field = _fieldService.GetFieldsByRequestType(reqTypeId, subTypeId).Find(x => x.FieldId == id);
                if (field != null)
                {
                    hfFieldId.Value = field.FieldId.ToString();
                    ddlRequestType.SelectedValue = field.RequestTypeId.ToString();
                    ddlSubType.SelectedValue = field.SubTypeId.HasValue ? field.SubTypeId.Value.ToString() : "";
                    txtCode.Text = field.FieldCode;
                    txtLabel.Text = field.FieldLabel;
                    ddlFieldType.SelectedValue = field.FieldType;
                    txtDropdownOptions.Text = field.DropdownOptions;
                    chkMandatory.Checked = field.IsMandatory == "Y";
                    txtDisplayOrder.Text = field.DisplayOrder.ToString();
                    modalTitle.InnerText = "Edit Custom Field";
                    
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "openEditModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                bool currentlyActive = parts[1] == "Y";
                _fieldService.SetFieldActive(id, !currentlyActive, CurrentPcno, CurrentIp);
                ShowToast("success", currentlyActive ? "Field deactivated." : "Field activated.");
                BindGrid();
            }
        }
    }
}
