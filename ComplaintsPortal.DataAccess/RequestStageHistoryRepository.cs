using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class RequestStageHistoryRepository
    {
        public void Insert(int requestId, int stageId, string actionByPcno, bool actedAsStandby,
            int? standbyForUserRoleId, string action, string remarks, int? nextStageId)
        {
            string sql = @"INSERT INTO TRN_REQUEST_STAGE_HISTORY
                                (REQUEST_ID, STAGE_ID, ACTION_BY_PCNO, ACTED_AS_STANDBY, STANDBY_FOR_USER_ROLE_ID,
                                 ACTION, REMARKS, ACTION_DATE, NEXT_STAGE_ID)
                            VALUES
                                (:requestId, :stageId, :pcno, :standby, :standbyForRoleId,
                                 :action, :remarks, SYSTIMESTAMP, :nextStageId)";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("requestId", requestId),
                DbHelper.Param("stageId", stageId),
                DbHelper.Param("pcno", actionByPcno),
                DbHelper.Param("standby", actedAsStandby ? "Y" : "N"),
                DbHelper.Param("standbyForRoleId", standbyForUserRoleId),
                DbHelper.Param("action", action),
                DbHelper.Param("remarks", remarks),
                DbHelper.Param("nextStageId", nextStageId));
        }

        public List<RequestStageHistory> GetForRequest(int requestId)
        {
            string sql = @"SELECT h.HISTORY_ID, h.REQUEST_ID, h.STAGE_ID, s.STAGE_NAME, h.ACTION_BY_PCNO,
                                   e.NAME AS ACTION_BY_NAME, h.ACTED_AS_STANDBY, h.STANDBY_FOR_USER_ROLE_ID,
                                   h.ACTION, h.REMARKS, h.ACTION_DATE, h.NEXT_STAGE_ID
                            FROM TRN_REQUEST_STAGE_HISTORY h
                            JOIN MST_WORKFLOW_STAGE s ON s.STAGE_ID = h.STAGE_ID
                            LEFT JOIN hrdata.empdetails e ON e.PCNO = h.ACTION_BY_PCNO
                            WHERE h.REQUEST_ID = :requestId
                            ORDER BY h.ACTION_DATE";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("requestId", requestId));

            var list = new List<RequestStageHistory>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RequestStageHistory
                {
                    HistoryId = long.Parse(row["HISTORY_ID"].ToString()),
                    RequestId = int.Parse(row["REQUEST_ID"].ToString()),
                    StageId = int.Parse(row["STAGE_ID"].ToString()),
                    StageName = row["STAGE_NAME"].ToString(),
                    ActionByPcno = row["ACTION_BY_PCNO"].ToString(),
                    ActionByName = row["ACTION_BY_NAME"] == DBNull.Value ? "" : row["ACTION_BY_NAME"].ToString(),
                    ActedAsStandby = row["ACTED_AS_STANDBY"].ToString(),
                    StandbyForUserRoleId = row["STANDBY_FOR_USER_ROLE_ID"] == DBNull.Value ? (int?)null : int.Parse(row["STANDBY_FOR_USER_ROLE_ID"].ToString()),
                    Action = row["ACTION"].ToString(),
                    Remarks = row["REMARKS"] == DBNull.Value ? "" : row["REMARKS"].ToString(),
                    ActionDate = Convert.ToDateTime(row["ACTION_DATE"]),
                    NextStageId = row["NEXT_STAGE_ID"] == DBNull.Value ? (int?)null : int.Parse(row["NEXT_STAGE_ID"].ToString())
                });
            }
            return list;
        }
    }
}
