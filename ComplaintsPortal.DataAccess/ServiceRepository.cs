using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class ServiceRepository
    {
        public List<ServiceMaster> GetAll(bool activeOnly = false)
        {
            string sql = "SELECT SERVICE_ID, SERVICE_CODE, SERVICE_NAME, IS_ACTIVE FROM MST_SERVICE";
            if (activeOnly) sql += " WHERE IS_ACTIVE = 'Y'";
            sql += " ORDER BY SERVICE_NAME";

            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<ServiceMaster>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ServiceMaster
                {
                    ServiceId = int.Parse(row["SERVICE_ID"].ToString()),
                    ServiceCode = row["SERVICE_CODE"].ToString(),
                    ServiceName = row["SERVICE_NAME"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public void Insert(ServiceMaster s)
        {
            string sql = @"INSERT INTO MST_SERVICE (SERVICE_CODE, SERVICE_NAME, IS_ACTIVE)
                            VALUES (:code, :name, 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("code", s.ServiceCode),
                DbHelper.Param("name", s.ServiceName));
        }

        public void Update(ServiceMaster s)
        {
            string sql = @"UPDATE MST_SERVICE
                            SET SERVICE_CODE = :code, SERVICE_NAME = :name
                            WHERE SERVICE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("code", s.ServiceCode),
                DbHelper.Param("name", s.ServiceName),
                DbHelper.Param("id", s.ServiceId));
        }

        public void SetActive(int serviceId, bool active)
        {
            string sql = "UPDATE MST_SERVICE SET IS_ACTIVE = :flag WHERE SERVICE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("flag", active ? "Y" : "N"),
                DbHelper.Param("id", serviceId));
        }
    }
}
