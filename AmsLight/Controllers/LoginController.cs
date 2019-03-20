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
        // GET: Login
        public ActionResult Index()
        {
            return View(new Login() { UserName = "tp1", Password = "tp1", RememberMe = "on" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "UserName,Password,RememberMe")] Login login)
        {
            AmsDbContext db = new AmsDbContext();
            var user = db.Logins.Where(l => l.UserName == login.UserName && l.Password == login.Password).FirstOrDefault();
            if (user.TypeId == 1 && user.LoginUserId == -999)
            {
                // Redirect to Super andmin. Also add / set the user roles
            }
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.LoginUserId.ToString(), login?.RememberMe != null && login.RememberMe.Equals("on") ? true : false);
                //     var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
                //     Read Login Id ThroughOut the application.

                //db.Entry(user).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }
            else { ModelState.AddModelError("Password", "Invalid User Name or Password"); }
            return View(login);
        }

        public ActionResult forgotpassword()
        {
            return View();
        }
    }
}