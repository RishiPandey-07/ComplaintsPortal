using System;

namespace ComplaintsPortal.Entities
{
    public class FieldDefinition
    {
        public int FieldId { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        
        // Null if it applies to all subtypes of this Request Type
        public int? SubTypeId { get; set; }
        public string SubTypeName { get; set; }

        public string FieldCode { get; set; }
        public string FieldLabel { get; set; }
        public string FieldType { get; set; } // TEXT, NUMBER, DATE, DROPDOWN, MULTILINE
        public string DropdownOptions { get; set; } // Comma separated if FieldType=DROPDOWN
        public string IsMandatory { get; set; } // Y/N
        public int DisplayOrder { get; set; }
        public string IsActive { get; set; }
    }
}
