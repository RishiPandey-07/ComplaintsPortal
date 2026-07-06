using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class ReportRepository
    {
        public List<ComplaintRequest> GetRequestsForReport(DateTime? from, DateTime? to, string status, string pcno, bool isAdmin)
        {
            string sql = @"SELECT req.REQUEST_ID, req.REQUEST_NUMBER, req.REQUEST_TYPE_ID, rt.TYPE_NAME,
                                   req.STATUS, req.REQUESTER_PCNO, e.NAME AS REQUESTER_NAME, req.DIVISION_ID, d.DIVISION_NAME,
                                   req.SUBMITTED_DATE, req.CLOSED_DATE, req.SLA_DUE_DATE
                            FROM TRN_REQUEST req
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = req.REQUEST_TYPE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = req.DIVISION_ID
                            LEFT JOIN hrdata.empdetails e ON e.PCNO = req.REQUESTER_PCNO
                            WHERE 1=1 ";

            if (from.HasValue)
            {
                sql += " AND req.SUBMITTED_DATE >= :fromDate ";
            }
            if (to.HasValue)
            {
                // Add 1 day to include the entire 'to' date
                sql += " AND req.SUBMITTED_DATE < :toDate ";
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND req.STATUS = :status ";
            }
            if (!isAdmin)
            {
                sql += " AND (req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno)) ";
            }

            sql += " ORDER BY req.SUBMITTED_DATE DESC";

            var parameters = new Dictionary<string, object>();
            if (from.HasValue) parameters.Add("fromDate", from.Value);
            if (to.HasValue) parameters.Add("toDate", to.Value.AddDays(1));
            if (!string.IsNullOrEmpty(status)) parameters.Add("status", status);
            if (!isAdmin) parameters.Add("pcno", pcno);

            var dt = DbHelper.ExecuteReader(sql, parameters);
            
            var list = new List<ComplaintRequest>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ComplaintRequest
                {
                    RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                    RequestNumber = row["REQUEST_NUMBER"].ToString(),
                    RequestTypeName = row["TYPE_NAME"].ToString(),
                    DivisionName = row["DIVISION_NAME"] == DBNull.Value ? "" : row["DIVISION_NAME"].ToString(),
                    RequesterName = row["REQUESTER_NAME"] == DBNull.Value ? "" : row["REQUESTER_NAME"].ToString(),
                    Status = row["STATUS"].ToString(),
                    SubmittedDate = Convert.ToDateTime(row["SUBMITTED_DATE"]),
                    ClosedDate = row.Table.Columns.Contains("CLOSED_DATE") && row["CLOSED_DATE"] != DBNull.Value ? Convert.ToDateTime(row["CLOSED_DATE"]) : (DateTime?)null,
                    SlaDueDate = row.Table.Columns.Contains("SLA_DUE_DATE") && row["SLA_DUE_DATE"] != DBNull.Value ? Convert.ToDateTime(row["SLA_DUE_DATE"]) : (DateTime?)null
                });
            }
            return list;
        }
    }
}
