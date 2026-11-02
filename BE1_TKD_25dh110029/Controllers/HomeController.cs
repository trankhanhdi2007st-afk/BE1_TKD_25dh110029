using System.Linq;
using System.Web.Mvc;
using BE1_TKD_25dh110029.Models;
using BE1_TKD_25dh110029.Models.ViewModel;
using PagedList;

namespace BE1_TKD_25dh110029.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;

                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm) ||
                    p.Category.CategoryName.Contains(searchTerm));
            }

            int pageNumber = page ?? 1;
            int pageSize = 6;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.Products = products
                .OrderByDescending(p => p.OrderDetails.Count())
                .Take(10)
                .ToList();

            model.NewProducts = products
                .OrderByDescending(p => p.ProductID)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.Categories = db.Categories.ToList();

            return View(model);
        }

        public ActionResult ProductList(
            int? categoryId,
            string searchTerm,
            decimal? minPrice,
            decimal? maxPrice,
            string sortOrder,
            int? page)
        {
            var products = db.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p =>
                    p.CategoryID == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p =>
                    p.ProductPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p =>
                    p.ProductPrice <= maxPrice.Value);
            }

            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;

                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;

                default:
                    products = products.OrderByDescending(p => p.ProductID);
                    break;
            }

            int pageNumber = page ?? 1;
            int pageSize = 6;

            var model = new ProductSearchVM
            {
                SearchTerm = searchTerm,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOrder = sortOrder,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Products = products.ToPagedList(pageNumber, pageSize)
            };

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.CategoryId = categoryId;

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

            return View(product);
        }
    }
}
