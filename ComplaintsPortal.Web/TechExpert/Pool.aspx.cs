using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComplaintsPortal.BusinessLogic;

namespace ComplaintsPortal.Web.TechExpert
{
    public partial class Pool : BasePage
    {
        private readonly RequestService _requestService = new RequestService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvPool.DataSource = _requestService.GetPoolForExpert(CurrentPcno);
            gvPool.DataBind();
        }

        protected void gvPool_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int requestId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "PickUp")
            {
                string error = _requestService.PickUp(requestId, CurrentPcno, CurrentIp);
                if (error != null)
                {
                    // Someone else grabbed it first - just refresh, the row will disappear/update.
                }
                BindGrid();
            }
            else if (e.CommandName == "Resolve")
            {
                var control = (Control)e.CommandSource;
                var row = (GridViewRow)control.NamingContainer;
                var txtResolution = (TextBox)row.FindControl("txtResolution");

                _requestService.MarkResolved(requestId, txtResolution.Text.Trim(), CurrentPcno, CurrentIp);
                BindGrid();
            }
        }
    }
}
