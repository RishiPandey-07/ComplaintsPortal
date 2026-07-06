using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class Divisions : BasePage
    {
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvDivisions.DataSource = _masterDataService.GetDivisions();
            gvDivisions.DataBind();
            upGrid.Update();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowToast("error", "Division code and name are required.");
                return;
            }

            var d = new Division
            {
                DivisionId = int.Parse(hfDivisionId.Value),
                DivisionCode = txtCode.Text.Trim(),
                DivisionName = txtName.Text.Trim()
            };

            string error = _masterDataService.SaveDivision(d, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                return;
            }

            ShowToast("success", d.DivisionId == 0 ? "Division added successfully." : "Division updated successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeDivisionModal();", true);
            BindGrid();
        }

        protected void gvDivisions_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditDiv")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                var division = _masterDataService.GetDivisions().Find(d => d.DivisionId == id);
                if (division != null)
                {
                    hfDivisionId.Value = division.DivisionId.ToString();
                    txtCode.Text = division.DivisionCode;
                    txtName.Text = division.DivisionName;
                    modalTitle.InnerText = "Edit Division";
                    
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "openEditModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                bool currentlyActive = parts[1] == "Y";
                
                _masterDataService.SetDivisionActive(id, !currentlyActive, CurrentPcno, CurrentIp);
                ShowToast("success", currentlyActive ? "Division deactivated." : "Division activated.");
                BindGrid();
            }
        }
    }
}
