using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;

namespace ComplaintsPortal.Web.Admin
{
    public partial class TechExpertAssignment : BasePage
    {
        private readonly TechExpertService _techExpertService = new TechExpertService();
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindServiceDropdown();
                BindRequestTypeDropdown();
                BindGrid();
            }
        }

        private void BindServiceDropdown()
        {
            ddlService.DataSource = _masterDataService.GetServices(activeOnly: true);
            ddlService.DataBind();
        }

        private void BindRequestTypeDropdown()
        {
            ddlRequestType.Items.Clear();
            ddlRequestType.Items.Add(new System.Web.UI.WebControls.ListItem("(All Types)", ""));
            
            if (ddlService.Items.Count > 0)
            {
                int svcId = int.Parse(ddlService.SelectedValue);
                var types = _masterDataService.GetRequestTypesByService(svcId);
                foreach (var t in types)
                    ddlRequestType.Items.Add(new System.Web.UI.WebControls.ListItem(t.TypeName, t.RequestTypeId.ToString()));
            }
            upModal.Update();
        }

        protected void ddlService_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRequestTypeDropdown();
            ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
        }

        private void BindGrid()
        {
            gvExperts.DataSource = _techExpertService.GetAllMappings();
            gvExperts.DataBind();
            upGrid.Update();
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPcno.Text))
            {
                ShowToast("error", "Please enter a PC NO.");
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            int svcId = int.Parse(ddlService.SelectedValue);
            int? reqTypeId = string.IsNullOrEmpty(ddlRequestType.SelectedValue) ? (int?)null : int.Parse(ddlRequestType.SelectedValue);

            string error = _techExpertService.AssignExpert(txtPcno.Text.Trim(), svcId, reqTypeId, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "keepModalOpen();", true);
                return;
            }

            txtPcno.Text = "";
            ShowToast("success", "Technical Expert assigned successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeExpertModal();", true);
            BindGrid();
        }

        protected void gvExperts_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Deactivate")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                _techExpertService.DeactivateMapping(id, CurrentPcno, CurrentIp);
                ShowToast("success", "Expert mapping removed.");
                BindGrid();
            }
        }
    }
}
