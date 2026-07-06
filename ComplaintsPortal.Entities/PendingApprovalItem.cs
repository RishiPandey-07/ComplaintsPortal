using System;

namespace ComplaintsPortal.Entities
{
    public class PendingApprovalItem
    {
        public int RequestId { get; set; }
        public string RequestNumber { get; set; }
        public string RequestTypeName { get; set; }
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public string RequesterPcno { get; set; }
        public string RequesterName { get; set; }
        public DateTime SubmittedDate { get; set; }
        public int CurrentStageId { get; set; }
        public string CurrentStageName { get; set; }
        public int ApproverRoleId { get; set; }
        public string ApproverRoleName { get; set; }
        public string RequiresDivision { get; set; }
        public bool IsStandbyItem { get; set; }
        public int? StandbyForUserRoleId { get; set; }
    }
}
