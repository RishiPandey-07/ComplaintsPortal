using System;

namespace ComplaintsPortal.Entities
{
    public class ConnectionRecord
    {
        public int ConnId { get; set; }
        public string Pcno { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string DivisionName { get; set; }
        
        public string ConnectionType { get; set; } // INTERNET, INTRANET, PRINTER, etc
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string PortNo { get; set; }
        public string SwitchName { get; set; }
        
        public DateTime AssignedDate { get; set; }
        public string Status { get; set; } // ACTIVE, INACTIVE
    }
}
