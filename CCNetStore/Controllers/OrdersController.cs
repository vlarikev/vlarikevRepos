using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CCNetStore.Models;

namespace CCNetStore.Controllers
{
    public class OrdersController : Controller
    {
        private CCStoreEntities db = new CCStoreEntities();

        // GET: PayForTheOrder
        [Authorize]
        public ActionResult PayForTheOrder(int id)
        {
            var order = db.Orders.Where(o => o.Id == id).FirstOrDefault();
            if (order.orderStatus == "confirmed")
            {
                order.orderStatus = "PAYED";
                int clientId = db.clients.FirstOrDefault(u => u.clientLogin == User.Identity.Name).clientId;
                var carts = db.carts.Where(c => c.clientId == clientId).ToList();

                db.carts.RemoveRange(carts);
                db.SaveChanges();
            }
            else
            {
                return RedirectToAction("IndexClient");
            }

            return View();
        }

        // GET: ConfirmOrder
        [Authorize]
        public ActionResult ConfirmOrder()
        {
            int clientId = db.clients.FirstOrDefault(c => c.clientLogin == User.Identity.Name).clientId;

            var carts = db.carts.Where(c => c.clientId == clientId).ToList();
            carts.ForEach(c => c.productStatus = "reservated");

            var listId = carts.Select(c => c.productId);

            var products = db.products.Where(p => listId.Contains(p.productId)).ToList();
            products.ForEach(p => p.productQuantity--);

            db.SaveChanges();

            db.Orders.Add(new Order { clientId = clientId, orderDate = DateTime.Now, totalPrice = CartsController.totalPrice, orderStatus = "confirmed", deliveryAddress = null});
            db.SaveChanges();
            CartsController.totalPrice = 0;
            return View();
        }

        // GET: Orders
        [Authorize(Roles = "admin, manager")]
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.client);
            return View(orders.ToList());
        }
        [Authorize]
        public ActionResult IndexClient()
        {
            var orders = db.Orders.Include(o => o.client);
            return View(orders.Where(c => c.client.clientLogin == User.Identity.Name).ToList());
        }

        // GET: Orders/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.clientId = new SelectList(db.clients, "clientId", "clientLogin", order.clientId);
            return View(order);
        }

        // POST: Orders/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,clientId,orderDate,totalPrice,orderStatus,deliveryAddress")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
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
            ViewBag.clientId = new SelectList(db.clients, "clientId", "clientLogin", order.clientId);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            
            int clientId = db.clients.FirstOrDefault(u => u.clientLogin == User.Identity.Name).clientId;
            var carts = db.carts.Where(c => c.clientId == clientId).ToList();

            if (order.orderStatus != "PAYED")
            {
                var listId = carts.Select(c => c.productId);
                var products = db.products.Where(p => listId.Contains(p.productId)).ToList();
                products.ForEach(p => p.productQuantity++);
            }

            db.carts.RemoveRange(carts);

            db.SaveChanges();
            if (User.IsInRole("admin") || User.IsInRole("manager"))
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
