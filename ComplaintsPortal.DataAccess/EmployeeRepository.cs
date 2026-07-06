using System.Data;

namespace ComplaintsPortal.DataAccess
{
    /// <summary>
    /// Read-only access to the existing hrdata.empdetails table.
    /// Adjust column names below (PCNO, NAME, DESIGNATION, DIVISION_ID)
    /// to match your actual hrdata.empdetails schema.
    /// </summary>
    public class EmployeeRepository
    {
        public DataRow GetByPcno(string pcno)
        {
            string sql = @"SELECT PCNO, NAME, DESIGNATION, DIVNAME, lrdemail
                            FROM hrdata.empdetails
                            WHERE PCNO = :pcno";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("pcno", pcno));
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataTable Search(string keyword)
        {
            string sql = @"SELECT PCNO, NAME, DESIGNATION
                            FROM hrdata.empdetails
                            WHERE UPPER(NAME) LIKE '%' || UPPER(:kw) || '%'
                               OR PCNO LIKE '%' || :kw || '%'
                            ORDER BY NAME";
            return DbHelper.ExecuteReader(sql, DbHelper.Param("kw", keyword));
        }

        public string GetEmailByPcno(string pcno)
        {
            if (string.IsNullOrEmpty(pcno)) return null;
            string sql = "SELECT lrdemail FROM hrdata.empdetails WHERE PCNO = :pcno";
            object result = DbHelper.ExecuteScalar(sql, DbHelper.Param("pcno", pcno));
            return result != null && result != System.DBNull.Value ? result.ToString() : null;
        }
    }
}
