using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class RequestRepository
    {
        public int InsertFlowBased(ComplaintRequest r, int subTypeId, int workflowId, int firstStageId)
        {
            return InsertInternal(r, subTypeId == 0 ? (int?)null : subTypeId, workflowId, firstStageId);
        }
        private int InsertInternal(ComplaintRequest r, int? subTypeId, int? workflowId, int? currentStageId)
        {
            string requestNumber = GenerateRequestNumber();
            string status = currentStageId.HasValue ? "IN_PROGRESS" : "SUBMITTED";

            string sql = @"INSERT INTO TRN_REQUEST
                                (REQUEST_NUMBER, REQUEST_TYPE_ID, SUBTYPE_ID, WORKFLOW_ID, REQUESTER_PCNO,
                                 DIVISION_ID, CURRENT_STAGE_ID, STATUS, SUBMITTED_DATE, SLA_DUE_DATE)
                            VALUES
                                (:reqNo, :typeId, :subTypeId, :workflowId, :pcno,
                                 :divId, :stageId, :status, SYSTIMESTAMP,
                                 (SELECT CASE WHEN SLA_HOURS IS NOT NULL THEN SYSTIMESTAMP + NUMTODSINTERVAL(SLA_HOURS, 'HOUR') ELSE NULL END FROM MST_REQUEST_TYPE WHERE REQUEST_TYPE_ID = :typeId))
                            RETURNING REQUEST_ID INTO :out_id";

            int requestId = DbHelper.ExecuteInsertReturningId(sql, new[]
            {
                DbHelper.Param("reqNo", requestNumber),
                DbHelper.Param("typeId", r.RequestTypeId),
                DbHelper.Param("subTypeId", subTypeId),
                DbHelper.Param("workflowId", workflowId),
                DbHelper.Param("pcno", r.RequesterPcno),
                DbHelper.Param("divId", r.DivisionId),
                DbHelper.Param("stageId", currentStageId),
                DbHelper.Param("status", status)
            });

            string updateFields = @"UPDATE TRN_REQUEST
                                     SET ROOM_NO = :room, BUILDING = :building, FLOOR = :floor, DESCRIPTION = :desc
                                     WHERE REQUEST_ID = :id";
            DbHelper.ExecuteNonQuery(updateFields,
                DbHelper.Param("room", r.RoomNo),
                DbHelper.Param("building", r.Building),
                DbHelper.Param("floor", r.Floor),
                DbHelper.Param("desc", r.Description),
                DbHelper.Param("id", requestId));

            return requestId;
        }
        /// <summary>Creates a new request and returns the generated REQUEST_ID.</summary>
        public int Insert(ComplaintRequest r)
        {
            string requestNumber = GenerateRequestNumber();

            string sql = @"INSERT INTO TRN_REQUEST
                                (REQUEST_NUMBER, REQUEST_TYPE_ID, REQUESTER_PCNO, DIVISION_ID, STATUS, SUBMITTED_DATE, SLA_DUE_DATE)
                            VALUES
                                (:reqNo, :typeId, :pcno, :divId, 'SUBMITTED', SYSTIMESTAMP,
                                 (SELECT CASE WHEN SLA_HOURS IS NOT NULL THEN SYSTIMESTAMP + NUMTODSINTERVAL(SLA_HOURS, 'HOUR') ELSE NULL END FROM MST_REQUEST_TYPE WHERE REQUEST_TYPE_ID = :typeId))
                            RETURNING REQUEST_ID INTO :out_id";

            int requestId = DbHelper.ExecuteInsertReturningId(sql, new[]
            {
                DbHelper.Param("reqNo", requestNumber),
                DbHelper.Param("typeId", r.RequestTypeId),
                DbHelper.Param("pcno", r.RequesterPcno),
                DbHelper.Param("divId", r.DivisionId)
            });

            // Phase 1 static fields go into their own simple columns for now.
            // (From Phase 3 onward these move to TRN_REQUEST_FIELD_VALUE via the dynamic form engine.)
            string updateFields = @"UPDATE TRN_REQUEST
                                     SET ROOM_NO = :room, BUILDING = :building, FLOOR = :floor, DESCRIPTION = :desc
                                     WHERE REQUEST_ID = :id";
            // NOTE: ROOM_NO/BUILDING/FLOOR/DESCRIPTION are Phase-1-only convenience columns,
            // not part of the core 27-table schema - add them via a small ALTER if you want
            // to store Phase 1 basic fields directly instead of through TRN_REQUEST_FIELD_VALUE.
            DbHelper.ExecuteNonQuery(updateFields,
                DbHelper.Param("room", r.RoomNo),
                DbHelper.Param("building", r.Building),
                DbHelper.Param("floor", r.Floor),
                DbHelper.Param("desc", r.Description),
                DbHelper.Param("id", requestId));

            return requestId;
        }

        public List<ComplaintRequest> GetMyRequests(string pcno)
        {
            string sql = @"SELECT req.REQUEST_ID, req.REQUEST_NUMBER, req.REQUEST_TYPE_ID, rt.TYPE_NAME,
                                   req.STATUS, req.PICKED_BY_PCNO, e.NAME AS PICKED_NAME, req.PICKED_DATE,
                                   req.SUBMITTED_DATE, req.CLOSED_DATE, req.SLA_DUE_DATE, req.ROOM_NO, req.BUILDING, req.FLOOR,
                                   req.DESCRIPTION, req.RESOLUTION_REMARKS
                            FROM TRN_REQUEST req
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = req.REQUEST_TYPE_ID
                            LEFT JOIN hrdata.empdetails e ON e.PCNO = req.PICKED_BY_PCNO
                            WHERE req.REQUESTER_PCNO = :pcno
                            ORDER BY req.SUBMITTED_DATE DESC";
            return MapList(DbHelper.ExecuteReader(sql, DbHelper.Param("pcno", pcno)));
        }

        /// <summary>Unpicked or self-picked general complaints for the Technical Expert pool screen.</summary>
        public List<ComplaintRequest> GetPoolRequests(List<string> eligiblePcnos, string currentPcno)
        {
            // Pool covers general (non-flow) requests only in Phase 1.
            string sql = @"SELECT req.REQUEST_ID, req.REQUEST_NUMBER, req.REQUEST_TYPE_ID, rt.TYPE_NAME,
                                   req.STATUS, req.PICKED_BY_PCNO, req.PICKED_DATE, req.REQUESTER_PCNO,
                                   e2.NAME AS REQUESTER_NAME, req.SUBMITTED_DATE, req.SLA_DUE_DATE, req.ROOM_NO, req.BUILDING,
                                   req.FLOOR, req.DESCRIPTION
                            FROM TRN_REQUEST req
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = req.REQUEST_TYPE_ID
                            LEFT JOIN hrdata.empdetails e2 ON e2.PCNO = req.REQUESTER_PCNO
                            WHERE rt.IS_FLOW_BASED = 'N'
                              AND req.STATUS IN ('SUBMITTED','IN_PROGRESS')
                              AND (req.PICKED_BY_PCNO IS NULL OR req.PICKED_BY_PCNO = :currentPcno)
                            ORDER BY req.SUBMITTED_DATE";
            return MapList(DbHelper.ExecuteReader(sql, DbHelper.Param("currentPcno", currentPcno)));
        }

        public bool TryPickUp(int requestId, string pcno)
        {
            // Only succeeds if nobody has picked it up yet - prevents two experts grabbing the same ticket.
            string sql = @"UPDATE TRN_REQUEST
                            SET PICKED_BY_PCNO = :pcno, PICKED_DATE = SYSTIMESTAMP, STATUS = 'IN_PROGRESS'
                            WHERE REQUEST_ID = :id AND PICKED_BY_PCNO IS NULL";
            int rows = DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("pcno", pcno),
                DbHelper.Param("id", requestId));
            return rows > 0;
        }

        public void MarkResolved(int requestId, string resolutionRemarks)
        {
            string sql = @"UPDATE TRN_REQUEST
                            SET STATUS = 'COMPLETED', CLOSED_DATE = SYSTIMESTAMP, RESOLUTION_REMARKS = :remarks
                            WHERE REQUEST_ID = :id";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("remarks", resolutionRemarks),
                DbHelper.Param("id", requestId));
        }

        private string GenerateRequestNumber()
        {
            // Simple date-based + sequence-based number, e.g. REQ-2026-000123
            string sql = "SELECT 'REQ-' || TO_CHAR(SYSDATE,'YYYY') || '-' || LPAD(SEQ_REQUEST.NEXTVAL,6,'0') FROM DUAL";
            return DbHelper.ExecuteScalar(sql).ToString();
            // NOTE: this peeks the sequence a value ahead of the row's own trigger-generated ID,
            // which is fine since REQUEST_NUMBER only needs to be unique and human-readable,
            // not equal to REQUEST_ID.
        }
        public void AdvanceToStage(int requestId, int? nextStageId)
        {
            string sql;
            if (nextStageId.HasValue)
            {
                sql = @"UPDATE TRN_REQUEST SET CURRENT_STAGE_ID = :stageId, STATUS = 'IN_PROGRESS' WHERE REQUEST_ID = :id";
                DbHelper.ExecuteNonQuery(sql, DbHelper.Param("stageId", nextStageId), DbHelper.Param("id", requestId));
            }
            else
            {
                sql = @"UPDATE TRN_REQUEST SET CURRENT_STAGE_ID = NULL, STATUS = 'COMPLETED', CLOSED_DATE = SYSTIMESTAMP WHERE REQUEST_ID = :id";
                DbHelper.ExecuteNonQuery(sql, DbHelper.Param("id", requestId));
            }
        }
        public void RejectToStage(int requestId, int? targetStageId)
        {
            string sql;
            if (targetStageId.HasValue)
            {
                sql = @"UPDATE TRN_REQUEST
                        SET CURRENT_STAGE_ID = :stageId, STATUS = 'IN_PROGRESS', REJECTION_COUNT = REJECTION_COUNT + 1
                        WHERE REQUEST_ID = :id";
                DbHelper.ExecuteNonQuery(sql, DbHelper.Param("stageId", targetStageId), DbHelper.Param("id", requestId));
            }
            else
            {
                sql = @"UPDATE TRN_REQUEST
                        SET CURRENT_STAGE_ID = NULL, STATUS = 'REJECTED', CLOSED_DATE = SYSTIMESTAMP, REJECTION_COUNT = REJECTION_COUNT + 1
                        WHERE REQUEST_ID = :id";
                DbHelper.ExecuteNonQuery(sql, DbHelper.Param("id", requestId));
            }
        }
        private List<ComplaintRequest> MapList(DataTable dt)
        {
            var list = new List<ComplaintRequest>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ComplaintRequest
                {
                    RequestId = int.Parse(row["REQUEST_ID"].ToString()),
                    RequestNumber = row["REQUEST_NUMBER"].ToString(),
                    RequestTypeId = int.Parse(row["REQUEST_TYPE_ID"].ToString()),
                    RequestTypeName = row.Table.Columns.Contains("TYPE_NAME") ? row["TYPE_NAME"].ToString() : "",
                    Status = row["STATUS"].ToString(),
                    PickedByPcno = SafeStr(row, "PICKED_BY_PCNO"),
                    PickedByName = SafeStr(row, "PICKED_NAME"),
                    PickedDate = row.Table.Columns.Contains("PICKED_DATE") && row["PICKED_DATE"] != DBNull.Value
                        ? Convert.ToDateTime(row["PICKED_DATE"]) : (DateTime?)null,
                    RequesterPcno = SafeStr(row, "REQUESTER_PCNO"),
                    RequesterName = SafeStr(row, "REQUESTER_NAME"),
                    SubmittedDate = Convert.ToDateTime(row["SUBMITTED_DATE"]),
                    ClosedDate = row.Table.Columns.Contains("CLOSED_DATE") && row["CLOSED_DATE"] != DBNull.Value
                        ? Convert.ToDateTime(row["CLOSED_DATE"]) : (DateTime?)null,
                    SlaDueDate = row.Table.Columns.Contains("SLA_DUE_DATE") && row["SLA_DUE_DATE"] != DBNull.Value
                        ? Convert.ToDateTime(row["SLA_DUE_DATE"]) : (DateTime?)null,
                    RoomNo = SafeStr(row, "ROOM_NO"),
                    Building = SafeStr(row, "BUILDING"),
                    Floor = SafeStr(row, "FLOOR"),
                    Description = SafeStr(row, "DESCRIPTION"),
                    ResolutionRemarks = SafeStr(row, "RESOLUTION_REMARKS")
                });
            }
            return list;
        }
        public ComplaintRequest GetById(int requestId)
        {
            string sql = @"SELECT req.REQUEST_ID, req.REQUEST_NUMBER, req.REQUEST_TYPE_ID, rt.TYPE_NAME,
                                   req.SUBTYPE_ID, req.WORKFLOW_ID, req.CURRENT_STAGE_ID, req.STATUS,
                                   req.REQUESTER_PCNO, e.NAME AS REQUESTER_NAME, req.DIVISION_ID, d.DIVISION_NAME,
                                   req.SUBMITTED_DATE, req.CLOSED_DATE, req.SLA_DUE_DATE, req.ROOM_NO, req.BUILDING, req.FLOOR,
                                   req.DESCRIPTION, req.RESOLUTION_REMARKS
                            FROM TRN_REQUEST req
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = req.REQUEST_TYPE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = req.DIVISION_ID
                            LEFT JOIN hrdata.empdetails e ON e.PCNO = req.REQUESTER_PCNO
                            WHERE req.REQUEST_ID = :id";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("id", requestId));
            var list = MapList(dt);
            if (list.Count == 0) return null;
            list[0].RequestTypeId = int.Parse(dt.Rows[0]["REQUEST_TYPE_ID"].ToString());
            list[0].SubTypeId = dt.Rows[0]["SUBTYPE_ID"] == DBNull.Value ? (int?)null : int.Parse(dt.Rows[0]["SUBTYPE_ID"].ToString());
            list[0].WorkflowId = dt.Rows[0]["WORKFLOW_ID"] == DBNull.Value ? (int?)null : int.Parse(dt.Rows[0]["WORKFLOW_ID"].ToString());
            list[0].CurrentStageId = dt.Rows[0]["CURRENT_STAGE_ID"] == DBNull.Value ? (int?)null : int.Parse(dt.Rows[0]["CURRENT_STAGE_ID"].ToString());
            list[0].DivisionId = int.Parse(dt.Rows[0]["DIVISION_ID"].ToString());
            return list[0];
        }
        public List<PendingApprovalItem> GetSpecificPersonStageRequests()
        {
            string sql = @"SELECT req.REQUEST_ID, req.REQUEST_NUMBER, rt.TYPE_NAME, req.DIVISION_ID, d.DIVISION_NAME,
                                   req.REQUESTER_PCNO, e.NAME AS REQUESTER_NAME, req.SUBMITTED_DATE, req.SLA_DUE_DATE,
                                   req.CURRENT_STAGE_ID, s.STAGE_NAME, s.APPROVER_ROLE_ID, r.ROLE_NAME, r.REQUIRES_DIVISION
                            FROM TRN_REQUEST req
                            JOIN MST_REQUEST_TYPE rt ON rt.REQUEST_TYPE_ID = req.REQUEST_TYPE_ID
                            JOIN MST_WORKFLOW_STAGE s ON s.STAGE_ID = req.CURRENT_STAGE_ID
                            JOIN MST_ROLE r ON r.ROLE_ID = s.APPROVER_ROLE_ID
                            LEFT JOIN MST_DIVISION d ON d.DIVISION_ID = req.DIVISION_ID
                            LEFT JOIN hrdata.empdetails e ON e.PCNO = req.REQUESTER_PCNO
                            WHERE req.STATUS = 'IN_PROGRESS' AND s.ASSIGNMENT_MODE = 'SPECIFIC_PERSON'
                            ORDER BY req.SUBMITTED_DATE";
            var dt = DbHelper.ExecuteReader(sql);
            var list = new List<PendingApprovalItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new PendingApprovalItem
                {
                    RequestId = int.Parse(row["REQUEST_ID"].ToString()),
                    RequestNumber = row["REQUEST_NUMBER"].ToString(),
                    RequestTypeName = row["TYPE_NAME"].ToString(),
                    DivisionId = int.Parse(row["DIVISION_ID"].ToString()),
                    DivisionName = row["DIVISION_NAME"] == DBNull.Value ? "" : row["DIVISION_NAME"].ToString(),
                    RequesterPcno = row["REQUESTER_PCNO"].ToString(),
                    RequesterName = row["REQUESTER_NAME"] == DBNull.Value ? "" : row["REQUESTER_NAME"].ToString(),
                    SubmittedDate = Convert.ToDateTime(row["SUBMITTED_DATE"]),
                    SlaDueDate = row.Table.Columns.Contains("SLA_DUE_DATE") && row["SLA_DUE_DATE"] != DBNull.Value
                        ? Convert.ToDateTime(row["SLA_DUE_DATE"]) : (DateTime?)null,
                    CurrentStageId = int.Parse(row["CURRENT_STAGE_ID"].ToString()),
                    CurrentStageName = row["STAGE_NAME"].ToString(),
                    ApproverRoleId = int.Parse(row["APPROVER_ROLE_ID"].ToString()),
                    ApproverRoleName = row["ROLE_NAME"].ToString(),
                    RequiresDivision = row["REQUIRES_DIVISION"].ToString()
                });
            }
            return list;
        }
        private string SafeStr(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) && row[col] != DBNull.Value ? row[col].ToString() : "";
        }

        public void InsertFieldValues(int requestId, List<RequestFieldValue> fieldValues)
        {
            string sql = @"
                INSERT INTO TRN_REQUEST_FIELD_VALUE 
                (VALUE_ID, REQUEST_ID, FIELD_ID, VALUE_TEXT, VALUE_NUMBER, VALUE_DATE)
                VALUES 
                ((SELECT NVL(MAX(VALUE_ID),0)+1 FROM TRN_REQUEST_FIELD_VALUE), :RequestId, :FieldId, :ValueText, :ValueNumber, :ValueDate)";

            foreach (var fv in fieldValues)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "RequestId", requestId },
                    { "FieldId", fv.FieldId },
                    { "ValueText", string.IsNullOrEmpty(fv.ValueText) ? DBNull.Value : (object)fv.ValueText },
                    { "ValueNumber", fv.ValueNumber.HasValue ? (object)fv.ValueNumber.Value : DBNull.Value },
                    { "ValueDate", fv.ValueDate.HasValue ? (object)fv.ValueDate.Value : DBNull.Value }
                };
                DbHelper.ExecuteNonQuery(sql, parameters);
            }
        }

        public List<RequestFieldValue> GetFieldValues(int requestId)
        {
            string sql = @"
                SELECT v.VALUE_ID, v.REQUEST_ID, v.FIELD_ID, v.VALUE_TEXT, v.VALUE_NUMBER, v.VALUE_DATE,
                       f.FIELD_LABEL, f.FIELD_TYPE
                FROM TRN_REQUEST_FIELD_VALUE v
                JOIN MST_FIELD_DEFINITION f ON f.FIELD_ID = v.FIELD_ID
                WHERE v.REQUEST_ID = :RequestId
                ORDER BY f.DISPLAY_ORDER";
                
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("RequestId", requestId));
            var list = new List<RequestFieldValue>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RequestFieldValue
                {
                    ValueId = Convert.ToInt32(row["VALUE_ID"]),
                    RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                    FieldId = Convert.ToInt32(row["FIELD_ID"]),
                    FieldLabel = row["FIELD_LABEL"].ToString(),
                    FieldType = row["FIELD_TYPE"].ToString(),
                    ValueText = row["VALUE_TEXT"] == DBNull.Value ? null : row["VALUE_TEXT"].ToString(),
                    ValueNumber = row["VALUE_NUMBER"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["VALUE_NUMBER"]),
                    ValueDate = row["VALUE_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["VALUE_DATE"])
                });
            }
            return list;
        }

        public void InsertAttachment(int requestId, string pcno, string fileName, string filePath)
        {
            string sql = @"
                INSERT INTO TRN_REQUEST_ATTACHMENT
                (ATTACHMENT_ID, REQUEST_ID, UPLOADED_BY_PCNO, FILE_NAME, FILE_PATH, UPLOADED_DATE)
                VALUES
                ((SELECT NVL(MAX(ATTACHMENT_ID),0)+1 FROM TRN_REQUEST_ATTACHMENT), :reqId, :pcno, :fname, :fpath, SYSTIMESTAMP)";
            
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("reqId", requestId),
                DbHelper.Param("pcno", pcno),
                DbHelper.Param("fname", fileName),
                DbHelper.Param("fpath", filePath));
        }

        public string[] GetAttachment(int requestId)
        {
            // Returns [FileName, FilePath] or null
            string sql = @"SELECT FILE_NAME, FILE_PATH FROM TRN_REQUEST_ATTACHMENT WHERE REQUEST_ID = :reqId ORDER BY ATTACHMENT_ID DESC";
            var dt = DbHelper.ExecuteReader(sql, DbHelper.Param("reqId", requestId));
            if (dt.Rows.Count > 0)
            {
                return new string[] { dt.Rows[0]["FILE_NAME"].ToString(), dt.Rows[0]["FILE_PATH"].ToString() };
            }
            return null;
        }
    }
}
