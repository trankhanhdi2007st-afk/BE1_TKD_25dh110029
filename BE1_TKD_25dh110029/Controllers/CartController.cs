
using System.Linq;
using System.Web.Mvc;
using BE1_TKD_25dh110029.Models;
using BE1_TKD_25dh110029.Models.ViewModel;

namespace BE1_TKD_25dh110029.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private CartService GetCartService()
        {
            return new CartService();
        }

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var item = new CartItem
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductImage = product.ProductImage,
                ProductPrice = product.ProductPrice,
                Quantity = quantity > 0 ? quantity : 1,
                Category = product.Category == null
                    ? "Sản phẩm"
                    : product.Category.CategoryName,
                IsSelected = true
            };

            var service = GetCartService();
            var cart = service.GetCart();

            cart.AddItem(item);
            service.SaveCart(cart);

            return RedirectToAction("Index");
        }

        // Xóa một sản phẩm
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var service = GetCartService();
            var cart = service.GetCart();

            cart.RemoveItem(id);
            service.SaveCart(cart);

            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        [HttpPost]
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();

            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var service = GetCartService();
            var cart = service.GetCart();

            cart.UpdateQuantity(id, quantity);
            service.SaveCart(cart);

            return RedirectToAction("Index");
        }
    }
}
