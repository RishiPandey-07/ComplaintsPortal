using System;

namespace ComplaintsPortal.Entities
{
    public class Division
    {
        public int DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string IsActive { get; set; } // "Y" / "N"
    }
}
