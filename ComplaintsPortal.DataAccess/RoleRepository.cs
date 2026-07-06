using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class RoleRepository
    {
        public List<Role> GetAll(bool activeOnly = false)
        {
            string sql = @"SELECT ROLE_ID, ROLE_CODE, ROLE_NAME, ROLE_CATEGORY, REQUIRES_DIVISION, IS_ACTIVE
                            FROM MST_ROLE";
            if (activeOnly) sql += " WHERE IS_ACTIVE = 'Y'";
            sql += " ORDER BY ROLE_CATEGORY, ROLE_NAME";

            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<Role>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Role
                {
                    RoleId = int.Parse(row["ROLE_ID"].ToString()),
                    RoleCode = row["ROLE_CODE"].ToString(),
                    RoleName = row["ROLE_NAME"].ToString(),
                    RoleCategory = row["ROLE_CATEGORY"].ToString(),
                    RequiresDivision = row["REQUIRES_DIVISION"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public void Insert(Role r)
        {
            string sql = @"INSERT INTO MST_ROLE (ROLE_CODE, ROLE_NAME, ROLE_CATEGORY, REQUIRES_DIVISION, IS_ACTIVE)
                            VALUES (:code, :name, :category, :reqDiv, 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("code", r.RoleCode),
                DbHelper.Param("name", r.RoleName),
                DbHelper.Param("category", r.RoleCategory),
                DbHelper.Param("reqDiv", r.RequiresDivision));
        }

        public void Update(Role r)
        {
            string sql = @"UPDATE MST_ROLE
                            SET ROLE_CODE = :code, ROLE_NAME = :name,
                                ROLE_CATEGORY = :category, REQUIRES_DIVISION = :reqDiv
                            WHERE ROLE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("code", r.RoleCode),
                DbHelper.Param("name", r.RoleName),
                DbHelper.Param("category", r.RoleCategory),
                DbHelper.Param("reqDiv", r.RequiresDivision),
                DbHelper.Param("id", r.RoleId));
        }

        public void SetActive(int roleId, bool active)
        {
            string sql = "UPDATE MST_ROLE SET IS_ACTIVE = :flag WHERE ROLE_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("flag", active ? "Y" : "N"),
                DbHelper.Param("id", roleId));
        }
    }
}
