using System;
using ComplaintsPortal.BusinessLogic;

namespace ComplaintsPortal.Web.Approvals
{
    public partial class PendingApprovals : BasePage
    {
        private readonly WorkflowEngineService _workflowEngineService = new WorkflowEngineService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvPending.DataSource = _workflowEngineService.GetPendingApprovals(CurrentPcno);
                gvPending.DataBind();
            }
        }

        protected bool IsSlaBreached(object slaDueDate)
        {
            if (slaDueDate == null || slaDueDate == DBNull.Value) return false;
            return Convert.ToDateTime(slaDueDate) < DateTime.Now;
        }
    }
}
