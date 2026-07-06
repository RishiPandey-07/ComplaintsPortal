using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class RequestTypeRepository
    {
        public List<RequestType> GetAll(bool activeOnly = true, int? serviceId = null)
        {
            string sql = @"SELECT rt.REQUEST_TYPE_ID, rt.SERVICE_ID, s.SERVICE_NAME,
                                   rt.TYPE_CODE, rt.TYPE_NAME, rt.IS_FLOW_BASED, rt.SLA_HOURS, rt.IS_ACTIVE
                            FROM MST_REQUEST_TYPE rt
                            JOIN MST_SERVICE s ON s.SERVICE_ID = rt.SERVICE_ID
                            WHERE 1=1";
            if (activeOnly) sql += " AND rt.IS_ACTIVE = 'Y'";
            if (serviceId.HasValue) sql += " AND rt.SERVICE_ID = " + serviceId.Value;
            sql += " ORDER BY s.SERVICE_NAME, rt.TYPE_NAME";

            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<RequestType>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RequestType
                {
                    RequestTypeId = int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                    ServiceId = int.Parse(row["SERVICE_ID"].ToString()),
                    ServiceName = row["SERVICE_NAME"].ToString(),
                    TypeCode = row["TYPE_CODE"].ToString(),
                    TypeName = row["TYPE_NAME"].ToString(),
                    IsFlowBased = row["IS_FLOW_BASED"].ToString(),
                    SlaHours = row["SLA_HOURS"] == DBNull.Value ? (int?)null : int.Parse(row["SLA_HOURS"].ToString()),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        /// <summary>Used by the Employee "New Request" screen dropdown.</summary>
        public List<RequestType> GetByService(int serviceId, bool activeOnly = true)
        {
            string sql = @"SELECT REQUEST_TYPE_ID, SERVICE_ID, TYPE_CODE, TYPE_NAME, IS_FLOW_BASED, IS_ACTIVE
                            FROM MST_REQUEST_TYPE
                            WHERE SERVICE_ID = :serviceId";
            if (activeOnly) sql += " AND IS_ACTIVE = 'Y'";
            sql += " ORDER BY TYPE_NAME";

            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("serviceId", serviceId));
            var list = new List<RequestType>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RequestType
                {
                    RequestTypeId = int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                    ServiceId = int.Parse(row["SERVICE_ID"].ToString()),
                    TypeCode = row["TYPE_CODE"].ToString(),
                    TypeName = row["TYPE_NAME"].ToString(),
                    IsFlowBased = row["IS_FLOW_BASED"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public void Insert(RequestType r)
        {
            string sql = @"INSERT INTO MST_REQUEST_TYPE (REQUEST_TYPE_ID, SERVICE_ID, TYPE_CODE, TYPE_NAME, IS_FLOW_BASED, SLA_HOURS, IS_ACTIVE)
                            VALUES ((SELECT NVL(MAX(REQUEST_TYPE_ID),0)+1 FROM MST_REQUEST_TYPE), :ServiceId, :Code, :Name, :Flow, :Sla, 'Y')";

            var parameters = new Dictionary<string, object>
            {
                { "ServiceId", r.ServiceId },
                { "Code", r.TypeCode },
                { "Name", r.TypeName },
                { "Flow", r.IsFlowBased },
                { "Sla", r.SlaHours.HasValue ? (object)r.SlaHours.Value : DBNull.Value }
            };
            DbHelper.ExecuteNonQuery(sql, parameters);
        }

        public void Update(RequestType r)
        {
            string sql = "UPDATE MST_REQUEST_TYPE SET SERVICE_ID = :ServiceId, TYPE_CODE = :Code, TYPE_NAME = :Name, IS_FLOW_BASED = :Flow, SLA_HOURS = :Sla WHERE REQUEST_TYPE_ID = :Id";
            var parameters = new Dictionary<string, object>
            {
                { "ServiceId", r.ServiceId },
                { "Code", r.TypeCode },
                { "Name", r.TypeName },
                { "Flow", r.IsFlowBased },
                { "Sla", r.SlaHours.HasValue ? (object)r.SlaHours.Value : DBNull.Value },
                { "Id", r.RequestTypeId }
            };
            DbHelper.ExecuteNonQuery(sql, parameters);
        }

        public void SetActive(int requestTypeId, bool active)
        {
            string sql = "UPDATE MST_REQUEST_TYPE SET IS_ACTIVE = :flag WHERE REQUEST_TYPE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("flag", active ? "Y" : "N"),
                DbHelper.Param("id", requestTypeId));
        }
    }
}
