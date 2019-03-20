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
    public class BatchesController : Controller
    {
        private AmsDbContext db = new AmsDbContext();

        // GET: Batches
        public ActionResult Index()
        {
            return View(db.Batches.Include("TrainingCenter").ToList());
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


        public PartialViewResult UpdateCandidateExcel(int batchId)
        {
            return PartialView(new CandidateExcel() { BatchId = batchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCandidateExcel(CandidateExcel ce, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!ce.ValidExcelFormets.Contains(ce.File.FileName.Split('.').Last()))
                    {
                        ModelState.AddModelError("File", "Upload a valid Candidate Excel File, allowed formats are : " + ce.ValidExcelFormets);
                        return View();
                    }
                    else
                    {
                        using (BinaryReader b = new BinaryReader(ce.File.InputStream))
                        {
                            Stream stream = new MemoryStream(b.ReadBytes(ce.File.ContentLength));
                            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            DataSet ds = excelReader.AsDataSet();
                            stream.Close();
                            var students = new List<Student>();
                            for (int i = 1; i < ds.Tables["Sheet1"].Rows.Count; i++)
                            {
                                students.Add(new Student()
                                {
                                    BatchId = ce.BatchId,
                                    CandidateCode = ds.Tables["Sheet1"].Rows[i][1].ToString(),
                                    StudentName = ds.Tables["Sheet1"].Rows[i][2].ToString(),
                                });


                            }
                            if (db.Students.Any(s => s.BatchId == ce.BatchId))
                                db.Students.RemoveRange(db.Students.Where(s => s.BatchId == ce.BatchId));

                            db.Students.AddRange(students);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }

                //{
                //    if (!string.IsNullOrEmpty(ccVm.CardImageUrl) || (ccVm.CardImageUpload != null && ccVm.CardImageUpload.ContentLength > 0))
                //    {
                //        if ((ccVm.CardImageUpload != null && ccVm.CardImageUpload.ContentLength > 0))
                //        {
                //            if (!validImageFormets.Contains(ccVm.CardImageUpload?.FileName.Split('.').Last()))
                //            {
                //                ModelState.AddModelError("CardImageUpload", "Upload Card Image in a valid image format, allowed formats are : " + validImageFormets);
                //                return View(ccVm);
                //            }
                //            else
                //            {
                //                ccVm.CardImageUrl = SaveImageAndGetUrl(ccVm.CardImageUpload);
                //            }
                //        }
                //        ccVm.CardId = ccVm.SaveBasic();
                //        if (ccVm.CardId > 0)
                //            return RedirectToAction("Details", new { id = ccVm.CardId });
                //        return RedirectToAction("Details", "creditcards", ccVm.CardId);
                //        //                   return RedirectToAction("Details", ccVm.CardId);
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("CardImageUpload", "This field is required");
                //        return View(ccVm);
                //    }
                //}
                //ViewBag.BankId = new SelectList(db.Banks, "BankId", "Name", ccVm.BankId);
                return View();
            }
            return View();
        }
    }
}
