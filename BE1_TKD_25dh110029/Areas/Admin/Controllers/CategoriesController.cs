using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BE1_TKD_25dh110029.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Admin/Categories
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _AdminLayout()
        {
            return View();
        }
    }
}