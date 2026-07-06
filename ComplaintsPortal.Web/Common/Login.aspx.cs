using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Web.Security;
using System.Web.UI;
using ComplaintsPortal.BusinessLogic;
using System.DirectoryServices;

namespace ComplaintsPortal.Web.Common
{
    public partial class Login : Page
    {
        private readonly AuthService _authService = new AuthService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please enter both username and password.";
                pnlError.Visible = true;
                return;
            }

            string employeeId = ValidateAndGetEmployeeId(username, password);
            if (string.IsNullOrEmpty(employeeId))
            {
                lblMessage.Text = "Invalid username or password.";
                pnlError.Visible = true;
                return;
            }

            var result = _authService.ResolveLogin(employeeId, Request.UserHostAddress);

            if (!result.Success)
            {
                lblMessage.Text = result.Message;
                pnlError.Visible = true;
                return;
            }

            // Tell ASP.NET this request is now authenticated, so the site-wide
            // <deny users="?"/> rule in Web.config lets subsequent requests through.
            FormsAuthentication.SetAuthCookie(result.Pcno, false);

            SessionContext.Pcno = result.Pcno;
            SessionContext.EmployeeName = result.EmployeeName;
            SessionContext.Roles = result.Roles;
            SessionContext.ActiveRoleCode = "EMPLOYEE"; // default landing role

            Response.Redirect("~/Employee/MyRequests.aspx");
        }
        private string ValidateAndGetEmployeeId(string username, string password)
        {
            string initLDAPPath = ConfigurationManager.AppSettings["AdLdapPath"];
            string initLDAPServer = ConfigurationManager.AppSettings["AdDomain"];
            string strCommu = "LDAP://" + initLDAPServer + "/" + initLDAPPath;

            DirectoryEntry entry = new DirectoryEntry(strCommu, username, password);
            string employeeId = null;

            try
            {
                object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("EmployeeID");

                SearchResult result = search.FindOne();
                if (result != null)
                {
                    DirectoryEntry dsresult = result.GetDirectoryEntry();
                    employeeId = dsresult.Properties["EmployeeID"][0].ToString();
                }
            }
            catch (Exception)
            {
                employeeId = null;
            }

            return employeeId;
        }

        /// <summary>
        /// Validates the typed credentials directly against AD/LDAP.
        /// AdDomain (e.g. "yourdomain.local") must be set in Web.config appSettings.
        /// </summary>
       
    }
}
