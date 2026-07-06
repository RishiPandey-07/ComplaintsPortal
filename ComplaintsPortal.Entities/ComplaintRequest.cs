using System;

namespace ComplaintsPortal.Entities
{
    public class ComplaintRequest
    {
        public int RequestId { get; set; }
        public string RequestNumber { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public int? SubTypeId { get; set; }
        public int? WorkflowId { get; set; }
        public string RequesterPcno { get; set; }
        public string RequesterName { get; set; }
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public int? CurrentStageId { get; set; }
        public string Status { get; set; } // SUBMITTED / IN_PROGRESS / COMPLETED / CLOSED / REJECTED
        public string PickedByPcno { get; set; }
        public string PickedByName { get; set; }
        public DateTime? PickedDate { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        // Phase 1 static fields (replaced by dynamic form values from Phase 3 onward)
        public string RoomNo { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Description { get; set; }
        public string ResolutionRemarks { get; set; }
    }
}
