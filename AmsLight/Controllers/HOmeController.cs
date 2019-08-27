using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmsLight.Controllers
{
    public class HomeController : Controller
    {
        // GET: HOme
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        public ActionResult InActive()
        {
             return View();
        }
    }
}