using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class Roles : BasePage
    {
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvRoles.DataSource = _masterDataService.GetRoles();
            gvRoles.DataBind();
            upGrid.Update();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowToast("error", "Role code and name are required.");
                return;
            }

            var r = new Role
            {
                RoleId = int.Parse(hfRoleId.Value),
                RoleCode = txtCode.Text.Trim(),
                RoleName = txtName.Text.Trim(),
                RoleCategory = ddlCategory.SelectedValue,
                RequiresDivision = chkReqDiv.Checked ? "Y" : "N"
            };

            string error = _masterDataService.SaveRole(r, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                return;
            }

            ShowToast("success", r.RoleId == 0 ? "Role added successfully." : "Role updated successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeRoleModal();", true);
            BindGrid();
        }

        protected void gvRoles_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRole")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                var role = _masterDataService.GetRoles().Find(x => x.RoleId == id);
                if (role != null)
                {
                    hfRoleId.Value = role.RoleId.ToString();
                    txtCode.Text = role.RoleCode;
                    txtName.Text = role.RoleName;
                    
                    var catItem = ddlCategory.Items.FindByValue(role.RoleCategory);
                    if (catItem != null) ddlCategory.SelectedValue = role.RoleCategory;

                    chkReqDiv.Checked = role.RequiresDivision == "Y";
                    modalTitle.InnerText = "Edit Role";
                    
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "openEditModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                bool currentlyActive = parts[1] == "Y";
                _masterDataService.SetRoleActive(id, !currentlyActive, CurrentPcno, CurrentIp);
                ShowToast("success", currentlyActive ? "Role deactivated." : "Role activated.");
                BindGrid();
            }
        }
    }
}
