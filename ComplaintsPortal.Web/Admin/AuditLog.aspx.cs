using System;
using ComplaintsPortal.DataAccess;

namespace ComplaintsPortal.Web.Admin
{
    public partial class AuditLog : BasePage
    {
        private readonly AuditRepository _auditRepo = new AuditRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            DateTime? from = null, to = null;
            DateTime parsed;
            if (DateTime.TryParse(txtFrom.Text, out parsed)) from = parsed;
            if (DateTime.TryParse(txtTo.Text, out parsed)) to = parsed;

            gvAudit.DataSource = _auditRepo.Search(
                string.IsNullOrWhiteSpace(txtPcno.Text) ? null : txtPcno.Text.Trim(),
                string.IsNullOrWhiteSpace(txtModule.Text) ? null : txtModule.Text.Trim(),
                from, to);
            gvAudit.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
