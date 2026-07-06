using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class TechExpertRepository
    {
        public List<TechExpertMapping> GetAll()
        {
            string sql = @"SELECT m.MAPPING_ID, m.PCNO, e.NAME AS EMP_NAME, m.SERVICE_ID, s.SERVICE_NAME,
                                   m.REQUEST_TYPE_ID, rt.TYPE_NAME, m.ASSIGNED_BY, m.ASSIGNED_DATE, m.IS_ACTIVE
                            FROM TRN_TECH_EXPERT_SERVICE m
                            JOIN MST_SERVICE s ON s.SERVICE_ID = m.SERVICE_ID
                            LEFT JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = m.REQUEST_TYPE_ID
                            LEFT JOIN hrdata.empdetails e ON e.PCNO = m.PCNO
                            ORDER BY m.ASSIGNED_DATE DESC";
            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<TechExpertMapping>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new TechExpertMapping
                {
                    MappingId = int.Parse(row["MAPPING_ID"].ToString()),
                    Pcno = row["PCNO"].ToString(),
                    EmployeeName = row["EMP_NAME"] == DBNull.Value ? "" : row["EMP_NAME"].ToString(),
                    ServiceId = int.Parse(row["SERVICE_ID"].ToString()),
                    ServiceName = row["SERVICE_NAME"].ToString(),
                    RequestTypeId = row["REQUEST_TYPE_ID"] == DBNull.Value ? (int?)null : int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                    RequestTypeName = row["TYPE_NAME"] == DBNull.Value ? "All types" : row["TYPE_NAME"].ToString(),
                    AssignedBy = row["ASSIGNED_BY"].ToString(),
                    AssignedDate = Convert.ToDateTime(row["ASSIGNED_DATE"]),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        /// <summary>Used by the Technical Expert Pool screen to find which experts qualify for a request.</summary>
        public List<string> GetExpertPcnosForRequestType(int serviceId, int requestTypeId)
        {
            string sql = @"SELECT DISTINCT PCNO FROM TRN_TECH_EXPERT_SERVICE
                            WHERE SERVICE_ID = :serviceId
                              AND (REQUEST_TYPE_ID = :requestTypeId OR REQUEST_TYPE_ID IS NULL)
                              AND IS_ACTIVE = 'Y'";
            var dt = DbHelper.ExecuteReader(sql,
                DbHelper.Param("serviceId", serviceId),
                DbHelper.Param("requestTypeId", requestTypeId));
            var list = new List<string>();
            foreach (DataRow row in dt.Rows) list.Add(row["PCNO"].ToString());
            return list;
        }

        public void Assign(TechExpertMapping m)
        {
            string sql = @"INSERT INTO TRN_TECH_EXPERT_SERVICE (PCNO, SERVICE_ID, REQUEST_TYPE_ID, ASSIGNED_BY, ASSIGNED_DATE, IS_ACTIVE)
                            VALUES (:pcno, :serviceId, :requestTypeId, :assignedBy, SYSDATE, 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("pcno", m.Pcno),
                DbHelper.Param("serviceId", m.ServiceId),
                DbHelper.Param("requestTypeId", m.RequestTypeId),
                DbHelper.Param("assignedBy", m.AssignedBy));
        }

        public void SetActive(int mappingId, bool active)
        {
            string sql = "UPDATE TRN_TECH_EXPERT_SERVICE SET IS_ACTIVE = :flag WHERE MAPPING_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("flag", active ? "Y" : "N"),
                DbHelper.Param("id", mappingId));
        }
    }
}
