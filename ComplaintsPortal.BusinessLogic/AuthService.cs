using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Pcno { get; set; }
        public string EmployeeName { get; set; }
        public string Message { get; set; }
        public List<UserRole> Roles { get; set; }
    }

    public class AuthService
    {
        private readonly EmployeeRepository _employeeRepo = new EmployeeRepository();
        private readonly UserRoleRepository _userRoleRepo = new UserRoleRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        /// <summary>
        /// Call this after IIS/Windows Authentication has already validated the AD credentials.
        /// samAccountName typically arrives as "DOMAIN\username" from HttpContext.User.Identity.Name -
        /// strip the domain prefix before calling this.
        /// </summary>
        public AuthResult ResolveLogin(string samAccountName, string ipAddress)
        {
            var result = new AuthResult { Roles = new List<UserRole>() };

            // NOTE: adjust this if PCNO is not literally equal to SAMAccountName in your
            // hrdata.empdetails - e.g. if it needs upper-casing or a prefix stripped.
            string pcno = samAccountName;

            DataRow emp = _employeeRepo.GetByPcno(pcno);
            if (emp == null)
            {
                result.Success = false;
                result.Message = "No active employee record found for this login. Contact Establishment/IT Admin.";
                return result;
            }

            result.Success = true;
            result.Pcno = pcno;
            result.EmployeeName = emp["NAME"].ToString();
            result.Roles = _userRoleRepo.GetActiveRolesForUser(pcno);

            _auditRepo.Log(pcno, "AUTH", pcno, "LOGIN", "User logged in", ipAddress);
            return result;
        }
    }
}
