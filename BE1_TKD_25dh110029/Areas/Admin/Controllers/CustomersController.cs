using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE1_TKD_25dh110029.Models;

namespace BE1_TKD_25dh110029.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Customers
        public ActionResult Index(string searchString, string sortOrder, int page = 1)
        {
            int pageSize = 8;

            var customers = db.Customers.Include(c => c.User).AsQueryable();

            // Tìm kiếm theo tên khách hàng
            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.SortOrder = sortOrder;

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(c => c.CustomerName);
                    break;

                case "email":
                    customers = customers.OrderBy(c => c.CustomerEmail);
                    break;

                default:
                    customers = customers.OrderBy(c => c.CustomerName);
                    break;
            }

            // Phân trang
            int totalItems = customers.Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var result = customers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(result);
        }

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {
            ViewBag.Username = new SelectList(db.Users, "Username", "Password");
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Username = new SelectList(db.Users, "Username", "Password", customer.Username);
            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Username = new SelectList(db.Users, "Username", "Password", customer.Username);
            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Username = new SelectList(db.Users, "Username", "Password", customer.Username);
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
