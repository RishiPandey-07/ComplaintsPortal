using System;

namespace ComplaintsPortal.Entities
{
    public class DashboardStats
    {
        public int TotalRequests { get; set; }
        public int PendingApprovals { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Rejected { get; set; }
        public int SlaBreached { get; set; }
    }

    public class ChartDataPoint
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
}
