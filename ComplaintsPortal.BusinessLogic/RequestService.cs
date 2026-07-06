using System.Collections.Generic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class RequestService
    {
        private readonly RequestRepository _requestRepo = new RequestRepository();
        private readonly TechExpertRepository _techExpertRepo = new TechExpertRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        public string SubmitRequest(ComplaintRequest r, string ip, out string requestNumber)
        {
            requestNumber = null;
            if (r.RequestTypeId == 0 || string.IsNullOrWhiteSpace(r.RequesterPcno) || r.DivisionId == 0)
                return "Request type, requester and division are required.";

            if (string.IsNullOrWhiteSpace(r.Description))
                return "Please describe the issue/request.";

            int id = _requestRepo.Insert(r);
            _auditRepo.Log(r.RequesterPcno, "REQUEST", id.ToString(), "SUBMIT", "Submitted new request", ip);

            // Re-fetch to get the generated REQUEST_NUMBER for confirmation display
            var mine = _requestRepo.GetMyRequests(r.RequesterPcno);
            var justCreated = mine.Find(x => x.RequestId == id);
            requestNumber = justCreated != null ? justCreated.RequestNumber : "";
            return null;
        }

        public List<ComplaintRequest> GetMyRequests(string pcno) => _requestRepo.GetMyRequests(pcno);

        public ComplaintRequest GetById(int requestId) => _requestRepo.GetById(requestId);

        /// <summary>
        /// Pool list for a technical expert: shows general (non-flow) requests for services/types
        /// this expert is assigned to, that are unpicked or already picked by them.
        /// </summary>
        public List<ComplaintRequest> GetPoolForExpert(string expertPcno)
        {
            return _requestRepo.GetPoolRequests(null, expertPcno);
        }

        public string PickUp(int requestId, string expertPcno, string ip)
        {
            bool success = _requestRepo.TryPickUp(requestId, expertPcno);
            if (!success)
                return "Someone else has already picked up this request.";

            _auditRepo.Log(expertPcno, "REQUEST", requestId.ToString(), "PICK_UP", "Picked up from pool", ip);
            return null;
        }

        public void MarkResolved(int requestId, string resolutionRemarks, string expertPcno, string ip)
        {
            _requestRepo.MarkResolved(requestId, resolutionRemarks);
            _auditRepo.Log(expertPcno, "REQUEST", requestId.ToString(), "RESOLVE", resolutionRemarks, ip);
        }
    }
}
