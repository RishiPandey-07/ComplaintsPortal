using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class UserRoleRepository
    {
        /// <summary>All active role assignments for one employee (used at login to build session role list).</summary>
        public List<UserRole> GetActiveRolesForUser(string pcno)
        {
            string sql = @"SELECT ur.USER_ROLE_ID, ur.PCNO, ur.ROLE_ID, r.ROLE_NAME, r.ROLE_CODE,
                                   ur.DIVISION_ID, d.DIVISION_NAME, ur.EFFECTIVE_FROM, ur.EFFECTIVE_TO,
                                   ur.IS_ACTIVE, ur.CREATED_BY, ur.CREATED_DATE
                            FROM TRN_USER_ROLE ur
                            JOIN MST_ROLE r ON r.ROLE_ID = ur.ROLE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = ur.DIVISION_ID
                            WHERE ur.PCNO = :pcno
                              AND ur.IS_ACTIVE = 'Y'
                              AND (ur.EFFECTIVE_TO IS NULL OR ur.EFFECTIVE_TO >= SYSDATE)
                            ORDER BY r.ROLE_CATEGORY, d.DIVISION_NAME";

            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("pcno", pcno));
            return MapList(dt);
        }

        /// <summary>Every role assignment (any employee) for a given role - e.g. "who is GD of Division X".</summary>
        public List<UserRole> GetByRole(int roleId)
        {
            string sql = @"SELECT ur.USER_ROLE_ID, ur.PCNO, ur.ROLE_ID, r.ROLE_NAME, r.ROLE_CODE,
                                   ur.DIVISION_ID, d.DIVISION_NAME, ur.EFFECTIVE_FROM, ur.EFFECTIVE_TO,
                                   ur.IS_ACTIVE, ur.CREATED_BY, ur.CREATED_DATE
                            FROM TRN_USER_ROLE ur
                            JOIN MST_ROLE r ON r.ROLE_ID = ur.ROLE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = ur.DIVISION_ID
                            WHERE ur.ROLE_ID = :roleId AND ur.IS_ACTIVE = 'Y'
                            ORDER BY d.DIVISION_NAME";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("roleId", roleId));
            return MapList(dt);
        }

        /// <summary>Full assignment list for the admin screen, with employee name joined from hrdata.</summary>
        public List<UserRole> GetAllForAdminGrid(string pcnoFilter)
        {
            string sql = @"SELECT ur.USER_ROLE_ID, ur.PCNO, e.NAME AS EMP_NAME, ur.ROLE_ID, r.ROLE_NAME,
                                   ur.DIVISION_ID, d.DIVISION_NAME, ur.EFFECTIVE_FROM, ur.EFFECTIVE_TO,
                                   ur.IS_ACTIVE, ur.CREATED_BY, ur.CREATED_DATE
                            FROM TRN_USER_ROLE ur
                            JOIN MST_ROLE r ON r.ROLE_ID = ur.ROLE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = ur.DIVISION_ID
                            LEFT JOIN hrdata.empdetails e ON e.PCNO = ur.PCNO
                            WHERE (:pcnoFilter IS NULL OR ur.PCNO = :pcnoFilter)
                            ORDER BY ur.CREATED_DATE DESC";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("pcnoFilter", pcnoFilter));
            var list = MapList(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
                list[i].EmployeeName = dt.Rows[i]["EMP_NAME"] == DBNull.Value ? "" : dt.Rows[i]["EMP_NAME"].ToString();
            return list;
        }

        public UserRole GetById(int userRoleId)
        {
            string sql = @"SELECT ur.USER_ROLE_ID, ur.PCNO, ur.ROLE_ID, r.ROLE_NAME, r.ROLE_CODE,
                                   ur.DIVISION_ID, d.DIVISION_NAME, ur.EFFECTIVE_FROM, ur.EFFECTIVE_TO,
                                   ur.IS_ACTIVE, ur.CREATED_BY, ur.CREATED_DATE
                            FROM TRN_USER_ROLE ur
                            JOIN MST_ROLE r ON r.ROLE_ID = ur.ROLE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = ur.DIVISION_ID
                            WHERE ur.USER_ROLE_ID = :id";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("id", userRoleId));
            var list = MapList(dt);
            return list.Count > 0 ? list[0] : null;
        }

        public void Assign(UserRole ur, string createdBy)
        {
            string sql = @"INSERT INTO TRN_USER_ROLE (PCNO, ROLE_ID, DIVISION_ID, EFFECTIVE_FROM, IS_ACTIVE, CREATED_BY)
                            VALUES (:pcno, :roleId, :divisionId, :effFrom, 'Y', :createdBy)";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("pcno", ur.Pcno),
                DbHelper.Param("roleId", ur.RoleId),
                DbHelper.Param("divisionId", ur.DivisionId),
                DbHelper.Param("effFrom", ur.EffectiveFrom == default(DateTime) ? DateTime.Now : ur.EffectiveFrom),
                DbHelper.Param("createdBy", createdBy));
        }

        public void EndAssignment(int userRoleId)
        {
            string sql = @"UPDATE TRN_USER_ROLE
                            SET IS_ACTIVE = 'N', EFFECTIVE_TO = SYSDATE
                            WHERE USER_ROLE_ID = :id";
            DbHelper.ExecuteNonQuery(sql, DbHelper.Param("id", userRoleId));
        }

        private List<UserRole> MapList(DataTable dt)
        {
            var list = new List<UserRole>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new UserRole
                {
                    UserRoleId = int.Parse(row["USER_ROLE_ID"].ToString()),
                    Pcno = row["PCNO"].ToString(),
                    RoleId = int.Parse(row["ROLE_ID"].ToString()),
                    RoleName = row["ROLE_NAME"].ToString(),
                    DivisionId = row["DIVISION_ID"] == DBNull.Value ? (int?)null : int.Parse(row["DIVISION_ID"].ToString()),
                    DivisionName = row["DIVISION_NAME"] == DBNull.Value ? "" : row["DIVISION_NAME"].ToString(),
                    EffectiveFrom = Convert.ToDateTime(row["EFFECTIVE_FROM"]),
                    EffectiveTo = row["EFFECTIVE_TO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["EFFECTIVE_TO"]),
                    IsActive = row["IS_ACTIVE"].ToString(),
                    CreatedBy = row["CREATED_BY"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CREATED_DATE"])
                });
            }
            return list;
        }
    }
}
