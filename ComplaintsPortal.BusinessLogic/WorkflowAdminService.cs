using System.Collections.Generic;
using System.Linq;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class WorkflowAdminService
    {
        private readonly RequestSubTypeRepository _subTypeRepo = new RequestSubTypeRepository();
        private readonly WorkflowRepository _workflowRepo = new WorkflowRepository();
        private readonly StandbyRepository _standbyRepo = new StandbyRepository();
        private readonly UserRoleRepository _userRoleRepo = new UserRoleRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        // ---- Sub-types ----
        public List<RequestSubType> GetSubTypes() => _subTypeRepo.GetAll();
        public List<RequestSubType> GetSubTypesByRequestType(int requestTypeId) => _subTypeRepo.GetByRequestType(requestTypeId);

        public string SaveSubType(RequestSubType st, string actorPcno, string ip)
        {
            if (st.RequestTypeId == 0 || string.IsNullOrWhiteSpace(st.SubTypeCode) || string.IsNullOrWhiteSpace(st.SubTypeName))
                return "Request type, sub-type code and name are required.";

            if (st.SubTypeId == 0)
            {
                _subTypeRepo.Insert(st);
                _auditRepo.Log(actorPcno, "SUBTYPE", st.SubTypeCode, "CREATE", "Created sub-type " + st.SubTypeName, ip);
            }
            else
            {
                _subTypeRepo.Update(st);
                _auditRepo.Log(actorPcno, "SUBTYPE", st.SubTypeId.ToString(), "UPDATE", "Updated sub-type " + st.SubTypeName, ip);
            }
            return null;
        }

        public void SetSubTypeActive(int id, bool active, string actorPcno, string ip)
        {
            _subTypeRepo.SetActive(id, active);
            _auditRepo.Log(actorPcno, "SUBTYPE", id.ToString(), active ? "ACTIVATE" : "DEACTIVATE", "", ip);
        }

        // ---- Workflows ----
        public List<Workflow> GetAllWorkflows() => _workflowRepo.GetAllWorkflows();

        public List<WorkflowStage> GetStages(int workflowId) => _workflowRepo.GetStages(workflowId);

        public string CreateWorkflow(Workflow w, string actorPcno, string ip, out int newWorkflowId)
        {
            newWorkflowId = 0;
            if (w.RequestTypeId == 0 || string.IsNullOrWhiteSpace(w.WorkflowName))
                return "Request type and workflow name are required.";

            // Increment version number
            var existing = _workflowRepo.GetAllWorkflows()
                .Where(x => x.RequestTypeId == w.RequestTypeId && x.SubTypeId == w.SubTypeId)
                .OrderByDescending(x => x.VersionNo)
                .FirstOrDefault();

            w.VersionNo = existing != null ? existing.VersionNo + 1 : 1;

            newWorkflowId = _workflowRepo.InsertWorkflow(w);
            _auditRepo.Log(actorPcno, "WORKFLOW", newWorkflowId.ToString(), "CREATE", "Created workflow " + w.WorkflowName, ip);
            return null;
        }

        public void DeactivateWorkflow(int workflowId, string actorPcno, string ip)
        {
            _workflowRepo.SetWorkflowActive(workflowId, false);
            _auditRepo.Log(actorPcno, "WORKFLOW", workflowId.ToString(), "DEACTIVATE", "", ip);
        }

        public string AddStage(WorkflowStage s, string actorPcno, string ip)
        {
            if (s.WorkflowId == 0 || s.ApproverRoleId == 0 || string.IsNullOrWhiteSpace(s.StageName))
                return "Workflow, stage name and approver role are required.";

            _workflowRepo.InsertStage(s);
            _auditRepo.Log(actorPcno, "WORKFLOW_STAGE", s.WorkflowId.ToString(), "CREATE", "Added stage " + s.StageName, ip);
            return null;
        }

        public void RemoveStage(int stageId, string actorPcno, string ip)
        {
            _workflowRepo.DeleteStage(stageId);
            _auditRepo.Log(actorPcno, "WORKFLOW_STAGE", stageId.ToString(), "DELETE", "", ip);
        }

        // ---- Standby ----
        public List<StandbyAssignment> GetStandbyAssignments() => _standbyRepo.GetAll();

        public string AssignStandby(int userRoleId, string standbyPcno, string actorPcno, string ip)
        {
            if (userRoleId == 0 || string.IsNullOrWhiteSpace(standbyPcno))
                return "Primary role assignment and standby employee are required.";

            var primary = _userRoleRepo.GetById(userRoleId);
            if (primary != null && primary.Pcno == standbyPcno)
                return "The standby cannot be the same person as the primary role holder.";

            _standbyRepo.Insert(userRoleId, standbyPcno);
            _auditRepo.Log(actorPcno, "STANDBY", userRoleId.ToString(), "ASSIGN", "Assigned standby " + standbyPcno, ip);
            return null;
        }

        public void EndStandby(int standbyId, string actorPcno, string ip)
        {
            _standbyRepo.EndStandby(standbyId);
            _auditRepo.Log(actorPcno, "STANDBY", standbyId.ToString(), "END", "", ip);
        }
    }
}
