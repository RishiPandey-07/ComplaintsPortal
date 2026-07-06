using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class Services : BasePage
    {
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvServices.DataSource = _masterDataService.GetServices();
            gvServices.DataBind();
            upGrid.Update();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowToast("error", "Service code and name are required.");
                return;
            }

            var s = new ServiceMaster
            {
                ServiceId = int.Parse(hfServiceId.Value),
                ServiceCode = txtCode.Text.Trim(),
                ServiceName = txtName.Text.Trim()
            };

            string error = _masterDataService.SaveService(s, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                return;
            }

            ShowToast("success", s.ServiceId == 0 ? "Service added successfully." : "Service updated successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeServiceModal();", true);
            BindGrid();
        }

        protected void gvServices_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditSvc")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                var svc = _masterDataService.GetServices().Find(x => x.ServiceId == id);
                if (svc != null)
                {
                    hfServiceId.Value = svc.ServiceId.ToString();
                    txtCode.Text = svc.ServiceCode;
                    txtName.Text = svc.ServiceName;
                    modalTitle.InnerText = "Edit Service";
                    
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "openEditModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                bool currentlyActive = parts[1] == "Y";
                _masterDataService.SetServiceActive(id, !currentlyActive, CurrentPcno, CurrentIp);
                ShowToast("success", currentlyActive ? "Service deactivated." : "Service activated.");
                BindGrid();
            }
        }
    }
}
