using System;
using System.Collections.Generic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class ReportService
    {
        private readonly ReportRepository _repo = new ReportRepository();

        public List<ComplaintRequest> GetRequestsForReport(DateTime? from, DateTime? to, string status, string pcno, bool isAdmin)
        {
            return _repo.GetRequestsForReport(from, to, status, pcno, isAdmin);
        }
    }
}
