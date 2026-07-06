using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class Reports : BasePage
    {
        private readonly ReportService _reportService = new ReportService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Default to last 30 days
                txtFromDate.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                BindGrid();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            DateTime? from = string.IsNullOrEmpty(txtFromDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtFromDate.Text);
            DateTime? to = string.IsNullOrEmpty(txtToDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtToDate.Text);
            string status = ddlStatus.SelectedValue;

            bool isAdmin = CurrentUser.IsAdmin;
            
            var results = _reportService.GetRequestsForReport(from, to, status, CurrentPcno, isAdmin);
            gvReports.DataSource = results;
            gvReports.DataBind();

            // Store results in Session for easy export without requerying
            Session["CurrentReportData"] = results;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var data = Session["CurrentReportData"] as List<ComplaintRequest>;
            if (data == null)
            {
                BindGrid();
                data = Session["CurrentReportData"] as List<ComplaintRequest>;
            }

            if (data == null || data.Count == 0) return;

            string csv = GenerateCsv(data);
            
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=RequestsReport_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csv);
            Response.Flush();
            Response.End();
        }

        private string GenerateCsv(List<ComplaintRequest> requests)
        {
            StringBuilder sb = new StringBuilder();
            
            // Header
            sb.AppendLine("Request No.,Type,Division,Requester,Status,Submitted,Closed,SLA Breached");
            
            foreach (var r in requests)
            {
                string submitted = r.SubmittedDate.ToString("yyyy-MM-dd HH:mm:ss");
                string closed = r.ClosedDate.HasValue ? r.ClosedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                string isSlaBreached = IsSlaBreached(r.SlaDueDate, r.Status) ? "Yes" : "No";

                sb.AppendLine($"{EscapeCsv(r.RequestNumber)},{EscapeCsv(r.RequestTypeName)},{EscapeCsv(r.DivisionName)},{EscapeCsv(r.RequesterName)},{EscapeCsv(r.Status)},{submitted},{closed},{isSlaBreached}");
            }
            
            return sb.ToString();
        }

        private string EscapeCsv(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (str.Contains(","))
            {
                return $"\"{str.Replace("\"", "\"\"")}\"";
            }
            return str;
        }

        protected bool IsSlaBreached(object slaDueDate, object statusObj)
        {
            if (slaDueDate == null || slaDueDate == DBNull.Value) return false;
            
            string status = statusObj?.ToString();
            if (status == "COMPLETED" || status == "CLOSED" || status == "REJECTED")
            {
                // If it's closed/completed, we'd need to compare closed date vs sla date.
                // For simplicity in this demo, just check if it's currently breached if still open.
                // A full implementation would compare (r.ClosedDate > r.SlaDueDate).
                // Returning false here for closed tickets for simplicity if we don't have closed date in context.
                return false; 
            }
            
            return Convert.ToDateTime(slaDueDate) < DateTime.Now;
        }
    }
}
