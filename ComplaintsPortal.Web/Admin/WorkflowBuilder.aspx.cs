using System;
using System.Linq;
using ComplaintsPortal.BusinessLogic;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web.Admin
{
    public partial class WorkflowBuilder : BasePage
    {
        private readonly WorkflowAdminService _workflowAdminService = new WorkflowAdminService();
        private readonly MasterDataService _masterDataService = new MasterDataService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRequestTypeDropdown();
                BindSubTypeDropdown();
                BindApproverRoleDropdown();
                BindExistingWorkflowDropdown();
            }
        }

        private void BindRequestTypeDropdown()
        {
            ddlRequestType.DataSource = _masterDataService.GetRequestTypes(activeOnly: true);
            ddlRequestType.DataBind();
        }

        private void BindSubTypeDropdown()
        {
            ddlSubType.Items.Clear();
            ddlSubType.Items.Add(new System.Web.UI.WebControls.ListItem("(none - applies to whole request type)", ""));
            if (ddlRequestType.Items.Count == 0) return;

            var subTypes = _workflowAdminService.GetSubTypesByRequestType(int.Parse(ddlRequestType.SelectedValue));
            foreach (var st in subTypes)
                ddlSubType.Items.Add(new System.Web.UI.WebControls.ListItem(st.SubTypeName, st.SubTypeId.ToString()));
        }

        private void BindApproverRoleDropdown()
        {
            ddlApproverRole.DataSource = _masterDataService.GetRoles(activeOnly: true);
            ddlApproverRole.DataBind();
        }

        private void BindExistingWorkflowDropdown()
        {
            ddlExistingWorkflow.DataSource = _workflowAdminService.GetAllWorkflows();
            ddlExistingWorkflow.DataBind();
        }

        protected void btnCreateWorkflow_Click(object sender, EventArgs e)
        {
            var w = new Workflow
            {
                RequestTypeId = int.Parse(ddlRequestType.SelectedValue),
                SubTypeId = string.IsNullOrEmpty(ddlSubType.SelectedValue) ? (int?)null : int.Parse(ddlSubType.SelectedValue),
                WorkflowName = txtWorkflowName.Text.Trim()
            };
            int newWorkflowId;
            string error = _workflowAdminService.CreateWorkflow(w, CurrentPcno, CurrentIp, out newWorkflowId);
            if (error != null) { lblCreateMessage.Text = error; return; }

            lblCreateMessage.Text = "";
            txtWorkflowName.Text = "";
            BindExistingWorkflowDropdown();
            ddlExistingWorkflow.SelectedValue = newWorkflowId.ToString();
            LoadStages();
        }

        protected void ddlExistingWorkflow_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStages();
        }

        private void LoadStages()
        {
            if (ddlExistingWorkflow.Items.Count == 0)
            {
                pnlStages.Visible = false;
                return;
            }
            pnlStages.Visible = true;
            int workflowId = int.Parse(ddlExistingWorkflow.SelectedValue);
            gvStages.DataSource = _workflowAdminService.GetStages(workflowId);
            gvStages.DataBind();
        }

        protected void btnAddStage_Click(object sender, EventArgs e)
        {
            int seq;
            if (!int.TryParse(txtStageSeq.Text.Trim(), out seq))
            {
                lblStageMessage.Text = "Sequence must be a number.";
                return;
            }

            int? slaHours = null;
            int parsedSla;
            if (int.TryParse(txtSlaHours.Text.Trim(), out parsedSla)) slaHours = parsedSla;

            var stage = new WorkflowStage
            {
                WorkflowId = int.Parse(ddlExistingWorkflow.SelectedValue),
                StageSeq = seq,
                StageName = txtStageName.Text.Trim(),
                ApproverRoleId = int.Parse(ddlApproverRole.SelectedValue),
                AssignmentMode = ddlAssignmentMode.SelectedValue,
                RejectTarget = ddlRejectTarget.SelectedValue,
                RequiresPrintout = chkRequiresPrintout.Checked ? "Y" : "N",
                RequiresAssetSubmission = chkRequiresAssetSubmission.Checked ? "Y" : "N",
                RequiresAssetAck = chkRequiresAssetAck.Checked ? "Y" : "N",
                RequiresOnlineApproval = "Y",
                SlaHours = slaHours
            };

            string error = _workflowAdminService.AddStage(stage, CurrentPcno, CurrentIp);
            if (error != null) { lblStageMessage.Text = error; return; }

            lblStageMessage.Text = "";
            txtStageSeq.Text = "";
            txtStageName.Text = "";
            txtSlaHours.Text = "";
            chkRequiresPrintout.Checked = false;
            chkRequiresAssetSubmission.Checked = false;
            chkRequiresAssetAck.Checked = false;
            LoadStages();
        }

        protected void gvStages_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int stageId = int.Parse(e.CommandArgument.ToString());
                _workflowAdminService.RemoveStage(stageId, CurrentPcno, CurrentIp);
                LoadStages();
            }
        }
    }
}
