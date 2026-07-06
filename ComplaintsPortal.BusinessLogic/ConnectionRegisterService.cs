using System.Collections.Generic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class ConnectionRegisterService
    {
        private readonly ConnectionRegisterRepository _connRepo = new ConnectionRegisterRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        public ConnectionRegisterService()
        {
            _connRepo.EnsureTableExists();
        }

        public List<ConnectionRecord> GetAll(string searchPcno = null)
        {
            return _connRepo.GetAll(searchPcno);
        }

        public string SaveConnection(ConnectionRecord c, string actorPcno, string ip)
        {
            if (string.IsNullOrWhiteSpace(c.Pcno)) return "PC NO is required.";
            if (string.IsNullOrWhiteSpace(c.ConnectionType)) return "Connection Type is required.";

            if (c.ConnId == 0)
            {
                _connRepo.Insert(c);
                _auditRepo.Log(actorPcno, "CONNECTION", c.Pcno, "CREATE", "Created " + c.ConnectionType + " connection", ip);
            }
            else
            {
                _connRepo.Update(c);
                _auditRepo.Log(actorPcno, "CONNECTION", c.ConnId.ToString(), "UPDATE", "Updated " + c.ConnectionType + " connection", ip);
            }
            return null;
        }

        public void SetStatus(int connId, bool active, string actorPcno, string ip)
        {
            string status = active ? "ACTIVE" : "INACTIVE";
            _connRepo.SetStatus(connId, status);
            _auditRepo.Log(actorPcno, "CONNECTION", connId.ToString(), "STATUS_CHANGE", "Set connection to " + status, ip);
        }
    }
}
