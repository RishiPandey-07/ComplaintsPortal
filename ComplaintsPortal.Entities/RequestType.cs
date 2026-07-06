namespace ComplaintsPortal.Entities
{
    public class RequestType
    {
        public int RequestTypeId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } // populated on read (joined) for display
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string IsFlowBased { get; set; }
        public int? SlaHours { get; set; }
        public string IsActive { get; set; }
    }
}
