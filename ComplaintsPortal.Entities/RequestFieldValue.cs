using System;

namespace ComplaintsPortal.Entities
{
    public class RequestFieldValue
    {
        public int ValueId { get; set; }
        public int RequestId { get; set; }
        public int FieldId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldType { get; set; }
        public string ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
    }
}
