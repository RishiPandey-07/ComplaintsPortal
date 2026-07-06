using System;

namespace ComplaintsPortal.Entities
{
    public class RequestStageHistory
    {
        public long HistoryId { get; set; }
        public int RequestId { get; set; }
        public int StageId { get; set; }
        public string StageName { get; set; }
        public string ActionByPcno { get; set; }
        public string ActionByName { get; set; }
        public string ActedAsStandby { get; set; }
        public int? StandbyForUserRoleId { get; set; }
        public string Action { get; set; } // APPROVED / REJECTED / FORWARDED / COMPLETED
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }
        public int? NextStageId { get; set; }
    }

    /// <summary>One row in the combined timeline view - either a completed history entry or an upcoming pending stage.</summary>
    public class TimelineStep
    {
        public string StageName { get; set; }
        public string Status { get; set; } // COMPLETED / CURRENT / PENDING
        public string ActedByName { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Remarks { get; set; }
        public string Action { get; set; }
    }
}
