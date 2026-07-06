using System;
using System.Linq;
using ComplaintsPortal.BusinessLogic;

namespace ComplaintsPortal.Web.Approvals
{
    public partial class ApprovalDetail : BasePage
    {
        private readonly WorkflowEngineService _workflowEngineService = new WorkflowEngineService();
        private readonly RequestService _requestService = new RequestService();

        private int RequestId => int.Parse(Request.QueryString["id"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRequest();
            }
        }

        private void LoadRequest()
        {
            var request = _requestService.GetById(RequestId);
            if (request == null)
            {
                litRequestNumber.Text = "Request not found";
                return;
            }

            litRequestNumber.Text = request.RequestNumber + " &middot; " + request.RequestTypeName;
            litRequestSummary.Text = request.DivisionName + " division &middot; Status: " + request.Status;

            rptTimeline.DataSource = _workflowEngineService.GetTimeline(RequestId);
            rptTimeline.DataBind();

            // Only show the action panel if this request is genuinely pending with the
            // logged-in person right now (direct role or standby) - re-checked server-side
            // rather than trusting the query string, for safety.
            var pending = _workflowEngineService.GetPendingApprovals(CurrentPcno);
            var match = pending.FirstOrDefault(p => p.RequestId == RequestId);
            pnlAction.Visible = match != null;

            if (match != null)
            {
                ViewState["ActedAsStandby"] = match.IsStandbyItem;
                ViewState["StandbyForUserRoleId"] = match.StandbyForUserRoleId;
            }
        }

        protected string GetIconClass(string status)
        {
            switch (status)
            {
                case "COMPLETED": return "bi bi-check-circle-fill text-success";
                case "CURRENT": return "bi bi-record-circle text-primary";
                default: return "bi bi-circle text-muted";
            }
        }

        protected void btnForward_Click(object sender, EventArgs e)
        {
            bool actedAsStandby = ViewState["ActedAsStandby"] != null && (bool)ViewState["ActedAsStandby"];
            int? standbyForUserRoleId = ViewState["StandbyForUserRoleId"] as int?;

            string error = _workflowEngineService.Approve(RequestId, txtRemarks.Text.Trim(), CurrentPcno,
                actedAsStandby, standbyForUserRoleId, CurrentIp);

            if (error != null) { lblMessage.Text = error; return; }

            Response.Redirect("~/Approvals/PendingApprovals.aspx");
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            bool actedAsStandby = ViewState["ActedAsStandby"] != null && (bool)ViewState["ActedAsStandby"];
            int? standbyForUserRoleId = ViewState["StandbyForUserRoleId"] as int?;

            string error = _workflowEngineService.Reject(RequestId, txtRemarks.Text.Trim(), CurrentPcno,
                actedAsStandby, standbyForUserRoleId, CurrentIp);

            if (error != null) { lblMessage.Text = error; return; }

            Response.Redirect("~/Approvals/PendingApprovals.aspx");
        }
    }
}
