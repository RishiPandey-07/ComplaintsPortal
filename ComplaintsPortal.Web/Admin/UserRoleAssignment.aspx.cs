using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class UserRoleAssignment : BasePage
    {
        private readonly UserRoleService _userRoleService = new UserRoleService();
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRoleDropdown();
                BindDivisionDropdown();
                BindGrid(null);
                ToggleDivisionVisibility();
            }
        }

        private void BindRoleDropdown()
        {
            ddlRole.DataSource = _masterDataService.GetRoles(activeOnly: true);
            ddlRole.DataBind();
        }

        private void BindDivisionDropdown()
        {
            ddlDivision.DataSource = _masterDataService.GetDivisions(activeOnly: true);
            ddlDivision.DataBind();
        }

        private void BindGrid(string pcnoFilter)
        {
            gvAssignments.DataSource = _userRoleService.GetAssignments(pcnoFilter);
            gvAssignments.DataBind();
            upGrid.Update();
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleDivisionVisibility();
            // Re-open modal if they change role while assigning
            ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
        }

        private void ToggleDivisionVisibility()
        {
            var role = _masterDataService.GetRoles().Find(r => r.RoleId.ToString() == ddlRole.SelectedValue);
            divDivision.Visible = role != null && role.RequiresDivision == "Y";
            upModal.Update();
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPcno.Text))
            {
                ShowToast("error", "Please enter a PC NO.");
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            var ur = new UserRole
            {
                Pcno = txtPcno.Text.Trim(),
                RoleId = int.Parse(ddlRole.SelectedValue),
                DivisionId = divDivision.Visible ? int.Parse(ddlDivision.SelectedValue) : (int?)null,
                EffectiveFrom = DateTime.Now
            };

            string error = _userRoleService.AssignRole(ur, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            txtPcno.Text = "";
            ShowToast("success", "Role assigned successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeAssignModal();", true);
            BindGrid(null);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(string.IsNullOrWhiteSpace(txtSearchPcno.Text) ? null : txtSearchPcno.Text.Trim());
        }

        protected void gvAssignments_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "End")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                _userRoleService.EndAssignment(id, CurrentPcno, CurrentIp);
                ShowToast("success", "Role assignment revoked.");
                BindGrid(null);
            }
        }
    }
}
