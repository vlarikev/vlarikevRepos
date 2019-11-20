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
    public class CartsController : Controller
    {
        private CCStoreEntities db = new CCStoreEntities();

        public static decimal totalPrice = 0;
        // GET: Carts
        [Authorize(Roles ="admin, manager")]
        public ActionResult Index()
        {
            var carts = db.carts.Include(c => c.client).Include(c => c.product);
            return View(carts.ToList());
        }
        [Authorize(Roles = "client")]
        public ActionResult IndexClient()
        {
            var carts = db.carts.Include(c => c.client).Include(c => c.product);

            totalPrice = carts.Where(c => c.client.clientLogin == User.Identity.Name).Sum(p => p.product.productPrice).Value;
            return View(carts.Where(c => c.client.clientLogin == User.Identity.Name).ToList());
        }

        // GET: Carts/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cart cart = db.carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cart cart = db.carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string pId)
        {
            cart cart = db.carts.Find(id);
            db.carts.Remove(cart);

            var product = db.products.Where(o => o.productId == pId).FirstOrDefault();
            product.productQuantity++;

            db.SaveChanges();
            if(User.IsInRole("admin") || User.IsInRole("manager"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("IndexClient");
            }
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
