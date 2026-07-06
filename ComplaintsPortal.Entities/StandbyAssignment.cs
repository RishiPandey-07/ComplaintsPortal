using System;

namespace ComplaintsPortal.Entities
{
    public class StandbyAssignment
    {
        public int StandbyId { get; set; }
        public int UserRoleId { get; set; }
        public string PrimaryPcno { get; set; } // whose role is being covered
        public string PrimaryName { get; set; }
        public string RoleName { get; set; }
        public string DivisionName { get; set; }
        public string StandbyPcno { get; set; }
        public string StandbyName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string IsActive { get; set; }
    }
}
