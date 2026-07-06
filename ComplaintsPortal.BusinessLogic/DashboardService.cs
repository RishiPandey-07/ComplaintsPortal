using System;
using System.Collections.Generic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class DashboardService
    {
        private readonly DashboardRepository _repo = new DashboardRepository();

        public DashboardStats GetStats(string pcno, bool isAdmin)
        {
            return _repo.GetStats(pcno, isAdmin);
        }

        public List<ChartDataPoint> GetRequestsByStatus(string pcno, bool isAdmin)
        {
            return _repo.GetRequestsByStatus(pcno, isAdmin);
        }

        public List<ChartDataPoint> GetRequestsByType(string pcno, bool isAdmin)
        {
            return _repo.GetRequestsByType(pcno, isAdmin);
        }
    }
}
