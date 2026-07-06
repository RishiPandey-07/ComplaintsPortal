using System;
using System.Web.Security;
using System.Web.UI;

namespace ComplaintsPortal.Web
{
    /// <summary>Every .aspx.cs code-behind should inherit from this instead of System.Web.UI.Page directly.</summary>
    public class BasePage : Page
    {
        protected string CurrentPcno => SessionContext.Pcno;
        protected string CurrentIp => Request.UserHostAddress;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!SessionContext.IsLoggedIn && !(this is Common.Login))
            {
                // Covers the case where the Forms Auth cookie is still valid (e.g. after
                // an app pool recycle) but the in-memory Session has been lost - clear the
                // stale cookie too, so the person gets a clean login instead of a loop.
                FormsAuthentication.SignOut();
                Response.Redirect("~/Common/Login.aspx");
            }
        }

        /// <summary>
        /// Shows a toast notification on the client side.
        /// Type can be: success, error, warning, info
        /// </summary>
        protected void ShowToast(string type, string message)
        {
            // Escape single quotes for JS
            string safeMsg = message.Replace("'", "\\'").Replace("\n", " ").Replace("\r", "");
            string script = $"showToast('{type}', '{safeMsg}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ToastMsg", script, true);
        }
    }
}
