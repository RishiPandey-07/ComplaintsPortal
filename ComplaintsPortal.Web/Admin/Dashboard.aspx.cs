using System;
using System.Linq;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.DataAccess;

namespace ComplaintsPortal.Web.Admin
{
    public partial class Dashboard : BasePage
    {
        private readonly DashboardService _dashboardService = new DashboardService();
        private readonly WorkflowEngineService _workflowEngine = new WorkflowEngineService();
        private readonly RequestRepository _reqRepo = new RequestRepository();
        private readonly RequestStageHistoryRepository _histRepo = new RequestStageHistoryRepository();

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
            bool isAdmin = CurrentUser.IsAdmin;
            bool isApprover = SessionContext.Roles.Any(r => r.RoleName != "Employee");

            // 1. Employee Panel (Always Visible)
            var myRequests = _reqRepo.GetMyRequests(CurrentPcno);
            lblEmpActive.InnerText = myRequests.Count(r => r.Status == "IN_PROGRESS" || r.Status == "PENDING").ToString();
            lblEmpResolved.InnerText = myRequests.Count(r => r.Status == "COMPLETED" || r.Status == "CLOSED").ToString();

            // 2. Approver Panel
            if (isApprover)
            {
                pnlApprover.Visible = true;
                var pendingApprovals = _workflowEngine.GetPendingApprovals(CurrentPcno);
                lblApproverPending.InnerText = pendingApprovals.Count.ToString();
                
                // Get count of requests sanctioned/forwarded/completed by this user
                string sql = "SELECT COUNT(DISTINCT REQUEST_ID) FROM TRN_REQUEST_STAGE_HISTORY WHERE ACTION_BY_PCNO = :pcno AND ACTION IN ('FORWARDED', 'COMPLETED', 'REJECTED')";
                lblApproverSanctioned.InnerText = Convert.ToInt32(DbHelper.ExecuteScalar(sql, DbHelper.Param("pcno", CurrentPcno))).ToString();
            }

            // 3. Global Admin Panel
            if (isAdmin)
            {
                pnlGlobal.Visible = true;
                var stats = _dashboardService.GetStats(CurrentPcno, isAdmin);
                lblTotal.InnerText = stats.TotalRequests.ToString();
                lblPending.InnerText = stats.PendingApprovals.ToString();
                lblInProgress.InnerText = stats.InProgress.ToString();
                lblCompleted.InnerText = stats.Completed.ToString();
                lblRejected.InnerText = stats.Rejected.ToString();
                lblSlaBreached.InnerText = stats.SlaBreached.ToString();

                var statusData = _dashboardService.GetRequestsByStatus(CurrentPcno, isAdmin);
                var typeData = _dashboardService.GetRequestsByType(CurrentPcno, isAdmin);

                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                StatusLabelsJson = serializer.Serialize(statusData.Select(x => x.Label).ToList());
                StatusValuesJson = serializer.Serialize(statusData.Select(x => x.Value).ToList());

                TypeLabelsJson = serializer.Serialize(typeData.Select(x => x.Label).ToList());
                TypeValuesJson = serializer.Serialize(typeData.Select(x => x.Value).ToList());
            }
        }
    }
}
