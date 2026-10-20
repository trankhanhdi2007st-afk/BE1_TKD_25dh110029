using System.Web;
using System.Web.Mvc;

namespace BE1_TKD_25dh110029
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
