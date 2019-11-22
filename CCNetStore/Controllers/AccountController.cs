using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CCNetStore.Models;

namespace CCNetStore.Controllers
{
    public class AccountController : Controller
    {
        private CCStoreEntities db = new CCStoreEntities();
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(client client)
        {
            if (ModelState.IsValid)
            {
                client user = null;
                user = db.clients.FirstOrDefault(u => u.clientLogin == client.clientLogin);
                if (user == null)
                {
                    db.clients.Add(new client { clientLogin = client.clientLogin, clientPassword = client.clientPassword, clientRole = "client"});
                    db.SaveChanges();

                    user = db.clients.Where(u => u.clientLogin == client.clientLogin && u.clientPassword == client.clientPassword).FirstOrDefault();
                }
                if (user != null)
                {
                    return RedirectToAction("Login", "Account"); 
                }
            }
            else
            {
                ModelState.AddModelError("", "User with such email is already registered");
            }

            return View(client);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(client client)
        {
            if (ModelState.IsValid)
            {
                client user = null;
                user = db.clients.FirstOrDefault(u => u.clientLogin == client.clientLogin && u.clientPassword == client.clientPassword);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(client.clientLogin, true);
                    
                    if (user.clientRole == "admin")
                    {
                        return RedirectToAction("IndexManager", "Products");
                    }
                    if (user.clientRole == "manager")
                    {
                        return RedirectToAction("IndexManager", "Products");
                    }
                    if (user.clientRole == "client")
                    {
                        return RedirectToAction("IndexClient", "Products", new { id = user.clientId });
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User with such Login isn't registered");
            }
            return View(client);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}