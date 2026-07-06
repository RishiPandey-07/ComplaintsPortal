using System.Collections.Generic;

namespace ComplaintsPortal.Entities
{
    public class Workflow
    {
        public int WorkflowId { get; set; }
        public int RequestTypeId { get; set; }
        public int? SubTypeId { get; set; }
        public string WorkflowName { get; set; }
        public int VersionNo { get; set; }
        public string IsActive { get; set; }
        public List<WorkflowStage> Stages { get; set; } = new List<WorkflowStage>();
    }

    public class WorkflowStage
    {
        public int StageId { get; set; }
        public int WorkflowId { get; set; }
        public int StageSeq { get; set; }
        public string StageName { get; set; }
        public int ApproverRoleId { get; set; }
        public string ApproverRoleName { get; set; }
        public string RequiresOnlineApproval { get; set; }
        public string RequiresPrintout { get; set; }
        public int? PrintTemplateId { get; set; }
        public string RequiresAssetSubmission { get; set; }
        public string RequiresAssetAck { get; set; }
        public int? SlaHours { get; set; }
        public string RejectTarget { get; set; } // PREVIOUS_STAGE / EMPLOYEE
        public string AssignmentMode { get; set; } // SPECIFIC_PERSON / POOL
        public string IsActive { get; set; }
    }
}
