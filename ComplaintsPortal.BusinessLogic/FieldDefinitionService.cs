using System.Collections.Generic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class FieldDefinitionService
    {
        private readonly FieldDefinitionRepository _fieldRepo = new FieldDefinitionRepository();
        private readonly AuditRepository _auditRepo = new AuditRepository();

        public List<FieldDefinition> GetFieldsByRequestType(int requestTypeId, int? subTypeId = null)
        {
            return _fieldRepo.GetFieldsByRequestType(requestTypeId, subTypeId);
        }

        public string SaveField(FieldDefinition f, string actorPcno, string ip)
        {
            if (f.RequestTypeId == 0) return "Request Type is required.";
            if (string.IsNullOrWhiteSpace(f.FieldCode)) return "Field Code is required.";
            if (string.IsNullOrWhiteSpace(f.FieldLabel)) return "Field Label is required.";
            if (string.IsNullOrWhiteSpace(f.FieldType)) return "Field Type is required.";

            if (f.FieldId == 0)
            {
                _fieldRepo.InsertField(f);
                _auditRepo.Log(actorPcno, "FIELD_DEF", f.FieldCode, "CREATE", "Created custom field " + f.FieldCode, ip);
            }
            else
            {
                _fieldRepo.UpdateField(f);
                _auditRepo.Log(actorPcno, "FIELD_DEF", f.FieldId.ToString(), "UPDATE", "Updated custom field " + f.FieldCode, ip);
            }
            return null;
        }

        public void SetFieldActive(int id, bool active, string actorPcno, string ip)
        {
            string status = active ? "Y" : "N";
            _fieldRepo.ToggleActive(id, status);
            _auditRepo.Log(actorPcno, "FIELD_DEF", id.ToString(), "STATUS_CHANGE", "Set field active=" + status, ip);
        }
    }
}
