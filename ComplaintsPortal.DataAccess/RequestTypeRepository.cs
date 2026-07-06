using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class RequestTypeRepository
    {
        public List<RequestType> GetAll(bool activeOnly = false)
        {
            string sql = @"SELECT rt.REQUEST_TYPE_ID, rt.SERVICE_ID, s.SERVICE_NAME,
                                   rt.TYPE_CODE, rt.TYPE_NAME, rt.IS_FLOW_BASED, rt.IS_ACTIVE
                            FROM MST_REQUEST_TYPE rt
                            JOIN MST_SERVICE s ON s.SERVICE_ID = rt.SERVICE_ID";
            if (activeOnly) sql += " WHERE rt.IS_ACTIVE = 'Y'";
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

        public void Insert(RequestType rt)
        {
            // Phase 1: IS_FLOW_BASED is always 'N' - workflow engine arrives in Phase 2
            string sql = @"INSERT INTO MST_REQUEST_TYPE (SERVICE_ID, TYPE_CODE, TYPE_NAME, IS_FLOW_BASED, IS_ACTIVE)
                            VALUES (:serviceId, :code, :name, 'N', 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("serviceId", rt.ServiceId),
                DbHelper.Param("code", rt.TypeCode),
                DbHelper.Param("name", rt.TypeName));
        }

        public void Update(RequestType rt)
        {
            string sql = @"UPDATE MST_REQUEST_TYPE
                            SET SERVICE_ID = :serviceId, TYPE_CODE = :code, TYPE_NAME = :name
                            WHERE REQUEST_TYPE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("serviceId", rt.ServiceId),
                DbHelper.Param("code", rt.TypeCode),
                DbHelper.Param("name", rt.TypeName),
                DbHelper.Param("id", rt.RequestTypeId));
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
