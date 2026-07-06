using System;
using ComplaintsPortal.BusinessLogic;

namespace ComplaintsPortal.Web.Employee
{
    public partial class MyRequests : BasePage
    {
        private readonly RequestService _requestService = new RequestService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvMyRequests.DataSource = _requestService.GetMyRequests(CurrentPcno);
                gvMyRequests.DataBind();
            }
        }

        protected string GetStatusBadgeClass(string status)
        {
            switch (status)
            {
                case "SUBMITTED": return "badge bg-warning text-dark";
                case "IN_PROGRESS": return "badge bg-info text-dark";
                case "COMPLETED": return "badge bg-success";
                case "REJECTED": return "badge bg-danger";
                case "CLOSED": return "badge bg-secondary";
                default: return "badge bg-light text-dark";
            }
        }
    }
}
