using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class RequestSubTypeRepository
    {
        public List<RequestSubType> GetAll(bool activeOnly = false)
        {
            string sql = @"SELECT st.SUBTYPE_ID, st.REQUEST_TYPE_ID, rt.TYPE_NAME, st.SUBTYPE_CODE, st.SUBTYPE_NAME, st.IS_ACTIVE
                            FROM MST_REQUEST_SUBTYPE st
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = st.REQUEST_TYPE_ID";
            if (activeOnly) sql += " WHERE st.IS_ACTIVE = 'Y'";
            sql += " ORDER BY rt.TYPE_NAME, st.SUBTYPE_NAME";

            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<RequestSubType>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RequestSubType
                {
                    SubTypeId = int.Parse(row["SUBTYPE_ID"].ToString()),
                    RequestTypeId = int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                    RequestTypeName = row["TYPE_NAME"].ToString(),
                    SubTypeCode = row["SUBTYPE_CODE"].ToString(),
                    SubTypeName = row["SUBTYPE_NAME"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public List<RequestSubType> GetByRequestType(int requestTypeId, bool activeOnly = true)
        {
            string sql = @"SELECT SUBTYPE_ID, REQUEST_TYPE_ID, SUBTYPE_CODE, SUBTYPE_NAME, IS_ACTIVE
                            FROM MST_REQUEST_SUBTYPE
                            WHERE REQUEST_TYPE_ID = :typeId";
            if (activeOnly) sql += " AND IS_ACTIVE = 'Y'";
            sql += " ORDER BY SUBTYPE_NAME";

            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("typeId", requestTypeId));
            var list = new List<RequestSubType>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RequestSubType
                {
                    SubTypeId = int.Parse(row["SUBTYPE_ID"].ToString()),
                    RequestTypeId = int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                    SubTypeCode = row["SUBTYPE_CODE"].ToString(),
                    SubTypeName = row["SUBTYPE_NAME"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public void Insert(RequestSubType st)
        {
            string sql = @"INSERT INTO MST_REQUEST_SUBTYPE (REQUEST_TYPE_ID, SUBTYPE_CODE, SUBTYPE_NAME, IS_ACTIVE)
                            VALUES (:typeId, :code, :name, 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("typeId", st.RequestTypeId),
                DbHelper.Param("code", st.SubTypeCode),
                DbHelper.Param("name", st.SubTypeName));
        }

        public void Update(RequestSubType st)
        {
            string sql = @"UPDATE MST_REQUEST_SUBTYPE
                            SET REQUEST_TYPE_ID = :typeId, SUBTYPE_CODE = :code, SUBTYPE_NAME = :name
                            WHERE SUBTYPE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("typeId", st.RequestTypeId),
                DbHelper.Param("code", st.SubTypeCode),
                DbHelper.Param("name", st.SubTypeName),
                DbHelper.Param("id", st.SubTypeId));
        }

        public void SetActive(int subTypeId, bool active)
        {
            string sql = "UPDATE MST_REQUEST_SUBTYPE SET IS_ACTIVE = :flag WHERE SUBTYPE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("flag", active ? "Y" : "N"),
                DbHelper.Param("id", subTypeId));
        }
    }
}
