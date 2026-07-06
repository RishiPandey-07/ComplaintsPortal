using System;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.TechExpert
{
    public partial class ConnectionRegister : BasePage
    {
        private readonly ConnectionRegisterService _connService = new ConnectionRegisterService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid(null);
            }
        }

        private void BindGrid(string searchPcno)
        {
            gvConnections.DataSource = _connService.GetAll(searchPcno);
            gvConnections.DataBind();
            upGrid.Update();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string pcno = string.IsNullOrWhiteSpace(txtSearchPcno.Text) ? null : txtSearchPcno.Text.Trim();
            BindGrid(pcno);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPcno.Text))
            {
                ShowToast("error", "PC NO is required.");
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "openEditModal();", true);
                return;
            }

            var c = new ConnectionRecord
            {
                ConnId = int.Parse(hfConnId.Value),
                Pcno = txtPcno.Text.Trim(),
                ConnectionType = ddlType.SelectedValue,
                IpAddress = txtIp.Text.Trim(),
                MacAddress = txtMac.Text.Trim(),
                SwitchName = txtSwitch.Text.Trim(),
                PortNo = txtPort.Text.Trim()
            };

            string error = _connService.SaveConnection(c, CurrentPcno, CurrentIp);
            if (error != null)
            {
                ShowToast("error", error);
                ScriptManager.RegisterStartupScript(this, GetType(), "KeepModalOpen", "openEditModal();", true);
                return;
            }

            ShowToast("success", "Connection record saved successfully.");
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeConnModal();", true);
            BindGrid(null);
        }

        protected void gvConnections_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditConn")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                var conn = _connService.GetAll().Find(x => x.ConnId == id);
                if (conn != null)
                {
                    hfConnId.Value = conn.ConnId.ToString();
                    txtPcno.Text = conn.Pcno;
                    ddlType.SelectedValue = conn.ConnectionType;
                    txtIp.Text = conn.IpAddress;
                    txtMac.Text = conn.MacAddress;
                    txtSwitch.Text = conn.SwitchName;
                    txtPort.Text = conn.PortNo;
                    modalTitle.InnerText = "Edit Connection Record";
                    
                    upModal.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "openEditModal();", true);
                }
            }
            else if (e.CommandName == "ToggleActive")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                bool currentlyActive = parts[1] == "ACTIVE";
                _connService.SetStatus(id, !currentlyActive, CurrentPcno, CurrentIp);
                ShowToast("success", currentlyActive ? "Connection deactivated." : "Connection activated.");
                BindGrid(null);
            }
        }
    }
}
