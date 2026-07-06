using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ComplaintsPortal.DataAccess
{
    /// <summary>
    /// Central place for all Oracle connection/command handling.
    /// Connection string key expected in Web.config:
    ///   <connectionStrings>
    ///     <add name="PortalDb" connectionString="User Id=PORTAL_APP;Password=...;Data Source=YOUR_TNS;" />
    ///   </connectionStrings>
    /// </summary>
    public static class DbHelper
    {
        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["PortalDb"].ConnectionString; }
        }

        public static OracleConnection GetConnection()
        {
            return new OracleConnection(ConnectionString);
        }

        public static DataTable ExecuteReader(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                using (var adapter = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Executes an INSERT and returns the auto-generated primary key
        /// (populated by the table's BEFORE INSERT trigger) via a RETURNING INTO clause.
        /// The sql must end with: RETURNING <PK_COLUMN> INTO :out_id
        /// </summary>
        public static int ExecuteInsertReturningId(string sql, OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.AddRange(parameters);
                var outParam = new OracleParameter("out_id", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).Value);
            }
        }

        public static OracleParameter Param(string name, object value)
        {
            return new OracleParameter(name, value ?? DBNull.Value);
        }
    }
}
