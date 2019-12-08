using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCNetStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Test for second branch
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult IndexAdmin()
        {
            return View();
        }

        [Authorize(Roles = "manager")]
        public ActionResult IndexManager()
        {
            return View();
        }
        [Authorize(Roles = "client")]
        public ActionResult IndexClient()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}