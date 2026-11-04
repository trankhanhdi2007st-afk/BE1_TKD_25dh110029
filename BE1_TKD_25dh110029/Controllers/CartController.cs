
using System;
using System.Data.Entity;
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
                return HttpNotFound();

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

        // GET: Cart/Checkout
        [HttpGet]
        public ActionResult Checkout()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCartService().GetCart();

            if (cart.Items == null || !cart.Items.Any())
                return RedirectToAction("Index");

            string username = Session["Username"].ToString();

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (customer == null)
                return HttpNotFound();

            ViewBag.CustomerName = customer.CustomerName;
            ViewBag.CustomerPhone = customer.CustomerPhone;
            ViewBag.CustomerAddress = customer.CustomerAddress;

            return View(cart);
        }

        // POST: Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string addressDelivery)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(addressDelivery))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ giao hàng!";
                return RedirectToAction("Checkout");
            }

            string username = Session["Username"].ToString();

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (customer == null)
                return HttpNotFound();

            var cart = GetCartService().GetCart();

            if (cart.Items == null || !cart.Items.Any())
                return RedirectToAction("Index");

            var orderItems = cart.Items
                .Where(item => item.Quantity > 0)
                .Select(item => new
                {
                    Item = item,
                    Product = db.Products.FirstOrDefault(
                        p => p.ProductID == item.ProductID)
                })
                .Where(x => x.Product != null)
                .ToList();

            if (!orderItems.Any())
            {
                TempData["Error"] =
                    "Không có sản phẩm hợp lệ trong giỏ hàng!";

                return RedirectToAction("Index");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        CustomerID = customer.CustomerID,
                        OrderDate = DateTime.Now,
                        TotalAmount = 0,
                        PaymentStatus = "Chưa thanh toán",
                        AddressDelivery = addressDelivery.Trim()
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    decimal total = 0;

                    foreach (var item in orderItems)
                    {
                        var detail = new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ProductID = item.Product.ProductID,
                            Quantity = item.Item.Quantity,
                            UnitPrice = item.Product.ProductPrice
                        };

                        total += item.Product.ProductPrice
                            * item.Item.Quantity;

                        db.OrderDetails.Add(detail);
                    }

                    order.TotalAmount = total;
                    db.SaveChanges();

                    transaction.Commit();

                    GetCartService().ClearCart();

                    return RedirectToAction(
                        "OrderSuccess",
                        new { id = order.OrderID });
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // GET: Cart/OrderSuccess
        public ActionResult OrderSuccess(int? id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return HttpNotFound();

            string username = Session["Username"].ToString();

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (customer == null)
                return HttpNotFound();

            var order = db.Orders.FirstOrDefault(
                o => o.OrderID == id &&
                     o.CustomerID == customer.CustomerID);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
