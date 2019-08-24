using AmsLight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmsLight.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private AmsDbContext db = new AmsDbContext();
        // GET: Dashboard
        public ActionResult Index()
        {
            int tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            ViewData["BatchCount"] = (from tc in db.TrainingCenters
                                      join b in db.Batches on tc.TrainingCenterId equals b.TrainingCenterId
                                      where tc.TpId == tpId
                                      select new { b.BatchId }).Count();

            var tcCount= (from tc in db.TrainingCenters where tc.TpId == tpId select tc).Count();
            ViewData["TrainingCentersCount"] = tcCount;
             //join b in db.Batches on tc.TrainingCenterId equals b.TrainingCenterId



            return View();
        }
    }
}