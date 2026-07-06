using System;
using System.Linq;
using ComplaintsPortal.BusinessLogic;

namespace ComplaintsPortal.Web.Admin
{
    public partial class Dashboard : BasePage
    {
        private readonly DashboardService _dashboardService = new DashboardService();

        protected string StatusLabelsJson { get; set; } = "[]";
        protected string StatusValuesJson { get; set; } = "[]";
        protected string TypeLabelsJson { get; set; } = "[]";
        protected string TypeValuesJson { get; set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            // For now, assume any user reaching this page sees their relevant dashboard view.
            // If they are an Admin (OIC_IT), they see everything. Otherwise, they see their own/division's stats.
            bool isAdmin = CurrentUser.IsAdmin; 
            
            // Note: If you want all employees to have a dashboard, we could check roles.
            // But this is in the Admin folder, so typically accessed by Admins.

            // Load top KPI numbers
            var stats = _dashboardService.GetStats(CurrentPcno, isAdmin);
            lblTotal.InnerText = stats.TotalRequests.ToString();
            lblPending.InnerText = stats.PendingApprovals.ToString();
            lblInProgress.InnerText = stats.InProgress.ToString();
            lblCompleted.InnerText = stats.Completed.ToString();
            lblRejected.InnerText = stats.Rejected.ToString();
            lblSlaBreached.InnerText = stats.SlaBreached.ToString();

            // Load Chart Data
            var statusData = _dashboardService.GetRequestsByStatus(CurrentPcno, isAdmin);
            StatusLabelsJson = Newtonsoft.Json.JsonConvert.SerializeObject(statusData.Select(x => x.Label).ToList());
            StatusValuesJson = Newtonsoft.Json.JsonConvert.SerializeObject(statusData.Select(x => x.Value).ToList());

            var typeData = _dashboardService.GetRequestsByType(CurrentPcno, isAdmin);
            TypeLabelsJson = Newtonsoft.Json.JsonConvert.SerializeObject(typeData.Select(x => x.Label).ToList());
            TypeValuesJson = Newtonsoft.Json.JsonConvert.SerializeObject(typeData.Select(x => x.Value).ToList());
        }
    }
}
