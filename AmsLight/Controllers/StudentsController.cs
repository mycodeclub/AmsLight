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
        public ActionResult Index(int? id)
        {
            var students = new List<Student>();
            if (id != null)
            {
                students = db.Students.Where(s => s.BatchId == id.Value).ToList();
                ViewBag.BatchId = id.Value;

            }
            else { ViewBag.BatchId = 0; }
            var batches = db.Batches.ToList();
            ViewBag.Batches = db.Batches.ToList();
            return View(students);
        }


        // GET: Students/Create
        public ActionResult Create(int? id = 0) // Batch Id
        {
            ViewBag.Students = db.Students.Where(s => s.BatchId == id.Value).ToList();
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
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
        public ActionResult Edit([Bind(Include = "StudentId,StudentName,BatchId,CandidateCode,PunchOutTime,IsPresent")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
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
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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




        public bool UpdateCandidateExcel(CandidateExcel ce)
        {
            bool isSavedSuccessfully = false;

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
                    ce.Message = ex.Message;
                }
            }
            return isSavedSuccessfully;
        }

    }
}
