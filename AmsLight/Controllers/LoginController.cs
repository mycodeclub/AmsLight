using AmsLight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AmsLight.Controllers
{
    public class LoginController : Controller
    {
        AmsDbContext db = new AmsDbContext();
        // GET: Login
        public ActionResult Index()
        {
            return View(new Login());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "UserName,Password,RememberMe")] Login login)
        {
            var user = db.Logins.Where(l => l.UserName == login.UserName && l.Password == login.Password).FirstOrDefault();
            if (user != null)
            {
                if (user.TypeId == 1 && user.TpId == -999)
                {
                    FormsAuthentication.SetAuthCookie(user.TpId.ToString(), login?.RememberMe != null && login.RememberMe.Equals("on") ? true : false);
                    return RedirectToAction("Index", "SuperAdmin");
                }
                if (user != null)
                {
                    var tp = db.TrainingPartner.Find(user.TpId);
                    if (!tp.IsActive)
                    {
                        return RedirectToAction("InActive", "Home");
                    }
                    FormsAuthentication.SetAuthCookie(user.TpId.ToString(), login?.RememberMe != null && login.RememberMe.Equals("on") ? true : false);
                    var name = db.TrainingPartner.Find(user.TpId).TpName;
                    Session["LoginTp"] = name;
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else { ModelState.AddModelError("Password", "Invalid User Name or Password"); }

            return View(login);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult forgotpassword()
        {
            return View();
        }
    }
}