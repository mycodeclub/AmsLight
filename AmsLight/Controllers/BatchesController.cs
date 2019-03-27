using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AmsLight.Models;
using ExcelDataReader;


namespace AmsLight.Controllers
{
    [Authorize]
    public class BatchesController : Controller
    {
        private AmsDbContext db = new AmsDbContext();

        // GET: Batches
        public ActionResult Index(int? tcId)
        {
            ViewBag.SelectedCenterId = 0;
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.TrainingCenters = db.TrainingCenters.Where(tc => tc.TpId == tpId).ToList();

            ViewBag.BatcheId = new SelectList(db.Batches.Where(b => b.TpId == tpId).ToList(), "BatchId", "BatchCode");
            List<Batch> batches;
            if (tcId.HasValue)
            {
                batches = db.Batches.Include("TrainingCenter").Where(b => b.TpId == tpId && b.TrainingCenterId == tcId.Value).ToList();
                ViewBag.SelectedCenterId = tcId.Value;
            }
            else batches = new List<Batch>();
            return View(batches);
        }

        // GET: Batches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = db.Batches.Include("TrainingCenter").Include("Students").Where(b => b.BatchId == id).FirstOrDefault();
            if (batch == null)
            {
                return HttpNotFound();
            }
            return View(batch);
        }

        // GET: Batches/Create
        public ActionResult Create()
        {
            ViewBag.TrainingCenterId = new SelectList(db.TrainingCenters, "TrainingCenterId", "CenterCode");
            return View();
        }

        // POST: Batches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BatchId,BatchCode,Center,StartTime,EndTime,Trainer1,Trainer2,StartDate,CreateDate")] Batch batch)
        {
            if (ModelState.IsValid)
            {
                batch.CreateDate = System.DateTime.Now;
                db.Batches.Add(batch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(batch);
        }

        // GET: Batches/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.TrainingCenterId = new SelectList(db.TrainingCenters, "TrainingCenterId", "CenterCode");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = db.Batches.Include("Students").Where(b => b.BatchId == id.Value).FirstOrDefault();
            if (batch.Students == null || batch.Students.Count == 0)
            {
                batch.Students = new List<Student>() { };
                for (int i = 0; i < 30; i++)
                { batch.Students.Add(new Student() { BatchId = id.Value }); }
            }

            if (batch == null)
            {
                return HttpNotFound();
            }
            return View(batch);
        }

        // POST: Batches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BatchId,BatchCode,Center,StartTime,EndTime,Trainer1,Trainer2,StartDate,CreateDate")] Batch batch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(batch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TrainingCenterId = new SelectList(db.TrainingCenters, "TrainingCenterId", "CenterCode");
            batch.Students = db.Students.Where(s => s.BatchId == batch.BatchId).ToList();
            return View(batch);
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
