using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class DivisionRepository
    {
        public List<Division> GetAll(bool activeOnly = false)
        {
            string sql = "SELECT DIVISION_ID, DIVISION_CODE, DIVISION_NAME, IS_ACTIVE FROM MST_DIVISION";
            if (activeOnly) sql += " WHERE IS_ACTIVE = 'Y'";
            sql += " ORDER BY DIVISION_NAME";

            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<Division>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Division
                {
                    DivisionId = int.Parse(row["DIVISION_ID"].ToString()),
                    DivisionCode = row["DIVISION_CODE"].ToString(),
                    DivisionName = row["DIVISION_NAME"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public void Insert(Division d)
        {
            string sql = @"INSERT INTO MST_DIVISION (DIVISION_CODE, DIVISION_NAME, IS_ACTIVE)
                            VALUES (:code, :name, 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("code", d.DivisionCode),
                DbHelper.Param("name", d.DivisionName));
        }

        public void Update(Division d)
        {
            string sql = @"UPDATE MST_DIVISION
                            SET DIVISION_CODE = :code, DIVISION_NAME = :name
                            WHERE DIVISION_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("code", d.DivisionCode),
                DbHelper.Param("name", d.DivisionName),
                DbHelper.Param("id", d.DivisionId));
        }

        public void SetActive(int divisionId, bool active)
        {
            string sql = "UPDATE MST_DIVISION SET IS_ACTIVE = :flag WHERE DIVISION_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("flag", active ? "Y" : "N"),
                DbHelper.Param("id", divisionId));
        }

        /// <summary>
        /// Resolves hrdata.empdetails.DIVNAME to MST_DIVISION.DIVISION_ID.
        /// Case-insensitive match. Returns 0 if not found.
        /// </summary>
        public int GetIdByName(string divisionName)
        {
            if (string.IsNullOrWhiteSpace(divisionName)) return 0;
            string sql = @"SELECT DIVISION_ID FROM MST_DIVISION
                           WHERE UPPER(DIVISION_NAME) = UPPER(:name) AND IS_ACTIVE = 'Y'
                           AND ROWNUM = 1";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("name", divisionName.Trim()));
            if (dt.Rows.Count == 0) return 0;
            return int.Parse(dt.Rows[0]["DIVISION_ID"].ToString());
        }
    }
}
