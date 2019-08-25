using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AmsLight.Models;
using ExcelDataReader;

namespace AmsLight.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private AmsDbContext db = new AmsDbContext();

        // GET: Students
        public ActionResult Index(int? centerId, int? batchId)
        {
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.TrainingCenters = db.TrainingCenters.Where(tc => tc.TpId == tpId).ToList();
            ViewBag.selectedTrainingCenteId = 0;
            ViewBag.selectedBatchId = 0;
            if (centerId.HasValue)
            {
                ViewBag.Batches = db.Batches.Where(b => b.TpId == tpId && b.TrainingCenterId.Equals(centerId.Value)).ToList();
                ViewBag.selectedTrainingCenteId = centerId.Value;
            }
            var students = new List<Student>();
            if (batchId.HasValue)
            {
                students = db.Students.Where(s =>
                                                batchId.Value.Equals(s.BatchId)
                                                && tpId.Equals(s.TpId)
                                             ).ToList();
                ViewBag.selectedBatchId = batchId.Value;
            }
            return View(students);
        }

        // GET: Students/Create
        public ActionResult Create(int? id = 0) // Batch Id
        {
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.Students = db.Students.Where(s => s.BatchId == id.Value && s.TpId.Equals(tpId)).ToList();
            return View(new CandidateExcel() { BatchId = id.Value });
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CandidateExcel ce)
        {
            bool isSaved = UpdateCandidateExcel(ce);
            ViewBag.isSaved = isSaved;
            ViewBag.selectedBatchId = ce.BatchId;
            ViewBag.Students = db.Students.Where(s => s.BatchId == ce.BatchId).ToList();
            return View(ce);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null || !student.TpId.Equals(tpId))
            {
                return HttpNotFound();
            }
            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchCode", student.BatchId);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
                if (student.TpId.Equals(tpId))
                {
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchCode", student.BatchId);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            Student student = db.Students.Find(id);
            if (student == null && !student.Equals(tpId))
            {
                return HttpNotFound();
            }
            return View(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public bool UpdateCandidateExcel(CandidateExcel ce)
        {
            bool isSavedSuccessfully = false;
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);

            if (ModelState.IsValid)
            {
                try
                {
                    if (!ce.ValidExcelFormets.Contains(ce.File.FileName.Split('.').Last()))
                        ModelState.AddModelError("File", "Upload a valid Candidate Excel File, allowed formats are : " + ce.ValidExcelFormets);
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
                                    TpId = tpId,
                                });
                            }
                            if (db.Students.Any(s => s.BatchId == ce.BatchId))
                                db.Students.RemoveRange(db.Students.Where(s => s.BatchId == ce.BatchId));
                            db.Students.AddRange(students);
                            db.SaveChanges();
                            isSavedSuccessfully = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("File", "Invalid Excel : " + ex.Message);

                }
            }
            return isSavedSuccessfully;
        }

        public ActionResult AddNewStudent(int CenterId, int BatchId)
        {
            TempData["CenterId"] = CenterId;
            TempData["BatchId"] = BatchId;
            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchCode");

            return View();
        }// POST: StudentsNew/AddNewStudent
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewStudent([Bind(Include = "StudentId,StudentName,Father_Husband_Name,Gender,Address,MobileNo,AadhaarId,BatchId,CandidateCode")] Student student)
        {
            if (ModelState.IsValid)
            {
                student.BatchId = Convert.ToInt32(TempData["BatchId"]);
                student.TpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BatchId = new SelectList(db.Batches, "BatchId", "BatchCode", student.BatchId);
            return View(student);
        }
    }
}
