using System.Collections.Generic;
using System.Linq;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class WorkflowEngineService
    {
        private readonly WorkflowRepository _workflowRepo = new WorkflowRepository();
        private readonly RequestRepository _requestRepo = new RequestRepository();
        private readonly RequestStageHistoryRepository _historyRepo = new RequestStageHistoryRepository();
        private readonly UserRoleRepository _userRoleRepo = new UserRoleRepository();
        private readonly StandbyRepository _standbyRepo = new StandbyRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();
        private readonly NotificationService _notificationService = new NotificationService();

        /// <summary>Used by the New Request screen's live flow preview.</summary>
        public Workflow GetActiveWorkflow(int requestTypeId, int? subTypeId)
        {
            return _workflowRepo.GetActiveWorkflow(requestTypeId, subTypeId);
        }

        /// <summary>Submits a flow-based request, positioning it at the workflow's first stage.
        /// Falls back to a general (no-flow) submission if no active workflow is configured yet.</summary>
        public string SubmitRequest(ComplaintRequest r, int? subTypeId, string ip, List<RequestFieldValue> fieldValues, out string requestNumber)
        {
            requestNumber = null;
            if (r.RequestTypeId == 0 || string.IsNullOrWhiteSpace(r.RequesterPcno) || r.DivisionId == 0)
                return "Request type, requester and division are required.";
            if (string.IsNullOrWhiteSpace(r.Description))
                return "Please describe the issue/request.";

            var workflow = GetActiveWorkflow(r.RequestTypeId, subTypeId);
            int requestId;

            if (workflow != null && workflow.Stages.Count > 0)
            {
                var firstStage = workflow.Stages.OrderBy(s => s.StageSeq).First();
                requestId = _requestRepo.InsertFlowBased(r, subTypeId ?? 0, workflow.WorkflowId, firstStage.StageId);
                _historyRepo.Insert(requestId, firstStage.StageId, r.RequesterPcno, false, null, "FORWARDED", "Submitted by requester", firstStage.StageId);
            }
            else
            {
                requestId = _requestRepo.Insert(r);
            }

            if (fieldValues != null && fieldValues.Count > 0)
            {
                _requestRepo.InsertFieldValues(requestId, fieldValues);
            }

            _auditRepo.Log(r.RequesterPcno, "REQUEST", requestId.ToString(), "SUBMIT", "Submitted new request", ip);

            var mine = _requestRepo.GetMyRequests(r.RequesterPcno);
            var justCreated = mine.FirstOrDefault(x => x.RequestId == requestId);
            requestNumber = justCreated != null ? justCreated.RequestNumber : "";

            // Notify Requester
            if (justCreated != null)
            {
                _notificationService.NotifyRequesterOnSubmit(justCreated);
            }

            return null;
        }

        /// <summary>Pending approvals for this person, covering both their own direct role
        /// assignments and anything they cover as a standby.</summary>
        public List<PendingApprovalItem> GetPendingApprovals(string pcno)
        {
            var directRoles = _userRoleRepo.GetActiveRolesForUser(pcno);
            var standbyUserRoleIds = _standbyRepo.GetActiveStandbyUserRoleIds(pcno);
            var standbyRoles = standbyUserRoleIds
                .Select(id => _userRoleRepo.GetById(id))
                .Where(ur => ur != null)
                .ToList();

            var allCandidates = _requestRepo.GetSpecificPersonStageRequests();
            var result = new List<PendingApprovalItem>();

            foreach (var item in allCandidates)
            {
                bool requiresDivision = item.RequiresDivision == "Y";

                bool matchesDirect = directRoles.Any(ur =>
                    ur.RoleId == item.ApproverRoleId &&
                    (!requiresDivision || ur.DivisionId == item.DivisionId));

                var standbyMatch = standbyRoles.FirstOrDefault(ur =>
                    ur.RoleId == item.ApproverRoleId &&
                    (!requiresDivision || ur.DivisionId == item.DivisionId));

                if (matchesDirect)
                {
                    item.IsStandbyItem = false;
                    result.Add(item);
                }
                else if (standbyMatch != null)
                {
                    item.IsStandbyItem = true;
                    item.StandbyForUserRoleId = standbyMatch.UserRoleId;
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>Approves the current stage and moves the request forward (or completes it if this was the last stage).</summary>
        public string Approve(int requestId, string remarks, string actorPcno, bool actedAsStandby, int? standbyForUserRoleId, string ip)
        {
            var request = _requestRepo.GetById(requestId);
            if (request == null || !request.CurrentStageId.HasValue) return "Request not found or not currently awaiting approval.";

            var currentStage = _workflowRepo.GetStageById(request.CurrentStageId.Value);
            var allStages = _workflowRepo.GetStages(currentStage.WorkflowId).OrderBy(s => s.StageSeq).ToList();
            var nextStage = allStages.FirstOrDefault(s => s.StageSeq == currentStage.StageSeq + 1);

            _requestRepo.AdvanceToStage(requestId, nextStage?.StageId);
            _historyRepo.Insert(requestId, currentStage.StageId, actorPcno, actedAsStandby, standbyForUserRoleId,
                nextStage != null ? "FORWARDED" : "COMPLETED", remarks, nextStage?.StageId);

            _auditRepo.Log(actorPcno, "REQUEST", requestId.ToString(), "APPROVE", remarks, ip);

            // Fetch the updated request to notify the next approver or tech expert pool
            var updatedRequest = _requestRepo.GetById(requestId);
            if (nextStage == null) 
            {
                // Completed workflow, now in tech pool
                _notificationService.NotifyTechExpertOnPool(updatedRequest);
            }
            // (Note: To accurately notify the next specific approver requires evaluating who is in the next stage role/division. 
            // In a production app, we'd query users mapped to nextStage.ApproverRoleId and updatedRequest.DivisionId.
            // For simplicity in this demo, if there is a next stage, the workflow continues.)

            return null;
        }

        /// <summary>Rejects the current stage - sends back to the previous stage or to the employee, per that stage's REJECT_TARGET.</summary>
        public string Reject(int requestId, string remarks, string actorPcno, bool actedAsStandby, int? standbyForUserRoleId, string ip)
        {
            if (string.IsNullOrWhiteSpace(remarks)) return "A remark is required when rejecting a request.";

            var request = _requestRepo.GetById(requestId);
            if (request == null || !request.CurrentStageId.HasValue) return "Request not found or not currently awaiting approval.";

            var currentStage = _workflowRepo.GetStageById(request.CurrentStageId.Value);
            WorkflowStage targetStage = null;

            if (currentStage.RejectTarget == "PREVIOUS_STAGE")
            {
                var allStages = _workflowRepo.GetStages(currentStage.WorkflowId).OrderBy(s => s.StageSeq).ToList();
                targetStage = allStages.FirstOrDefault(s => s.StageSeq == currentStage.StageSeq - 1);
                // If there's no previous stage (this was the first stage after submission),
                // fall back to sending it back to the employee instead.
            }

            _requestRepo.RejectToStage(requestId, targetStage?.StageId);
            _historyRepo.Insert(requestId, currentStage.StageId, actorPcno, actedAsStandby, standbyForUserRoleId,
                "REJECTED", remarks, targetStage?.StageId);

            _auditRepo.Log(actorPcno, "REQUEST", requestId.ToString(), "REJECT", remarks, ip);

            // Notify Requester
            var updatedRequest = _requestRepo.GetById(requestId);
            if (targetStage == null)
            {
                _notificationService.NotifyRequesterOnReject(updatedRequest, remarks);
            }

            return null;
        }

        /// <summary>Builds the full timeline for a request: completed history steps + remaining pending stages.</summary>
        public List<TimelineStep> GetTimeline(int requestId)
        {
            var request = _requestRepo.GetById(requestId);
            var history = _historyRepo.GetForRequest(requestId);
            var steps = new List<TimelineStep>();

            steps.Add(new TimelineStep
            {
                StageName = "Submitted",
                Status = "COMPLETED",
                ActedByName = request.RequesterName,
                ActionDate = request.SubmittedDate
            });

            if (!request.WorkflowId.HasValue)
            {
                steps.Add(new TimelineStep
                {
                    StageName = "Technical Expert pool",
                    Status = request.Status == "COMPLETED" ? "COMPLETED" : "CURRENT"
                });
                return steps;
            }

            var allStages = _workflowRepo.GetStages(request.WorkflowId.Value).OrderBy(s => s.StageSeq).ToList();
            var completedStageIds = history.Where(h => h.Action == "FORWARDED" || h.Action == "COMPLETED").Select(h => h.StageId).ToList();

            foreach (var stage in allStages)
            {
                var histEntry = history.LastOrDefault(h => h.StageId == stage.StageId);
                bool isCurrent = request.CurrentStageId == stage.StageId && request.Status == "IN_PROGRESS";
                bool isDone = histEntry != null && (histEntry.Action == "FORWARDED" || histEntry.Action == "COMPLETED");

                steps.Add(new TimelineStep
                {
                    StageName = stage.StageName,
                    Status = isDone ? "COMPLETED" : (isCurrent ? "CURRENT" : "PENDING"),
                    ActedByName = histEntry?.ActionByName,
                    ActionDate = histEntry?.ActionDate,
                    Remarks = histEntry?.Remarks,
                    Action = histEntry?.Action
                });

                // Stop listing further stages once we hit one that was rejected back past this point.
                if (histEntry != null && histEntry.Action == "REJECTED") break;
            }

            return steps;
        }
    }
}
