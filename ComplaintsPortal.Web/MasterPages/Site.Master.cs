using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComplaintsPortal.Web.MasterPages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BuildRoleSwitcher();
            }
            BuildMenu();
        }

        private void BuildRoleSwitcher()
        {
            ddlRoleSwitcher.Items.Clear();

            // Build from actual session roles - no hardcoded "Employee" to avoid duplication
            var roles = SessionContext.Roles;
            if (roles == null || roles.Count == 0)
            {
                ddlRoleSwitcher.Items.Add(new ListItem("Employee", "EMPLOYEE"));
                ddlRoleSwitcher.Visible = false;
                return;
            }

            // Add distinct roles from session
            var seen = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in roles)
            {
                string code = (r.RoleCode ?? r.RoleName ?? "EMPLOYEE").ToUpper().Replace(" ", "_");
                string name = r.RoleName ?? "Employee";
                if (seen.Add(code))
                    ddlRoleSwitcher.Items.Add(new ListItem(name, code));
            }

            // Always ensure Employee exists
            if (ddlRoleSwitcher.Items.FindByValue("EMPLOYEE") == null)
                ddlRoleSwitcher.Items.Insert(0, new ListItem("Employee", "EMPLOYEE"));

            // Only show switcher if user has more than 1 role
            ddlRoleSwitcher.Visible = ddlRoleSwitcher.Items.Count > 1;

            // Set the selected value
            string active = SessionContext.ActiveRoleCode ?? "EMPLOYEE";
            var selected = ddlRoleSwitcher.Items.FindByValue(active);
            if (selected != null) ddlRoleSwitcher.SelectedValue = active;
        }

        protected void ddlRoleSwitcher_SelectedIndexChanged(object sender, EventArgs e)
        {
            SessionContext.ActiveRoleCode = ddlRoleSwitcher.SelectedValue;
            Response.Redirect(Request.Url.PathAndQuery);
        }

        private void BuildMenu()
        {
            phMenu.Controls.Clear();

            var roles = SessionContext.Roles;

            // ── Employee section (always visible) ──────────────────────────────
            AddMenuSection("Employee");
            AddMenuLink("bi-plus-circle", "New Request", "~/Employee/NewRequest.aspx");
            AddMenuLink("bi-list-ul", "My Requests", "~/Employee/MyRequests.aspx");

            // ── Approvals: only users with a non-Employee role ─────────────────
            bool hasApproverRole = roles?.Any(r =>
                !string.Equals(r.RoleName, "Employee", StringComparison.OrdinalIgnoreCase)) == true;
            if (hasApproverRole)
            {
                AddMenuSection("Approvals");
                AddMenuLink("bi-check2-circle", "Pending Approvals", "~/Approvals/PendingApprovals.aspx");
            }

            // ── Tech Expert: match role name or code ──────────────────────────
            bool isTechExpert = roles?.Any(r =>
                string.Equals(r.RoleName, "Technical Expert", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.RoleCode, "TECH_EXPERT", StringComparison.OrdinalIgnoreCase)) == true;
            if (isTechExpert)
            {
                AddMenuSection("Tech Expert");
                AddMenuLink("bi-tools", "Complaint Pool", "~/TechExpert/Pool.aspx");
            }

            // ── Admin: Administrator or OIC IT ────────────────────────────────
            bool isAdmin = roles?.Any(r =>
                string.Equals(r.RoleName, "Administrator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.RoleName, "OIC IT", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.RoleCode, "ADMN", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.RoleCode, "OIC_IT", StringComparison.OrdinalIgnoreCase)) == true;
            if (isAdmin)
            {
                AddMenuSection("Administration");
                AddMenuLink("bi-speedometer2", "Dashboard", "~/Admin/Dashboard.aspx");
                AddMenuLink("bi-file-earmark-spreadsheet", "Reports", "~/Admin/Reports.aspx");
                AddMenuLink("bi-diagram-3", "Divisions", "~/Admin/Divisions.aspx");
                AddMenuLink("bi-grid-3x3-gap", "Services", "~/Admin/Services.aspx");
                AddMenuLink("bi-tags", "Request Types", "~/Admin/RequestTypes.aspx");
                AddMenuLink("bi-tag", "Sub-Types", "~/Admin/Subtypes.aspx");
                AddMenuLink("bi-person-badge", "Roles", "~/Admin/Roles.aspx");
                AddMenuSection("User Management");
                AddMenuLink("bi-people", "User Role Assignment", "~/Admin/UserRoleAssignment.aspx");
                AddMenuLink("bi-person-gear", "Tech Expert Assignment", "~/Admin/TechExpertAssignment.aspx");
                AddMenuLink("bi-arrow-left-right", "Standby Assignment", "~/Admin/StandbyAssignment.aspx");
                AddMenuSection("Workflow");
                AddMenuLink("bi-diagram-2", "Workflow Builder", "~/Admin/WorkflowBuilder.aspx");
                AddMenuSection("Logs");
                AddMenuLink("bi-journal-text", "Audit Log", "~/Admin/AuditLog.aspx");
            }
        }

        private void AddMenuSection(string label)
        {
            var div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            div.Attributes["class"] = "menu-section-label";
            div.InnerText = label;
            phMenu.Controls.Add(div);
        }

        private void AddMenuLink(string icon, string text, string url)
        {
            string fullUrl = ResolveUrl(url);
            string currentUrl = Request.Url.AbsolutePath;
            bool isActive = currentUrl.EndsWith(fullUrl.TrimStart('~').TrimStart('/'),
                StringComparison.OrdinalIgnoreCase);

            var li = new System.Web.UI.HtmlControls.HtmlGenericControl("a");
            li.Attributes["href"] = fullUrl;
            li.Attributes["class"] = "sidebar-link" + (isActive ? " active" : "");
            li.InnerHtml = $"<i class='bi {icon}'></i><span>{text}</span>";
            phMenu.Controls.Add(li);
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            SessionContext.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("~/Common/Login.aspx");
        }
    }
}
