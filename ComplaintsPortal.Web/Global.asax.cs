using System;

namespace ComplaintsPortal.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Phase 1: rely on TRN_AUDIT_LOG for user-action logging.
            // Consider adding file-based error logging here (e.g. log4net) once you
            // move past Phase 1 - Server.GetLastError() gives you the unhandled exception.
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}
