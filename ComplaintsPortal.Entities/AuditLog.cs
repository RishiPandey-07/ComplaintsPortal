using System;

namespace ComplaintsPortal.Entities
{
    public class AuditLog
    {
        public long AuditId { get; set; }
        public string Pcno { get; set; }
        public string ModuleName { get; set; }
        public string EntityId { get; set; }
        public string ActionType { get; set; }
        public string ActionDesc { get; set; }
        public string IpAddress { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
