using System.Collections.Generic;
using System.Web;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.Web
{
    /// <summary>Central place for reading/writing session state - avoids scattering Session["..."] keys across pages.</summary>
    public static class SessionContext
    {
        public static string Pcno
        {
            get { return HttpContext.Current.Session["Pcno"] as string; }
            set { HttpContext.Current.Session["Pcno"] = value; }
        }

        public static string EmployeeName
        {
            get { return HttpContext.Current.Session["EmployeeName"] as string; }
            set { HttpContext.Current.Session["EmployeeName"] = value; }
        }

        public static List<UserRole> Roles
        {
            get { return HttpContext.Current.Session["Roles"] as List<UserRole>; }
            set { HttpContext.Current.Session["Roles"] = value; }
        }

        /// <summary>The role category currently selected in the header switcher, e.g. "GD", "Employee".</summary>
        public static string ActiveRoleCode
        {
            get { return HttpContext.Current.Session["ActiveRoleCode"] as string ?? "EMPLOYEE"; }
            set { HttpContext.Current.Session["ActiveRoleCode"] = value; }
        }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(Pcno);

        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}
