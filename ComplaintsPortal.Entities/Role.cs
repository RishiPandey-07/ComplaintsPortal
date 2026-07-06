namespace ComplaintsPortal.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string RoleCategory { get; set; } // EMPLOYEE / DIVISION / AD / ESTABLISHMENT / IT
        public string RequiresDivision { get; set; } // "Y" / "N"
        public string IsActive { get; set; }
    }
}
