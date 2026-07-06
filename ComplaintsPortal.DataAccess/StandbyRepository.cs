using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class StandbyRepository
    {
        public List<StandbyAssignment> GetAll()
        {
            string sql = @"SELECT sb.STANDBY_ID, sb.USER_ROLE_ID, ur.PCNO AS PRIMARY_PCNO, e1.NAME AS PRIMARY_NAME,
                                   r.ROLE_NAME, d.DIVISION_NAME, sb.STANDBY_PCNO, e2.NAME AS STANDBY_NAME,
                                   sb.EFFECTIVE_FROM, sb.EFFECTIVE_TO, sb.IS_ACTIVE
                            FROM TRN_STANDBY_MAP sb
                            JOIN TRN_USER_ROLE ur ON ur.USER_ROLE_ID = sb.USER_ROLE_ID
                            JOIN MST_ROLE r ON r.ROLE_ID = ur.ROLE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = ur.DIVISION_ID
                            LEFT JOIN hrdata.empdetails e1 ON e1.PCNO = ur.PCNO
                            LEFT JOIN hrdata.empdetails e2 ON e2.PCNO = sb.STANDBY_PCNO
                            ORDER BY sb.EFFECTIVE_FROM DESC";
            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<StandbyAssignment>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new StandbyAssignment
                {
                    StandbyId = int.Parse(row["STANDBY_ID"].ToString()),
                    UserRoleId = int.Parse(row["USER_ROLE_ID"].ToString()),
                    PrimaryPcno = row["PRIMARY_PCNO"].ToString(),
                    PrimaryName = row["PRIMARY_NAME"] == DBNull.Value ? "" : row["PRIMARY_NAME"].ToString(),
                    RoleName = row["ROLE_NAME"].ToString(),
                    DivisionName = row["DIVISION_NAME"] == DBNull.Value ? "" : row["DIVISION_NAME"].ToString(),
                    StandbyPcno = row["STANDBY_PCNO"].ToString(),
                    StandbyName = row["STANDBY_NAME"] == DBNull.Value ? "" : row["STANDBY_NAME"].ToString(),
                    EffectiveFrom = Convert.ToDateTime(row["EFFECTIVE_FROM"]),
                    EffectiveTo = row["EFFECTIVE_TO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["EFFECTIVE_TO"]),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        /// <summary>All USER_ROLE_IDs for which this PCNO is currently an active standby.</summary>
        public List<int> GetActiveStandbyUserRoleIds(string standbyPcno)
        {
            string sql = @"SELECT USER_ROLE_ID FROM TRN_STANDBY_MAP
                            WHERE STANDBY_PCNO = :pcno AND IS_ACTIVE = 'Y'
                              AND (EFFECTIVE_TO IS NULL OR EFFECTIVE_TO >= SYSDATE)";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("pcno", standbyPcno));
            var list = new List<int>();
            foreach (DataRow row in dt.Rows) list.Add(int.Parse(row["USER_ROLE_ID"].ToString()));
            return list;
        }

        public void Insert(int userRoleId, string standbyPcno)
        {
            string sql = @"INSERT INTO TRN_STANDBY_MAP (USER_ROLE_ID, STANDBY_PCNO, EFFECTIVE_FROM, IS_ACTIVE)
                            VALUES (:userRoleId, :standbyPcno, SYSDATE, 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("userRoleId", userRoleId),
                DbHelper.Param("standbyPcno", standbyPcno));
        }

        public void EndStandby(int standbyId)
        {
            string sql = "UPDATE TRN_STANDBY_MAP SET IS_ACTIVE = 'N', EFFECTIVE_TO = SYSDATE WHERE STANDBY_ID = :id";
            DbHelper.ExecuteNonQuery(sql, DbHelper.Param("id", standbyId));
        }
    }
}
