using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class AuditRepository
    {
        public void Log(string pcno, string moduleName, string entityId, string actionType, string actionDesc, string ipAddress)
        {
            string sql = @"INSERT INTO TRN_AUDIT_LOG (PCNO, MODULE_NAME, ENTITY_ID, ACTION_TYPE, ACTION_DESC, IP_ADDRESS, ACTION_DATE)
                            VALUES (:pcno, :module, :entityId, :actionType, :actionDesc, :ip, SYSTIMESTAMP)";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("pcno", pcno),
                DbHelper.Param("module", moduleName),
                DbHelper.Param("entityId", entityId),
                DbHelper.Param("actionType", actionType),
                DbHelper.Param("actionDesc", actionDesc),
                DbHelper.Param("ip", ipAddress));
        }

        public List<AuditLog> Search(string pcnoFilter, string moduleFilter, DateTime? fromDate, DateTime? toDate)
        {
            string sql = @"SELECT AUDIT_ID, PCNO, MODULE_NAME, ENTITY_ID, ACTION_TYPE, ACTION_DESC, IP_ADDRESS, ACTION_DATE
                            FROM TRN_AUDIT_LOG
                            WHERE (:pcno IS NULL OR PCNO = :pcno)
                              AND (:module IS NULL OR MODULE_NAME = :module)
                              AND (:fromDate IS NULL OR ACTION_DATE >= :fromDate)
                              AND (:toDate IS NULL OR ACTION_DATE <= :toDate)
                            ORDER BY ACTION_DATE DESC
                            FETCH FIRST 500 ROWS ONLY";
            var dt = DbHelper.ExecuteReader(sql,
                DbHelper.Param("pcno", pcnoFilter),
                DbHelper.Param("module", moduleFilter),
                DbHelper.Param("fromDate", fromDate),
                DbHelper.Param("toDate", toDate));

            var list = new List<AuditLog>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AuditLog
                {
                    AuditId = long.Parse(row["AUDIT_ID"].ToString()),
                    Pcno = row["PCNO"].ToString(),
                    ModuleName = row["MODULE_NAME"].ToString(),
                    EntityId = row["ENTITY_ID"] == DBNull.Value ? "" : row["ENTITY_ID"].ToString(),
                    ActionType = row["ACTION_TYPE"].ToString(),
                    ActionDesc = row["ACTION_DESC"] == DBNull.Value ? "" : row["ACTION_DESC"].ToString(),
                    IpAddress = row["IP_ADDRESS"] == DBNull.Value ? "" : row["IP_ADDRESS"].ToString(),
                    ActionDate = Convert.ToDateTime(row["ACTION_DATE"])
                });
            }
            return list;
        }
    }
}
