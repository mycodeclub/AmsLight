using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmsLight.Controllers
{
    public class AttendanceController : Controller
    {
        // GET: Attendances
        public ActionResult Index()
        {
            return View();
        }
    }
}