
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BE1_TKD_25dh110029
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string area = filterContext.RouteData.DataTokens["area"] as string;

            if (!string.Equals(area, "Admin", StringComparison.OrdinalIgnoreCase))
                return;

            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["Username"] == null)
                return false;

            string username = httpContext.Session["Username"].ToString();

            using (var db = new Models.MyStoreEntities())
            {
                var user = db.Users.FirstOrDefault(
                    u => u.Username == username);

                return user != null
                    && user.UserRole != null
                    && user.UserRole.Trim() == "A";
            }
        }


        protected override void HandleUnauthorizedRequest(
            AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["Username"] == null)
            {
                filterContext.Result = new RedirectResult(
                    "/Account/Login");
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(403);
            }
        }
    }
}
