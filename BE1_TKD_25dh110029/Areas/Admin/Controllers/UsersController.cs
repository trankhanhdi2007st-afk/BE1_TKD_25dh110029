
using System;
using System.Linq;
using System.Web.Mvc;
using BE1_TKD_25dh110029.Models;

namespace BE1_TKD_25dh110029.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Danh sách tài khoản
        public ActionResult Index(string searchString, string sortOrder, int page = 1)
        {
            int pageSize = 8;

            var users = db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                users = users.Where(u => u.Username.Contains(searchString));

            ViewBag.CurrentFilter = searchString;
            ViewBag.SortOrder = sortOrder;

            switch (sortOrder)
            {
                case "role":
                    users = users.OrderBy(u => u.UserRole);
                    break;
                case "username_desc":
                    users = users.OrderByDescending(u => u.Username);
                    break;
                default:
                    users = users.OrderBy(u => u.Username);
                    break;
            }

            int totalItems = users.Count();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var result = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(result);
        }

        // Thêm tài khoản
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string username, string password, string userRole)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(userRole))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (db.Users.Any(u => u.Username == username))
            {
                ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại.";
                return View();
            }

            db.Users.Add(new User
            {
                Username = username,
                Password = password,
                UserRole = userRole
            });

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Sửa tài khoản
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var user = db.Users.Find(id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, string password, string userRole)
        {
            var user = db.Users.Find(id);

            if (user == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(userRole))
            {
                ViewBag.ErrorMessage = "Vui lòng chọn vai trò.";
                return View(user);
            }

            if (!string.IsNullOrWhiteSpace(password))
                user.Password = password;

            user.UserRole = userRole;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Xóa tài khoản
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var user = db.Users.Find(id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);

            if (user == null)
                return HttpNotFound();

            // Không xóa tài khoản đang liên kết với khách hàng
            bool hasCustomer = db.Customers.Any(c => c.Username == id);

            if (hasCustomer)
            {
                TempData["ErrorMessage"] =
                    "Không thể xóa tài khoản đang liên kết với khách hàng.";

                return RedirectToAction("Index");
            }

            db.Users.Remove(user);
            db.SaveChanges();

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
