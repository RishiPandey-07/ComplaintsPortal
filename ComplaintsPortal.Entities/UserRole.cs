using System;

namespace ComplaintsPortal.Entities
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public string Pcno { get; set; }
        public string EmployeeName { get; set; } // populated from hrdata.empdetails on read
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int? DivisionId { get; set; }
        public string DivisionName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
