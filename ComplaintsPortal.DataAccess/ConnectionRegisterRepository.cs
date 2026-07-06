using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class ConnectionRegisterRepository
    {
        public void EnsureTableExists()
        {
            try
            {
                string check = "SELECT COUNT(1) FROM user_tables WHERE table_name = 'TRN_CONNECTION_REGISTER'";
                var count = Convert.ToInt32(DbHelper.ExecuteScalar(check));
                if (count == 0)
                {
                    string ddl = @"
                        CREATE TABLE TRN_CONNECTION_REGISTER (
                            CONN_ID NUMBER PRIMARY KEY,
                            PCNO VARCHAR2(20) NOT NULL,
                            CONNECTION_TYPE VARCHAR2(50) NOT NULL,
                            IP_ADDRESS VARCHAR2(50),
                            MAC_ADDRESS VARCHAR2(50),
                            PORT_NO VARCHAR2(50),
                            SWITCH_NAME VARCHAR2(100),
                            ASSIGNED_DATE TIMESTAMP DEFAULT SYSTIMESTAMP,
                            STATUS VARCHAR2(20) DEFAULT 'ACTIVE'
                        )";
                    DbHelper.ExecuteNonQuery(ddl);
                }
            }
            catch { /* Ignore if it fails due to permissions, DBA handles it */ }
        }

        public List<ConnectionRecord> GetAll(string searchPcno = null)
        {
            string sql = @"
                SELECT c.CONN_ID, c.PCNO, c.CONNECTION_TYPE, c.IP_ADDRESS, c.MAC_ADDRESS, c.PORT_NO, c.SWITCH_NAME, c.ASSIGNED_DATE, c.STATUS,
                       e.NAME, e.DESIGNATION, e.DIVNAME AS DIVISION_NAME
                FROM TRN_CONNECTION_REGISTER c
                LEFT JOIN hrdata.empdetails e ON e.PCNO = c.PCNO
                WHERE 1=1 ";

            if (!string.IsNullOrEmpty(searchPcno))
                sql += " AND c.PCNO = :Search ";

            sql += " ORDER BY c.ASSIGNED_DATE DESC";

            var dt = string.IsNullOrEmpty(searchPcno) 
                ? DbHelper.ExecuteReader(sql) 
                : DbHelper.ExecuteReader(sql, DbHelper.Param("Search", searchPcno));

            var list = new List<ConnectionRecord>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ConnectionRecord
                {
                    ConnId = Convert.ToInt32(row["CONN_ID"]),
                    Pcno = row["PCNO"].ToString(),
                    EmployeeName = row["NAME"] == DBNull.Value ? "" : row["NAME"].ToString(),
                    Designation = row["DESIGNATION"] == DBNull.Value ? "" : row["DESIGNATION"].ToString(),
                    DivisionName = row["DIVISION_NAME"] == DBNull.Value ? "" : row["DIVISION_NAME"].ToString(),
                    ConnectionType = row["CONNECTION_TYPE"].ToString(),
                    IpAddress = row["IP_ADDRESS"] == DBNull.Value ? "" : row["IP_ADDRESS"].ToString(),
                    MacAddress = row["MAC_ADDRESS"] == DBNull.Value ? "" : row["MAC_ADDRESS"].ToString(),
                    PortNo = row["PORT_NO"] == DBNull.Value ? "" : row["PORT_NO"].ToString(),
                    SwitchName = row["SWITCH_NAME"] == DBNull.Value ? "" : row["SWITCH_NAME"].ToString(),
                    AssignedDate = Convert.ToDateTime(row["ASSIGNED_DATE"]),
                    Status = row["STATUS"].ToString()
                });
            }
            return list;
        }

        public void Insert(ConnectionRecord c)
        {
            string sql = @"
                INSERT INTO TRN_CONNECTION_REGISTER 
                (CONN_ID, PCNO, CONNECTION_TYPE, IP_ADDRESS, MAC_ADDRESS, PORT_NO, SWITCH_NAME, STATUS)
                VALUES 
                ((SELECT NVL(MAX(CONN_ID),0)+1 FROM TRN_CONNECTION_REGISTER), :Pcno, :Type, :Ip, :Mac, :Port, :Switch, 'ACTIVE')";

            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("Pcno", c.Pcno),
                DbHelper.Param("Type", c.ConnectionType),
                DbHelper.Param("Ip", c.IpAddress),
                DbHelper.Param("Mac", c.MacAddress),
                DbHelper.Param("Port", c.PortNo),
                DbHelper.Param("Switch", c.SwitchName));
        }

        public void Update(ConnectionRecord c)
        {
            string sql = @"
                UPDATE TRN_CONNECTION_REGISTER 
                SET CONNECTION_TYPE = :Type, IP_ADDRESS = :Ip, MAC_ADDRESS = :Mac, PORT_NO = :Port, SWITCH_NAME = :Switch
                WHERE CONN_ID = :Id";

            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("Type", c.ConnectionType),
                DbHelper.Param("Ip", c.IpAddress),
                DbHelper.Param("Mac", c.MacAddress),
                DbHelper.Param("Port", c.PortNo),
                DbHelper.Param("Switch", c.SwitchName),
                DbHelper.Param("Id", c.ConnId));
        }

        public void SetStatus(int connId, string status)
        {
            string sql = "UPDATE TRN_CONNECTION_REGISTER SET STATUS = :Status WHERE CONN_ID = :Id";
            DbHelper.ExecuteNonQuery(sql, DbHelper.Param("Status", status), DbHelper.Param("Id", connId));
        }
    }
}
