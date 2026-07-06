using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class WorkflowRepository
    {
        /// <summary>The active workflow (with ordered stages) for a request type/sub-type, or null if none is configured.</summary>
        public Workflow GetActiveWorkflow(int requestTypeId, int? subTypeId)
        {
            string sql = @"SELECT WORKFLOW_ID, REQUEST_TYPE_ID, SUBTYPE_ID, WORKFLOW_NAME, VERSION_NO, IS_ACTIVE
                            FROM MST_WORKFLOW
                            WHERE REQUEST_TYPE_ID = :typeId
                              AND (SUBTYPE_ID = :subTypeId OR (:subTypeId IS NULL AND SUBTYPE_ID IS NULL))
                              AND IS_ACTIVE = 'Y'
                              AND ROWNUM = 1
                            ORDER BY VERSION_NO DESC";
            var dt = DbHelper.ExecuteReader(sql,
                DbHelper.Param("typeId", requestTypeId),
                DbHelper.Param("subTypeId", subTypeId));

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            var workflow = new Workflow
            {
                WorkflowId = int.Parse(row["WORKFLOW_ID"].ToString()),
                RequestTypeId = int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                SubTypeId = row["SUBTYPE_ID"] == DBNull.Value ? (int?)null : int.Parse(row["SUBTYPE_ID"].ToString()),
                WorkflowName = row["WORKFLOW_NAME"].ToString(),
                VersionNo = int.Parse(row["VERSION_NO"].ToString()),
                IsActive = row["IS_ACTIVE"].ToString()
            };
            workflow.Stages = GetStages(workflow.WorkflowId);
            return workflow;
        }

        public List<WorkflowStage> GetStages(int workflowId)
        {
            string sql = @"SELECT s.STAGE_ID, s.WORKFLOW_ID, s.STAGE_SEQ, s.STAGE_NAME, s.APPROVER_ROLE_ID, r.ROLE_NAME,
                                   s.REQUIRES_ONLINE_APPROVAL, s.REQUIRES_PRINTOUT, s.PRINT_TEMPLATE_ID,
                                   s.REQUIRES_ASSET_SUBMISSION, s.REQUIRES_ASSET_ACK, s.SLA_HOURS,
                                   s.REJECT_TARGET, s.ASSIGNMENT_MODE, s.IS_ACTIVE
                            FROM MST_WORKFLOW_STAGE s
                            JOIN MST_ROLE r ON r.ROLE_ID = s.APPROVER_ROLE_ID
                            WHERE s.WORKFLOW_ID = :workflowId
                            ORDER BY s.STAGE_SEQ";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("workflowId", workflowId));

            var list = new List<WorkflowStage>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new WorkflowStage
                {
                    StageId = int.Parse(row["STAGE_ID"].ToString()),
                    WorkflowId = int.Parse(row["WORKFLOW_ID"].ToString()),
                    StageSeq = int.Parse(row["STAGE_SEQ"].ToString()),
                    StageName = row["STAGE_NAME"].ToString(),
                    ApproverRoleId = int.Parse(row["APPROVER_ROLE_ID"].ToString()),
                    ApproverRoleName = row["ROLE_NAME"].ToString(),
                    RequiresOnlineApproval = row["REQUIRES_ONLINE_APPROVAL"].ToString(),
                    RequiresPrintout = row["REQUIRES_PRINTOUT"].ToString(),
                    PrintTemplateId = row["PRINT_TEMPLATE_ID"] == DBNull.Value ? (int?)null : int.Parse(row["PRINT_TEMPLATE_ID"].ToString()),
                    RequiresAssetSubmission = row["REQUIRES_ASSET_SUBMISSION"].ToString(),
                    RequiresAssetAck = row["REQUIRES_ASSET_ACK"].ToString(),
                    SlaHours = row["SLA_HOURS"] == DBNull.Value ? (int?)null : int.Parse(row["SLA_HOURS"].ToString()),
                    RejectTarget = row["REJECT_TARGET"].ToString(),
                    AssignmentMode = row["ASSIGNMENT_MODE"].ToString(),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public WorkflowStage GetStageById(int stageId)
        {
            string sql = @"SELECT s.STAGE_ID, s.WORKFLOW_ID, s.STAGE_SEQ, s.STAGE_NAME, s.APPROVER_ROLE_ID, r.ROLE_NAME,
                                   s.REQUIRES_ONLINE_APPROVAL, s.REQUIRES_PRINTOUT, s.PRINT_TEMPLATE_ID,
                                   s.REQUIRES_ASSET_SUBMISSION, s.REQUIRES_ASSET_ACK, s.SLA_HOURS,
                                   s.REJECT_TARGET, s.ASSIGNMENT_MODE, s.IS_ACTIVE
                            FROM MST_WORKFLOW_STAGE s
                            JOIN MST_ROLE r ON r.ROLE_ID = s.APPROVER_ROLE_ID
                            WHERE s.STAGE_ID = :stageId";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("stageId", stageId));
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new WorkflowStage
            {
                StageId = int.Parse(row["STAGE_ID"].ToString()),
                WorkflowId = int.Parse(row["WORKFLOW_ID"].ToString()),
                StageSeq = int.Parse(row["STAGE_SEQ"].ToString()),
                StageName = row["STAGE_NAME"].ToString(),
                ApproverRoleId = int.Parse(row["APPROVER_ROLE_ID"].ToString()),
                ApproverRoleName = row["ROLE_NAME"].ToString(),
                RequiresOnlineApproval = row["REQUIRES_ONLINE_APPROVAL"].ToString(),
                RequiresPrintout = row["REQUIRES_PRINTOUT"].ToString(),
                PrintTemplateId = row["PRINT_TEMPLATE_ID"] == DBNull.Value ? (int?)null : int.Parse(row["PRINT_TEMPLATE_ID"].ToString()),
                RequiresAssetSubmission = row["REQUIRES_ASSET_SUBMISSION"].ToString(),
                RequiresAssetAck = row["REQUIRES_ASSET_ACK"].ToString(),
                SlaHours = row["SLA_HOURS"] == DBNull.Value ? (int?)null : int.Parse(row["SLA_HOURS"].ToString()),
                RejectTarget = row["REJECT_TARGET"].ToString(),
                AssignmentMode = row["ASSIGNMENT_MODE"].ToString(),
                IsActive = row["IS_ACTIVE"].ToString()
            };
        }

        public List<Workflow> GetAllWorkflows()
        {
            string sql = @"SELECT w.WORKFLOW_ID, w.REQUEST_TYPE_ID, rt.TYPE_NAME, w.SUBTYPE_ID, st.SUBTYPE_NAME,
                                   w.WORKFLOW_NAME, w.VERSION_NO, w.IS_ACTIVE
                            FROM MST_WORKFLOW w
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = w.REQUEST_TYPE_ID
                            LEFT JOIN MST_REQUEST_SUBTYPE st ON st.SUBTYPE_ID = w.SUBTYPE_ID
                            ORDER BY rt.TYPE_NAME, w.VERSION_NO DESC";
            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<Workflow>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Workflow
                {
                    WorkflowId = int.Parse(row["WORKFLOW_ID"].ToString()),
                    RequestTypeId = int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                    SubTypeId = row["SUBTYPE_ID"] == DBNull.Value ? (int?)null : int.Parse(row["SUBTYPE_ID"].ToString()),
                    WorkflowName = row["WORKFLOW_NAME"].ToString(),
                    VersionNo = int.Parse(row["VERSION_NO"].ToString()),
                    IsActive = row["IS_ACTIVE"].ToString()
                });
            }
            return list;
        }

        public int InsertWorkflow(Workflow w)
        {
            string sql = @"INSERT INTO MST_WORKFLOW (REQUEST_TYPE_ID, SUBTYPE_ID, WORKFLOW_NAME, VERSION_NO, IS_ACTIVE)
                            VALUES (:typeId, :subTypeId, :name, :version, 'Y')
                            RETURNING WORKFLOW_ID INTO :out_id";
            return DbHelper.ExecuteInsertReturningId(sql, new[]
            {
                DbHelper.Param("typeId", w.RequestTypeId),
                DbHelper.Param("subTypeId", w.SubTypeId),
                DbHelper.Param("name", w.WorkflowName),
                DbHelper.Param("version", w.VersionNo)
            });
        }

        public void SetWorkflowActive(int workflowId, bool active)
        {
            string sql = "UPDATE MST_WORKFLOW SET IS_ACTIVE = :flag WHERE WORKFLOW_ID = :id";
            DbHelper.ExecuteNonQuery(sql, DbHelper.Param("flag", active ? "Y" : "N"), DbHelper.Param("id", workflowId));
        }

        public void InsertStage(WorkflowStage s)
        {
            string sql = @"INSERT INTO MST_WORKFLOW_STAGE
                                (WORKFLOW_ID, STAGE_SEQ, STAGE_NAME, APPROVER_ROLE_ID, REQUIRES_ONLINE_APPROVAL,
                                 REQUIRES_PRINTOUT, REQUIRES_ASSET_SUBMISSION, REQUIRES_ASSET_ACK, SLA_HOURS,
                                 REJECT_TARGET, ASSIGNMENT_MODE, IS_ACTIVE)
                            VALUES
                                (:workflowId, :seq, :name, :roleId, :onlineApproval,
                                 :printout, :assetSub, :assetAck, :slaHours,
                                 :rejectTarget, :assignMode, 'Y')";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("workflowId", s.WorkflowId),
                DbHelper.Param("seq", s.StageSeq),
                DbHelper.Param("name", s.StageName),
                DbHelper.Param("roleId", s.ApproverRoleId),
                DbHelper.Param("onlineApproval", s.RequiresOnlineApproval ?? "Y"),
                DbHelper.Param("printout", s.RequiresPrintout ?? "N"),
                DbHelper.Param("assetSub", s.RequiresAssetSubmission ?? "N"),
                DbHelper.Param("assetAck", s.RequiresAssetAck ?? "N"),
                DbHelper.Param("slaHours", s.SlaHours),
                DbHelper.Param("rejectTarget", s.RejectTarget ?? "EMPLOYEE"),
                DbHelper.Param("assignMode", s.AssignmentMode ?? "SPECIFIC_PERSON"));
        }

        public void DeleteStage(int stageId)
        {
            DbHelper.ExecuteNonQuery("DELETE FROM MST_WORKFLOW_STAGE WHERE STAGE_ID = :id", DbHelper.Param("id", stageId));
        }
    }
}
