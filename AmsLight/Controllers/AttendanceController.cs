using AmsLight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmsLight.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private AmsDbContext db = new AmsDbContext();

        // GET: Attendances
        public ActionResult Index(int tcId = 2, int batchId = 1)
        {
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.TrainingCenters = db.TrainingCenters.Where(tc => tc.TpId == tpId).ToList();
            ViewBag.Batches = db.Batches.Where(b => b.TrainingCenterId == tcId).ToList();
            ViewBag.Students = db.Students.Where(s => s.BatchId == batchId).ToList();
            ViewBag.SelectedTcId = tcId;
            ViewBag.SelectedBatchId = batchId;

            return View();
        }
    }
}