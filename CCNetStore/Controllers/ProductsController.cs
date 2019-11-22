using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CCNetStore.Models;

namespace CCNetStore.Controllers
{
    public class ProductsController : Controller
    {
        private CCStoreEntities db = new CCStoreEntities();

        // GET: AddToCart
        [Authorize(Roles = "client")]
        public ActionResult AddToCart(string id, string name)
        {
            int clientId = db.clients.FirstOrDefault(u => u.clientLogin == User.Identity.Name).clientId;

            var orders = db.carts.Where(c => c.clientId == clientId && c.productStatus == "reservated").FirstOrDefault();

            if (orders == null)
            {
                db.carts.Add(new cart { clientId = clientId, productId = id, productName = name, cpQuantity = 1, productStatus = "free" });
                db.SaveChanges();
            }
            else
            {
                return RedirectToAction("IndexClient");
            }

            return View();
        }

        // GET: Progucts
        [Authorize]
        public ActionResult IndexClient()
        {
            return View(db.products.ToList());
        }

        // GET: Products
        [Authorize(Roles = "admin, manager")]
        public ActionResult IndexManager()
        {
            return View(db.products.ToList());
        }

        // GET: Products/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "admin, manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [Authorize(Roles = "admin, manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productId,productName,productQuantity,productDescription,productPrice,productStatus")] product product)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(product);
                db.SaveChanges();
                return RedirectToAction("IndexManager");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "admin, manager")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [Authorize(Roles = "admin, manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productId,productName,productQuantity,productDescription,productPrice,productStatus")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexManager");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "admin, manager")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Roles = "admin, manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("IndexManager");
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
