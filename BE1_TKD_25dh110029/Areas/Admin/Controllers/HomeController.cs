
using System.Linq;
using System.Web.Mvc;
using BE1_TKD_25dh110029.Models;
using BE1_TKD_25dh110029.Models.ViewModel;
using PagedList;

namespace BE1_TKD_25dh110029.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
//http://localhost:51182/Models/MyStoreModel.Context.cs
            // Tìm kiếm sản phẩm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;

                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm) ||
                    p.Category.CategoryName.Contains(searchTerm));
            }


            // Thiết lập phân trang
            int pageNumber = page ?? 1;
            int pageSize = 6;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            // Lấy sản phẩm nổi bật
            model.Products = products
                .OrderByDescending(p => p.OrderDetails.Count())
                .Take(10)
                .ToList();

            // Lấy sản phẩm mới và phân trang
            model.NewProducts = products
                .OrderByDescending(p => p.ProductID)
                .ToPagedList(1, int.MaxValue);

            return View(model);
        }

        public ActionResult ProductDetail(int id)
        {
            var product = db.Products
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View("ProductDetail", product);
        }


    }
}
