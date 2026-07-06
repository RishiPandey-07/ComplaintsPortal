namespace ComplaintsPortal.Entities
{
    public class RequestSubType
    {
        public int SubTypeId { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public string SubTypeCode { get; set; }
        public string SubTypeName { get; set; }
        public string IsActive { get; set; }
    }
}
