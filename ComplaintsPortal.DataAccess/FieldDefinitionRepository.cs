using System;
using System.Collections.Generic;
using System.Data;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.DataAccess
{
    public class FieldDefinitionRepository
    {
        public List<FieldDefinition> GetFieldsByRequestType(int requestTypeId, int? subTypeId = null)
        {
            var list = new List<FieldDefinition>();
            string sql = @"
                SELECT f.FIELD_ID, f.REQUEST_TYPE_ID, f.SUBTYPE_ID, f.FIELD_CODE, f.FIELD_LABEL, 
                       f.FIELD_TYPE, f.DROPDOWN_OPTIONS, f.IS_MANDATORY, f.DISPLAY_ORDER, f.IS_ACTIVE,
                       rt.TYPE_NAME, st.SUBTYPE_NAME
                FROM MST_FIELD_DEFINITION f
                JOIN MST_REQUEST_TYPE rt ON f.REQUEST_TYPE_ID = rt.REQUEST_TYPE_ID
                LEFT JOIN MST_REQUEST_SUBTYPE st ON f.SUBTYPE_ID = st.SUBTYPE_ID
                WHERE f.REQUEST_TYPE_ID = :ReqTypeId";

            if (subTypeId.HasValue)
                sql += " AND (f.SUBTYPE_ID = :SubTypeId OR f.SUBTYPE_ID IS NULL)";

            sql += " ORDER BY f.DISPLAY_ORDER";

            var parameters = new Dictionary<string, object>
            {
                { "ReqTypeId", requestTypeId }
            };
            if (subTypeId.HasValue)
            {
                parameters.Add("SubTypeId", subTypeId.Value);
            }

            using (var reader = DbHelper.ExecuteReader(sql, parameters))
            {
                while (reader.Read())
                {
                    list.Add(new FieldDefinition
                    {
                        FieldId = Convert.ToInt32(reader["FIELD_ID"]),
                        RequestTypeId = Convert.ToInt32(reader["REQUEST_TYPE_ID"]),
                        SubTypeId = reader["SUBTYPE_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SUBTYPE_ID"]),
                        FieldCode = reader["FIELD_CODE"].ToString(),
                        FieldLabel = reader["FIELD_LABEL"].ToString(),
                        FieldType = reader["FIELD_TYPE"].ToString(),
                        DropdownOptions = reader["DROPDOWN_OPTIONS"] == DBNull.Value ? null : reader["DROPDOWN_OPTIONS"].ToString(),
                        IsMandatory = reader["IS_MANDATORY"].ToString(),
                        DisplayOrder = Convert.ToInt32(reader["DISPLAY_ORDER"]),
                        IsActive = reader["IS_ACTIVE"].ToString(),
                        RequestTypeName = reader["TYPE_NAME"].ToString(),
                        SubTypeName = reader["SUBTYPE_NAME"] == DBNull.Value ? null : reader["SUBTYPE_NAME"].ToString()
                    });
                }
            }
            return list;
        }

        public void InsertField(FieldDefinition f)
        {
            string sql = @"
                INSERT INTO MST_FIELD_DEFINITION 
                (FIELD_ID, REQUEST_TYPE_ID, SUBTYPE_ID, FIELD_CODE, FIELD_LABEL, FIELD_TYPE, DROPDOWN_OPTIONS, IS_MANDATORY, DISPLAY_ORDER, IS_ACTIVE)
                VALUES 
                ((SELECT NVL(MAX(FIELD_ID),0)+1 FROM MST_FIELD_DEFINITION), 
                 :ReqTypeId, :SubTypeId, :Code, :Label, :Type, :Opts, :Mandatory, :DispOrder, 'Y')";

            var parameters = new Dictionary<string, object>
            {
                { "ReqTypeId", f.RequestTypeId },
                { "SubTypeId", f.SubTypeId.HasValue ? (object)f.SubTypeId.Value : DBNull.Value },
                { "Code", f.FieldCode },
                { "Label", f.FieldLabel },
                { "Type", f.FieldType },
                { "Opts", string.IsNullOrEmpty(f.DropdownOptions) ? DBNull.Value : (object)f.DropdownOptions },
                { "Mandatory", f.IsMandatory },
                { "DispOrder", f.DisplayOrder }
            };

            DbHelper.ExecuteNonQuery(sql, parameters);
        }

        public void UpdateField(FieldDefinition f)
        {
            string sql = @"
                UPDATE MST_FIELD_DEFINITION 
                SET FIELD_LABEL = :Label, FIELD_TYPE = :Type, DROPDOWN_OPTIONS = :Opts, 
                    IS_MANDATORY = :Mandatory, DISPLAY_ORDER = :DispOrder
                WHERE FIELD_ID = :Id";

            var parameters = new Dictionary<string, object>
            {
                { "Label", f.FieldLabel },
                { "Type", f.FieldType },
                { "Opts", string.IsNullOrEmpty(f.DropdownOptions) ? DBNull.Value : (object)f.DropdownOptions },
                { "Mandatory", f.IsMandatory },
                { "DispOrder", f.DisplayOrder },
                { "Id", f.FieldId }
            };

            DbHelper.ExecuteNonQuery(sql, parameters);
        }

        public void ToggleActive(int id, string active)
        {
            string sql = "UPDATE MST_FIELD_DEFINITION SET IS_ACTIVE = :Active WHERE FIELD_ID = :Id";
            var parameters = new Dictionary<string, object>
            {
                { "Active", active },
                { "Id", id }
            };
            DbHelper.ExecuteNonQuery(sql, parameters);
        }
    }
}
