using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;

namespace ComplaintsPortal.Web.Admin
{
    public partial class StandbyAssignment : BasePage
    {
        private readonly StandbyService _standbyService = new StandbyService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Only IT admins should be able to see everyone's standbys.
                // For now, the BusinessLogic method handles retrieving all if Admin, else just for CurrentPcno.
                BindGrid();
            }
        }

        private void BindGrid()
        {
            gvStandby.DataSource = _standbyService.GetStandbys(CurrentPcno);
            gvStandby.DataBind();
            upGrid.Update();
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOriginalPcno.Text) || string.IsNullOrWhiteSpace(txtStandbyPcno.Text))
            {
                ShowToast("error", "Both Original PC NO and Standby PC NO are required.");
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            DateTime fromDate, toDate;
            if (!DateTime.TryParse(txtFromDate.Text, out fromDate) || !DateTime.TryParse(txtToDate.Text, out toDate))
            {
                ShowToast("error", "Invalid dates.");
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            if (toDate < fromDate)
            {
                ShowToast("error", "End Date cannot be before Start Date.");
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            string error = _standbyService.AssignStandby(txtOriginalPcno.Text.Trim(), txtStandbyPcno.Text.Trim(), fromDate, toDate, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            txtOriginalPcno.Text = "";
            txtStandbyPcno.Text = "";
            ShowToast("success", "Delegation assigned successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeStandbyModal();", true);
            BindGrid();
        }

        protected void gvStandby_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Revoke")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                _standbyService.RevokeStandby(id, CurrentPcno, CurrentIp);
                ShowToast("success", "Delegation revoked.");
                BindGrid();
            }
        }
    }
}
