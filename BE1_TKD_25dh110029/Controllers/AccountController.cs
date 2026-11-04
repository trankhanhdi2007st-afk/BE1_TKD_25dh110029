
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BE1_TKD_25dh110029.Models;
using BE1_TKD_25dh110029.Models.ViewModel;

namespace BE1_TKD_25dh110029.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool exists = db.Users.Any(u => u.Username == model.Username);

            if (exists)
            {
                ModelState.AddModelError(
                    "Username",
                    "Tên đăng nhập đã tồn tại!");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                UserRole = "C"
            };

            var customer = new Customer
            {
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                CustomerEmail = model.CustomerEmail,
                CustomerAddress = model.CustomerAddress,
                Username = model.Username
            };

            db.Users.Add(user);
            db.Customers.Add(customer);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Hãy đăng nhập.";
            return RedirectToAction("Login");
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = db.Users.FirstOrDefault(u =>
                u.Username == model.Username &&
                u.Password.Trim() == model.Password);

            if (user == null || user.UserRole == null)
            {
                ModelState.AddModelError(
                    "",
                    "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(model);
            }

            string role = user.UserRole.Trim();

            if (role != "A" && role != "C")
            {
                ModelState.AddModelError(
                    "",
                    "Tài khoản không có quyền truy cập!");
                return View(model);
            }

            Session["Username"] = user.Username;

            FormsAuthentication.SetAuthCookie(
                user.Username,
                model.RememberMe);

            if (role == "A")
            {
                return RedirectToAction(
                    "Index", "Home",
                    new { area = "Admin" });
            }

            return RedirectToAction(
                "Index", "Home",
                new { area = "" });
        }

        // GET: Account/ProfileInfo
        public ActionResult ProfileInfo()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login");

            string username = Session["Username"].ToString();

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (customer == null)
                return HttpNotFound();

            var model = new ProfileInfoVM
            {
                Username = customer.Username,
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail,
                CustomerAddress = customer.CustomerAddress
            };

            return View(model);
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(ProfileInfoVM model)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login");

            string username = Session["Username"].ToString();

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (customer == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                model.Username = username;
                model.CustomerName = customer.CustomerName;
                return View("ProfileInfo", model);
            }

            customer.CustomerPhone = model.CustomerPhone;
            customer.CustomerEmail = model.CustomerEmail;
            customer.CustomerAddress = model.CustomerAddress;

            db.SaveChanges();

            TempData["ProfileSuccess"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ProfileInfo");
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ProfileInfoVM model)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login");

            string username = Session["Username"].ToString();

            var user = db.Users.FirstOrDefault(
                u => u.Username == username);

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (user == null || customer == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(model.CurrentPassword)
                || user.Password.Trim() != model.CurrentPassword)
            {
                ModelState.AddModelError(
                    "CurrentPassword",
                    "Mật khẩu hiện tại không chính xác!");

                return View("ProfileInfo", GetProfileModel(customer));
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword)
                || model.NewPassword.Length < 6
                || model.NewPassword.Length > 50)
            {
                ModelState.AddModelError(
                    "NewPassword",
                    "Mật khẩu mới phải từ 6 đến 50 ký tự!");

                return View("ProfileInfo", GetProfileModel(customer));
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError(
                    "ConfirmNewPassword",
                    "Mật khẩu xác nhận không khớp!");

                return View("ProfileInfo", GetProfileModel(customer));
            }

            user.Password = model.NewPassword;
            db.SaveChanges();

            TempData["PasswordSuccess"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ProfileInfo");
        }

        private ProfileInfoVM GetProfileModel(Customer customer)
        {
            return new ProfileInfoVM
            {
                Username = customer.Username,
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail,
                CustomerAddress = customer.CustomerAddress
            };
        }

        // GET: Account/MyOrders
        public ActionResult MyOrders()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login");

            string username = Session["Username"].ToString();

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (customer == null)
                return HttpNotFound();

            var orders = db.Orders
                .Where(o => o.CustomerID == customer.CustomerID)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: Account/OrderDetail
        public ActionResult OrderDetail(int? id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login");

            if (id == null)
                return new HttpStatusCodeResult(
                    System.Net.HttpStatusCode.BadRequest);

            string username = Session["Username"].ToString();

            var customer = db.Customers.FirstOrDefault(
                c => c.Username == username);

            if (customer == null)
                return HttpNotFound();

            var order = db.Orders
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o =>
                    o.OrderID == id &&
                    o.CustomerID == customer.CustomerID);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Remove("Username");

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
