using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Web.Mvc;
using BE1_TKD_25dh110029.Models;

namespace BE1_TKD_25dh110029.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Danh sách đơn hàng
        public ActionResult Index(string paymentStatus)
        {
            var orders = db.Orders
                .Include(o => o.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orders = orders.Where(o => o.PaymentStatus == paymentStatus);
            }

            orders = orders.OrderByDescending(o => o.OrderDate);

            var statuses = new[]
            {
        "Chưa thanh toán",
        "Đã thanh toán",
        "Đã hủy"
    };

            ViewBag.PaymentStatuses = new SelectList(
                statuses,
                paymentStatus
            );

            return View(orders.ToList());
        }
        // Danh sách đơn hàng theo khách hàng
        public ActionResult CustomerOrders(int? customerId)
        {
            if (customerId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = db.Customers.Find(customerId);

            if (customer == null)
                return HttpNotFound();

            var orders = db.Orders
                .Include(o => o.Customer)
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.CustomerName = customer.CustomerName;
            ViewBag.CustomerID = customer.CustomerID;

            return View(orders);
        }

        // Chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }
        // GET: Admin/Orders/Edit/1
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders.Find(id);

            if (order == null)
                return HttpNotFound();

            ViewBag.PaymentStatus = new SelectList(
                new[]
                {
            "Chưa thanh toán",
            "Đã thanh toán",
            "Đã hủy"
                },
                order.PaymentStatus
            );

            return View(order);
        }

        // POST: Admin/Orders/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            int orderID,
            string paymentStatus)
        {
            var order = db.Orders.Find(orderID);

            if (order == null)
                return HttpNotFound();

            var allowedStatuses = new[]
            {
        "Chưa thanh toán",
        "Đã thanh toán",
        "Đã hủy"
    };

            if (!allowedStatuses.Contains(paymentStatus))
            {
                ModelState.AddModelError(
                    "PaymentStatus",
                    "Trạng thái thanh toán không hợp lệ!"
                );

                ViewBag.PaymentStatus = new SelectList(
                    allowedStatuses,
                    order.PaymentStatus
                );

                return View(order);
            }

            order.PaymentStatus = paymentStatus;
            db.SaveChanges();

            TempData["SuccessMessage"] =
                "Cập nhật trạng thái đơn hàng thành công!";

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}