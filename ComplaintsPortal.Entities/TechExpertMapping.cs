using System;

namespace ComplaintsPortal.Entities
{
    public class TechExpertMapping
    {
        public int MappingId { get; set; }
        public string Pcno { get; set; }
        public string EmployeeName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int? RequestTypeId { get; set; } // null = all types under the service
        public string RequestTypeName { get; set; }
        public string AssignedBy { get; set; }
        public DateTime AssignedDate { get; set; }
        public string IsActive { get; set; }
    }
}
