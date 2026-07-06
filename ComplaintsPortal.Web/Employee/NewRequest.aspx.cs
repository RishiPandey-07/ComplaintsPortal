using System;
using System.Linq;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.DataAccess;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Employee
{
    public partial class NewRequest : BasePage
    {
        private readonly MasterDataService _masterDataService = new MasterDataService();
        private readonly WorkflowAdminService _workflowAdminService = new WorkflowAdminService();
        private readonly WorkflowEngineService _workflowEngineService = new WorkflowEngineService();
        private readonly EmployeeRepository _employeeRepo = new EmployeeRepository();
        private readonly FieldDefinitionService _fieldService = new FieldDefinitionService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindServiceDropdown();
                BindRequestTypeDropdown();
                BindSubTypeDropdown();
                UpdateFlowPreview();
            }
        }

        private void BindServiceDropdown()
        {
            ddlService.DataSource = _masterDataService.GetServices(activeOnly: true);
            ddlService.DataBind();
        }

        private void BindRequestTypeDropdown()
        {
            if (ddlService.Items.Count == 0) return;
            int serviceId = int.Parse(ddlService.SelectedValue);
            ddlRequestType.DataSource = _masterDataService.GetRequestTypesByService(serviceId);
            ddlRequestType.DataBind();
        }

        private void BindSubTypeDropdown()
        {
            divSubType.Visible = false;
            ddlSubType.Items.Clear();
            if (ddlRequestType.Items.Count == 0) return;

            int requestTypeId = int.Parse(ddlRequestType.SelectedValue);
            var subTypes = _workflowAdminService.GetSubTypesByRequestType(requestTypeId);
            if (subTypes.Count == 0) return;

            divSubType.Visible = true;
            foreach (var st in subTypes)
                ddlSubType.Items.Add(new System.Web.UI.WebControls.ListItem(st.SubTypeName, st.SubTypeId.ToString()));
        }

        protected void ddlService_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRequestTypeDropdown();
            BindSubTypeDropdown();
            UpdateFlowPreview();
            BindCustomFields();
        }

        protected void ddlRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubTypeDropdown();
            UpdateFlowPreview();
            BindCustomFields();
        }

        protected void ddlSubType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFlowPreview();
            BindCustomFields();
        }

        private void BindCustomFields()
        {
            if (ddlRequestType.Items.Count == 0) return;
            
            int requestTypeId = int.Parse(ddlRequestType.SelectedValue);
            var fields = _fieldService.GetFieldsByRequestType(requestTypeId, SelectedSubTypeId)
                                      .Where(f => f.IsActive == "Y").ToList();
            
            rptCustomFields.DataSource = fields;
            rptCustomFields.DataBind();
        }

        protected void rptCustomFields_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                var field = (FieldDefinition)e.Item.DataItem;
                
                var txtSingle = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtSingle");
                var txtMulti = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtMulti");
                var txtNumber = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtNumber");
                var txtDate = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtDate");
                var ddlDropdown = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlDropdown");

                switch (field.FieldType)
                {
                    case "TEXT": txtSingle.Visible = true; break;
                    case "MULTILINE": txtMulti.Visible = true; break;
                    case "NUMBER": txtNumber.Visible = true; break;
                    case "DATE": txtDate.Visible = true; break;
                    case "DROPDOWN":
                        ddlDropdown.Visible = true;
                        if (!string.IsNullOrEmpty(field.DropdownOptions))
                        {
                            ddlDropdown.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select --", ""));
                            foreach (var opt in field.DropdownOptions.Split(','))
                                ddlDropdown.Items.Add(new System.Web.UI.WebControls.ListItem(opt.Trim(), opt.Trim()));
                        }
                        break;
                }
            }
        }

        private int? SelectedSubTypeId => divSubType.Visible && ddlSubType.Items.Count > 0
            ? int.Parse(ddlSubType.SelectedValue)
            : (int?)null;

        /// <summary>Reads the real workflow (if configured) for the selected type/sub-type and
        /// renders it as a connected badge trail, matching the design agreed on earlier.</summary>
        private void UpdateFlowPreview()
        {
            if (ddlRequestType.Items.Count == 0)
            {
                litFlowPreview.Text = "";
                return;
            }

            int requestTypeId = int.Parse(ddlRequestType.SelectedValue);
            var workflow = _workflowEngineService.GetActiveWorkflow(requestTypeId, SelectedSubTypeId);

            if (workflow == null || workflow.Stages.Count == 0)
            {
                litFlowPreview.Text = "<div class='flow-step pool'><i class='bi bi-tools'></i> Technical Expert pool</div> <span class='ms-2 text-muted' style='font-size:11px;'>(no approval needed)</span>";
                return;
            }

            var stageNames = workflow.Stages.OrderBy(s => s.StageSeq).Select(s => s.ApproverRoleName);
            string trail = "<div class='flow-step you'><i class='bi bi-person-fill'></i> You</div>";
            foreach (var name in stageNames)
                trail += " <i class='bi bi-chevron-right flow-arrow'></i> <div class='flow-step'>" + name + "</div>";

            litFlowPreview.Text = trail;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var empRow = _employeeRepo.GetByPcno(CurrentPcno);
            if (empRow == null)
            {
                lblMessage.Text = "Could not resolve your employee record. Contact Admin.";
                return;
            }

            // Resolve DIVNAME (string) from empdetails → DIVISION_ID in MST_DIVISION
            string divName = empRow["DIVNAME"].ToString();
            int divisionId = new ComplaintsPortal.DataAccess.DivisionRepository().GetIdByName(divName);
            if (divisionId == 0)
            {
                lblMessage.Text = $"Your division '{divName}' is not configured in the portal yet. Contact Admin.";
                return;
            }

            var request = new ComplaintRequest
            {
                RequestTypeId = int.Parse(ddlRequestType.SelectedValue),
                RequesterPcno = CurrentPcno,
                DivisionId = divisionId,
                RoomNo = txtRoomNo.Text.Trim(),
                Building = txtBuilding.Text.Trim(),
                Floor = txtFloor.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            string requestNumber;
            
            // Gather dynamic fields
            var fieldValues = new System.Collections.Generic.List<RequestFieldValue>();
            foreach (System.Web.UI.WebControls.RepeaterItem item in rptCustomFields.Items)
            {
                var hfId = (System.Web.UI.WebControls.HiddenField)item.FindControl("hfFieldId");
                var hfType = (System.Web.UI.WebControls.HiddenField)item.FindControl("hfFieldType");
                var hfMan = (System.Web.UI.WebControls.HiddenField)item.FindControl("hfIsMandatory");
                
                string type = hfType.Value;
                string rawVal = "";
                
                if (type == "TEXT") rawVal = ((System.Web.UI.WebControls.TextBox)item.FindControl("txtSingle")).Text;
                else if (type == "MULTILINE") rawVal = ((System.Web.UI.WebControls.TextBox)item.FindControl("txtMulti")).Text;
                else if (type == "NUMBER") rawVal = ((System.Web.UI.WebControls.TextBox)item.FindControl("txtNumber")).Text;
                else if (type == "DATE") rawVal = ((System.Web.UI.WebControls.TextBox)item.FindControl("txtDate")).Text;
                else if (type == "DROPDOWN") rawVal = ((System.Web.UI.WebControls.DropDownList)item.FindControl("ddlDropdown")).SelectedValue;

                if (hfMan.Value == "Y" && string.IsNullOrWhiteSpace(rawVal))
                {
                    lblMessage.Text = "Please fill all mandatory custom fields.";
                    return;
                }
                
                if (!string.IsNullOrWhiteSpace(rawVal))
                {
                    var fv = new RequestFieldValue { FieldId = int.Parse(hfId.Value), FieldType = type };
                    if (type == "NUMBER") fv.ValueNumber = decimal.Parse(rawVal);
                    else if (type == "DATE") fv.ValueDate = DateTime.Parse(rawVal);
                    else fv.ValueText = rawVal.Trim();
                    fieldValues.Add(fv);
                }
            }

            string error = _workflowEngineService.SubmitRequest(request, SelectedSubTypeId, CurrentIp, fieldValues, out requestNumber);
            if (error != null)
            {
                lblMessage.Text = error;
                return;
            }

            lblMessage.Text = "";
            ShowToast("success", "Request submitted successfully. Ref No: " + requestNumber);
            ClearForm();
        }

        private void ClearForm()
        {
            txtRoomNo.Text = "";
            txtBuilding.Text = "";
            txtFloor.Text = "";
            txtDescription.Text = "";
            BindCustomFields(); // Resets dynamic fields
        }
    }
}
