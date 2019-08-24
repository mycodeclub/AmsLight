using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AmsLight.Models;

namespace AmsLight.Controllers
{
    [Authorize]
    public class TrainingCentersController : Controller
    {
        private AmsDbContext db = new AmsDbContext();

        // GET: TrainingCenters
        public ActionResult Index()
        {
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            
            return View(db.TrainingCenters.Where(tc=>tc.TpId==tpId).ToList());
        }

        // GET: TrainingCenters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCenter trainingCenter = db.TrainingCenters.Find(id);
            if (trainingCenter == null)
            {
                return HttpNotFound();
            }
            return View(trainingCenter);
        }

        // GET: TrainingCenters/Create
        public ActionResult Create()
        {
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            var tp = db.TrainingCenters.Find(tpId); return View(new TrainingCenter() { TpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name) });
        }

        // POST: TrainingCenters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TrainingCenterId,TpId,CenterCode,CenterName,Phone1,Phone2,City,State,AddressLine1,AddressLine2,Zip,IsActive")] TrainingCenter trainingCenter)
        {
            if (ModelState.IsValid)
            {
                db.TrainingCenters.Add(trainingCenter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trainingCenter);
        }

        // GET: TrainingCenters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCenter trainingCenter = db.TrainingCenters.Find(id);
            if (trainingCenter == null)
            {
                return HttpNotFound();
            }
            return View(trainingCenter);
        }

        // POST: TrainingCenters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TrainingCenterId,TpId,CenterCode,CenterName,Phone1,Phone2,City,State,AddressLine1,AddressLine2,Zip,IsActive")] TrainingCenter trainingCenter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingCenter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainingCenter);
        }

        // GET: TrainingCenters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCenter trainingCenter = db.TrainingCenters.Find(id);
            if (trainingCenter == null)
            {
                return HttpNotFound();
            }
            return View(trainingCenter);
        }

        // POST: TrainingCenters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrainingCenter trainingCenter = db.TrainingCenters.Find(id);
            db.TrainingCenters.Remove(trainingCenter);
            db.SaveChanges();
            return RedirectToAction("Index");
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
