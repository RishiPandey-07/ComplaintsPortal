using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class DashboardRepository
    {
        public DashboardStats GetStats(string pcno, bool isAdmin)
        {
            var stats = new DashboardStats();
            
            // Note: If isAdmin is false, we could filter by pcno. For now, assume dashboard is mostly for admins/managers.
            string filter = isAdmin ? "" : "WHERE req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno)";

            // 1. Total Requests
            string sqlTotal = $"SELECT COUNT(*) FROM TRN_REQUEST req {filter}";
            stats.TotalRequests = Convert.ToInt32(isAdmin ? DbHelper.ExecuteScalar(sqlTotal) : DbHelper.ExecuteScalar(sqlTotal, DbHelper.Param("pcno", pcno)));

            // 2. Pending Approvals (Status = IN_PROGRESS)
            string sqlPending = $"SELECT COUNT(*) FROM TRN_REQUEST req WHERE STATUS = 'IN_PROGRESS' {(isAdmin ? "" : "AND (req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno))")}";
            stats.PendingApprovals = Convert.ToInt32(isAdmin ? DbHelper.ExecuteScalar(sqlPending) : DbHelper.ExecuteScalar(sqlPending, DbHelper.Param("pcno", pcno)));

            // 3. Completed (Status = COMPLETED)
            string sqlCompleted = $"SELECT COUNT(*) FROM TRN_REQUEST req WHERE STATUS = 'COMPLETED' {(isAdmin ? "" : "AND (req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno))")}";
            stats.Completed = Convert.ToInt32(isAdmin ? DbHelper.ExecuteScalar(sqlCompleted) : DbHelper.ExecuteScalar(sqlCompleted, DbHelper.Param("pcno", pcno)));

            // 4. Rejected (Status = REJECTED)
            string sqlRejected = $"SELECT COUNT(*) FROM TRN_REQUEST req WHERE STATUS = 'REJECTED' {(isAdmin ? "" : "AND (req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno))")}";
            stats.Rejected = Convert.ToInt32(isAdmin ? DbHelper.ExecuteScalar(sqlRejected) : DbHelper.ExecuteScalar(sqlRejected, DbHelper.Param("pcno", pcno)));

            // 5. SLA Breached
            string sqlSla = $"SELECT COUNT(*) FROM TRN_REQUEST req WHERE STATUS NOT IN ('COMPLETED', 'CLOSED', 'REJECTED') AND SLA_DUE_DATE IS NOT NULL AND SLA_DUE_DATE < SYSTIMESTAMP {(isAdmin ? "" : "AND (req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno))")}";
            stats.SlaBreached = Convert.ToInt32(isAdmin ? DbHelper.ExecuteScalar(sqlSla) : DbHelper.ExecuteScalar(sqlSla, DbHelper.Param("pcno", pcno)));

            return stats;
        }

        public List<ChartDataPoint> GetRequestsByStatus(string pcno, bool isAdmin)
        {
            string sql = $@"SELECT STATUS AS Label, COUNT(*) AS Value 
                            FROM TRN_REQUEST req 
                            {(isAdmin ? "" : "WHERE req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno)")}
                            GROUP BY STATUS";
            var dt = isAdmin ? DbHelper.ExecuteReader(sql) : DbHelper.ExecuteReader(sql, DbHelper.Param("pcno", pcno));
            
            var list = new List<ChartDataPoint>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChartDataPoint
                {
                    Label = row["Label"].ToString(),
                    Value = Convert.ToInt32(row["Value"])
                });
            }
            return list;
        }

        public List<ChartDataPoint> GetRequestsByType(string pcno, bool isAdmin)
        {
            string sql = $@"SELECT rt.TYPE_NAME AS Label, COUNT(req.REQUEST_ID) AS Value 
                            FROM TRN_REQUEST req
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = req.REQUEST_TYPE_ID
                            {(isAdmin ? "" : "WHERE req.REQUESTER_PCNO = :pcno OR req.DIVISION_ID IN (SELECT DIVISION_ID FROM hrdata.empdetails WHERE PCNO = :pcno)")}
                            GROUP BY rt.TYPE_NAME
                            ORDER BY Value DESC";
            var dt = isAdmin ? DbHelper.ExecuteReader(sql) : DbHelper.ExecuteReader(sql, DbHelper.Param("pcno", pcno));
            
            var list = new List<ChartDataPoint>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChartDataPoint
                {
                    Label = row["Label"].ToString(),
                    Value = Convert.ToInt32(row["Value"])
                });
            }
            return list;
        }
    }
}
