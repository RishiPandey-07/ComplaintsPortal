using System.Collections.Generic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class UserRoleService
    {
        private readonly UserRoleRepository _userRoleRepo = new UserRoleRepository();
        private readonly TechExpertRepository _techExpertRepo = new TechExpertRepository();
        private readonly RoleRepository _roleRepo = new RoleRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        public List<UserRole> GetAssignments(string pcnoFilter) => _userRoleRepo.GetAllForAdminGrid(pcnoFilter);

        public string AssignRole(UserRole ur, string actorPcno, string ip)
        {
            if (string.IsNullOrWhiteSpace(ur.Pcno) || ur.RoleId == 0)
                return "Employee and role are required.";

            var role = _roleRepo.GetAll().Find(r => r.RoleId == ur.RoleId);
            if (role != null && role.RequiresDivision == "Y" && ur.DivisionId == null)
                return "This role requires a division to be selected.";

            _userRoleRepo.Assign(ur, actorPcno);
            _auditRepo.Log(actorPcno, "USER_ROLE", ur.Pcno, "ASSIGN_ROLE",
                "Assigned role " + ur.RoleId + " to " + ur.Pcno, ip);
            return null;
        }

        public void EndAssignment(int userRoleId, string actorPcno, string ip)
        {
            _userRoleRepo.EndAssignment(userRoleId);
            _auditRepo.Log(actorPcno, "USER_ROLE", userRoleId.ToString(), "END_ASSIGNMENT", "", ip);
        }

        public List<TechExpertMapping> GetTechExpertAssignments() => _techExpertRepo.GetAll();

        public string AssignTechExpert(TechExpertMapping m, string actorPcno, string ip)
        {
            if (string.IsNullOrWhiteSpace(m.Pcno) || m.ServiceId == 0)
                return "Employee and service are required.";

            m.AssignedBy = actorPcno;
            _techExpertRepo.Assign(m);
            _auditRepo.Log(actorPcno, "TECH_EXPERT", m.Pcno, "ASSIGN", "Assigned technical expert " + m.Pcno, ip);
            return null;
        }

        public void DeactivateTechExpert(int mappingId, string actorPcno, string ip)
        {
            _techExpertRepo.SetActive(mappingId, false);
            _auditRepo.Log(actorPcno, "TECH_EXPERT", mappingId.ToString(), "DEACTIVATE", "", ip);
        }
    }
}
