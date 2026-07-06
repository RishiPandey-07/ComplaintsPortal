using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class RequestTypes : BasePage
    {
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindServiceDropdown();
                BindGrid();
            }
        }

        private void BindServiceDropdown()
        {
            ddlService.DataSource = _masterDataService.GetServices(activeOnly: true);
            ddlService.DataBind();
        }

        private void BindGrid()
        {
            gvRequestTypes.DataSource = _masterDataService.GetRequestTypes();
            gvRequestTypes.DataBind();
            upGrid.Update();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowToast("error", "Type code and name are required.");
                return;
            }

            var rt = new RequestType
            {
                RequestTypeId = int.Parse(hfRequestTypeId.Value),
                ServiceId = int.Parse(ddlService.SelectedValue),
                TypeCode = txtCode.Text.Trim(),
                TypeName = txtName.Text.Trim(),
                IsFlowBased = chkFlowBased.Checked ? "Y" : "N"
            };

            string error = _masterDataService.SaveRequestType(rt, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                return;
            }

            ShowToast("success", rt.RequestTypeId == 0 ? "Request Type added successfully." : "Request Type updated successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeRequestTypeModal();", true);
            BindGrid();
        }

        protected void gvRequestTypes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditType")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                var rt = _masterDataService.GetRequestTypes().Find(x => x.RequestTypeId == id);
                if (rt != null)
                {
                    hfRequestTypeId.Value = rt.RequestTypeId.ToString();
                    ddlService.SelectedValue = rt.ServiceId.ToString();
                    txtCode.Text = rt.TypeCode;
                    txtName.Text = rt.TypeName;
                    chkFlowBased.Checked = rt.IsFlowBased == "Y";
                    modalTitle.InnerText = "Edit Request Type";
                    
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "openEditModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                bool currentlyActive = parts[1] == "Y";
                _masterDataService.SetRequestTypeActive(id, !currentlyActive, CurrentPcno, CurrentIp);
                ShowToast("success", currentlyActive ? "Request Type deactivated." : "Request Type activated.");
                BindGrid();
            }
        }
    }
}
