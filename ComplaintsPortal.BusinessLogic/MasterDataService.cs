using System;
using System.Collections.Generic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    /// <summary>
    /// Thin validation wrapper over the four Phase 1 master-data repositories
    /// (Division, Service, RequestType, Role). Each Admin screen calls through
    /// here rather than the repository directly, so basic rules (e.g. no blank
    /// codes, no duplicate codes) live in one place.
    /// </summary>
    public class MasterDataService
    {
        private readonly DivisionRepository _divisionRepo = new DivisionRepository();
        private readonly ServiceRepository _serviceRepo = new ServiceRepository();
        private readonly RequestTypeRepository _requestTypeRepo = new RequestTypeRepository();
        private readonly RoleRepository _roleRepo = new RoleRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        // ---- Division ----
        public List<Division> GetDivisions(bool activeOnly = false) => _divisionRepo.GetAll(activeOnly);

        public string SaveDivision(Division d, string actorPcno, string ip)
        {
            if (string.IsNullOrWhiteSpace(d.DivisionCode) || string.IsNullOrWhiteSpace(d.DivisionName))
                return "Division code and name are required.";

            if (d.DivisionId == 0)
            {
                _divisionRepo.Insert(d);
                _auditRepo.Log(actorPcno, "DIVISION", d.DivisionCode, "CREATE", "Created division " + d.DivisionName, ip);
            }
            else
            {
                _divisionRepo.Update(d);
                _auditRepo.Log(actorPcno, "DIVISION", d.DivisionId.ToString(), "UPDATE", "Updated division " + d.DivisionName, ip);
            }
            return null; // null = success
        }

        public void SetDivisionActive(int id, bool active, string actorPcno, string ip)
        {
            _divisionRepo.SetActive(id, active);
            _auditRepo.Log(actorPcno, "DIVISION", id.ToString(), active ? "ACTIVATE" : "DEACTIVATE", "", ip);
        }

        // ---- Service ----
        public List<ServiceMaster> GetServices(bool activeOnly = false) => _serviceRepo.GetAll(activeOnly);

        public string SaveService(ServiceMaster s, string actorPcno, string ip)
        {
            if (string.IsNullOrWhiteSpace(s.ServiceCode) || string.IsNullOrWhiteSpace(s.ServiceName))
                return "Service code and name are required.";

            if (s.ServiceId == 0)
            {
                _serviceRepo.Insert(s);
                _auditRepo.Log(actorPcno, "SERVICE", s.ServiceCode, "CREATE", "Created service " + s.ServiceName, ip);
            }
            else
            {
                _serviceRepo.Update(s);
                _auditRepo.Log(actorPcno, "SERVICE", s.ServiceId.ToString(), "UPDATE", "Updated service " + s.ServiceName, ip);
            }
            return null;
        }

        public void SetServiceActive(int id, bool active, string actorPcno, string ip)
        {
            _serviceRepo.SetActive(id, active);
            _auditRepo.Log(actorPcno, "SERVICE", id.ToString(), active ? "ACTIVATE" : "DEACTIVATE", "", ip);
        }

        // ---- Request Type ----
        public List<RequestType> GetRequestTypes(bool activeOnly = false) => _requestTypeRepo.GetAll(activeOnly);
        public List<RequestType> GetRequestTypesByService(int serviceId) => _requestTypeRepo.GetByService(serviceId);

        public string SaveRequestType(RequestType rt, string actorPcno, string ip)
        {
            if (rt.ServiceId == 0 || string.IsNullOrWhiteSpace(rt.TypeCode) || string.IsNullOrWhiteSpace(rt.TypeName))
                return "Service, type code and type name are required.";

            if (rt.RequestTypeId == 0)
            {
                _requestTypeRepo.Insert(rt);
                _auditRepo.Log(actorPcno, "REQUEST_TYPE", rt.TypeCode, "CREATE", "Created request type " + rt.TypeName, ip);
            }
            else
            {
                _requestTypeRepo.Update(rt);
                _auditRepo.Log(actorPcno, "REQUEST_TYPE", rt.RequestTypeId.ToString(), "UPDATE", "Updated request type " + rt.TypeName, ip);
            }
            return null;
        }

        public void SetRequestTypeActive(int id, bool active, string actorPcno, string ip)
        {
            _requestTypeRepo.SetActive(id, active);
            _auditRepo.Log(actorPcno, "REQUEST_TYPE", id.ToString(), active ? "ACTIVATE" : "DEACTIVATE", "", ip);
        }

        // ---- Role ----
        public List<Role> GetRoles(bool activeOnly = false) => _roleRepo.GetAll(activeOnly);

        public string SaveRole(Role r, string actorPcno, string ip)
        {
            if (string.IsNullOrWhiteSpace(r.RoleCode) || string.IsNullOrWhiteSpace(r.RoleName) || string.IsNullOrWhiteSpace(r.RoleCategory))
                return "Role code, name and category are required.";

            if (r.RoleId == 0)
            {
                _roleRepo.Insert(r);
                _auditRepo.Log(actorPcno, "ROLE", r.RoleCode, "CREATE", "Created role " + r.RoleName, ip);
            }
            else
            {
                _roleRepo.Update(r);
                _auditRepo.Log(actorPcno, "ROLE", r.RoleId.ToString(), "UPDATE", "Updated role " + r.RoleName, ip);
            }
            return null;
        }

        public void SetRoleActive(int id, bool active, string actorPcno, string ip)
        {
            _roleRepo.SetActive(id, active);
            _auditRepo.Log(actorPcno, "ROLE", id.ToString(), active ? "ACTIVATE" : "DEACTIVATE", "", ip);
        }
    }
}
