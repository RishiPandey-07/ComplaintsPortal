using System;
using System.Collections.Generic;
using System.Linq;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class NotificationService
    {
        private readonly EmailService _emailService = new EmailService();
        private readonly EmployeeRepository _empRepo = new EmployeeRepository();
        private readonly UserRoleRepository _userRoleRepo = new UserRoleRepository();

        public void NotifyRequesterOnSubmit(ComplaintRequest req)
        {
            string email = _empRepo.GetEmailByPcno(req.RequesterPcno);
            if (string.IsNullOrEmpty(email)) return;

            string subject = $"Request Submitted Successfully: {req.RequestNumber}";
            string body = $@"
                <h3>Your request has been submitted</h3>
                <p><strong>Request No:</strong> {req.RequestNumber}</p>
                <p><strong>Type:</strong> {req.RequestTypeName}</p>
                <p><strong>Description:</strong> {req.Description}</p>
                <br/>
                <p>You can track the progress of your request in the Complaints Portal under 'My Requests'.</p>";

            _emailService.SendEmailAsync(new EmailMessage { To = email, Subject = subject, Body = body });
        }

        public void NotifyApproverOnPending(ComplaintRequest req, string approverPcno)
        {
            string email = _empRepo.GetEmailByPcno(approverPcno);
            if (string.IsNullOrEmpty(email)) return;

            string subject = $"Action Required: Pending Approval for {req.RequestNumber}";
            string body = $@"
                <h3>A request is pending your approval</h3>
                <p><strong>Request No:</strong> {req.RequestNumber}</p>
                <p><strong>Type:</strong> {req.RequestTypeName}</p>
                <p><strong>Raised By:</strong> {req.RequesterName} ({req.RequesterPcno})</p>
                <p><strong>Description:</strong> {req.Description}</p>
                <br/>
                <p>Please log in to the Complaints Portal and check your 'Pending Approvals' to review and take action on this request.</p>";

            _emailService.SendEmailAsync(new EmailMessage { To = email, Subject = subject, Body = body });
        }

        public void NotifyTechExpertOnPool(ComplaintRequest req)
        {
            // For Tech Experts, we might want to notify all experts mapped to this service/request type.
            // This is a simplified version. A full implementation would query TRN_TECH_EXPERT_SERVICE.
            // For now, if we have a pool, we can either not spam everyone or send to a group DL.
            // Let's assume we want to fetch tech experts for this RequestTypeId.
            var expertRepo = new TechExpertRepository();
            var allMappings = expertRepo.GetAll(true);
            var relevantMappings = allMappings.Where(x => 
                x.ServiceId == req.RequestTypeId // Assuming Service mapping logic or request type matching
                || x.RequestTypeId == req.RequestTypeId).ToList();

            var emails = new HashSet<string>();
            foreach (var mapping in relevantMappings)
            {
                string em = _empRepo.GetEmailByPcno(mapping.Pcno);
                if (!string.IsNullOrEmpty(em)) emails.Add(em);
            }

            if (emails.Count == 0) return;

            string subject = $"New Request in Pool: {req.RequestNumber}";
            string body = $@"
                <h3>A new request has arrived in the Technical Expert Pool</h3>
                <p><strong>Request No:</strong> {req.RequestNumber}</p>
                <p><strong>Type:</strong> {req.RequestTypeName}</p>
                <p><strong>Description:</strong> {req.Description}</p>
                <br/>
                <p>Please log in to the Complaints Portal and check your 'Complaint Pool' to pick up this request.</p>";

            string toAddresses = string.Join(",", emails);
            _emailService.SendEmailAsync(new EmailMessage { To = toAddresses, Subject = subject, Body = body });
        }

        public void NotifyRequesterOnResolve(ComplaintRequest req)
        {
            string email = _empRepo.GetEmailByPcno(req.RequesterPcno);
            if (string.IsNullOrEmpty(email)) return;

            string subject = $"Request Resolved: {req.RequestNumber}";
            string body = $@"
                <h3>Your request has been marked as resolved</h3>
                <p><strong>Request No:</strong> {req.RequestNumber}</p>
                <p><strong>Type:</strong> {req.RequestTypeName}</p>
                <p><strong>Resolution Remarks:</strong> {req.ResolutionRemarks}</p>
                <br/>
                <p>Please log in to the Complaints Portal to view the details.</p>";

            _emailService.SendEmailAsync(new EmailMessage { To = email, Subject = subject, Body = body });
        }

        public void NotifyRequesterOnReject(ComplaintRequest req, string rejectRemarks)
        {
            string email = _empRepo.GetEmailByPcno(req.RequesterPcno);
            if (string.IsNullOrEmpty(email)) return;

            string subject = $"Request Rejected: {req.RequestNumber}";
            string body = $@"
                <h3>Your request has been rejected</h3>
                <p><strong>Request No:</strong> {req.RequestNumber}</p>
                <p><strong>Type:</strong> {req.RequestTypeName}</p>
                <p><strong>Rejection Remarks:</strong> {rejectRemarks}</p>
                <br/>
                <p>Please log in to the Complaints Portal to view the details or submit a new request if needed.</p>";

            _emailService.SendEmailAsync(new EmailMessage { To = email, Subject = subject, Body = body });
        }
    }
}
