using AmsLight.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AmsLight.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        Random random = new Random();
        private AmsDbContext db = new AmsDbContext();
        private int tpId;
        public AttendanceController()
        {
            tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
        }
        // GET: Attendances
        public ActionResult Index()
        {
            Attendance att = new Attendance();
            return View(att);
        }
        public ActionResult DownloadCSV(Attendance att)
        {
            db.Configuration.ProxyCreationEnabled = false;

            Random r = new Random();
            try
            {
                att.SelectedBatch = db.Batches.Find(att.SelectedBatch.BatchId);
                att.SelectedTc = db.TrainingCenters.Find(att.SelectedTc.TrainingCenterId);
                var csv = new StringBuilder();
                int count = 3;
                var CandidateCode = string.Empty; // Populated by Excel
                                                  //  var attDownloadTime = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, random.Next(8, 11), random.Next(0, 59), random.Next(0, 59));
                var attDownloadTime = new DateTime(System.DateTime.Now.Year, att.AttendancesDate.Month, att.AttendancesDate.Day, random.Next(8, 11), random.Next(0, 59), random.Next(0, 59));
                var machineId = "34173cb38f07f89ddbebc2ac9128303f"; // Update BioMetric Machine Id as per center ...
                var attendancesDate = att.AttendancesDate.Year.ToString() + FormatToTwoDigit(att.AttendancesDate.Month) + FormatToTwoDigit(att.AttendancesDate.Day);
                csv.Append("FH^1^1.0.0^TP^12^" + attDownloadTime.Year + FormatToTwoDigit(attDownloadTime.Month) + FormatToTwoDigit(attDownloadTime.Day) + FormatToTwoDigit(attDownloadTime.Hour) + ":" + FormatToTwoDigit(attDownloadTime.Minute) + ":" + FormatToTwoDigit(attDownloadTime.Second) + "+0530^45^" + machineId + "\n");
                csv.Append("BH^2^TPC^" + att.SelectedTc.CenterCode + "\n");
                //--------------------- Find some better approch to populate Punch IN OUT time for Trainer .... ...............
                var T1punchIn = attendancesDate + FormatToTwoDigit(att.SelectedBatch.StartTime.Hours) + ":" + FormatToTwoDigit(random.Next(att.SelectedBatch.StartTime.Minutes - 10, att.SelectedBatch.StartTime.Minutes + 5)) + ":" + FormatToTwoDigit(random.Next(1, 59));
                var T1punchOut = attendancesDate + FormatToTwoDigit(att.SelectedBatch.EndTime.Hours) + ":" + FormatToTwoDigit(random.Next(att.SelectedBatch.EndTime.Minutes + 15, att.SelectedBatch.EndTime.Minutes + 29)) + ":" + FormatToTwoDigit(random.Next(1, 59));
                var T2punchIn = attendancesDate + FormatToTwoDigit(att.SelectedBatch.StartTime.Hours) + ":" + FormatToTwoDigit(random.Next(att.SelectedBatch.StartTime.Minutes - 10, att.SelectedBatch.StartTime.Minutes + 5)) + ":" + FormatToTwoDigit(random.Next(1, 59));
                var T2punchOut = attendancesDate + FormatToTwoDigit(att.SelectedBatch.EndTime.Hours) + ":" + FormatToTwoDigit(random.Next(att.SelectedBatch.EndTime.Minutes + 15, att.SelectedBatch.EndTime.Minutes + 29)) + ":" + FormatToTwoDigit(random.Next(1, 59));
                //--------------------- Find some better approch to populate Punch IN OUT time for Trainer .... ...............

                csv.Append("BD^" + count + "^" + att.SelectedBatch.BatchCode + "^A^" + "TRN^" + att.SelectedBatch.Trainer1 + "^P^" + attendancesDate + "^" + T1punchIn + "+0530^" + T1punchOut + "+0530\n");
                csv.Append("BD^" + count + "^" + att.SelectedBatch.BatchCode + "^A^" + "TRN^" + att.SelectedBatch.Trainer2 + "^P^" + attendancesDate + "^" + T2punchIn + "+0530^" + T2punchOut + "+0530\n");
                foreach (var stu in att.Students)
                {
                    var punchInTime = attendancesDate + FormatToTwoDigit(stu.PunchInTime.Hours) + ":" + FormatToTwoDigit(stu.PunchInTime.Minutes) + ":" + FormatToTwoDigit(stu.PunchInTime.Seconds);
                    var punchOutTime = attendancesDate + FormatToTwoDigit(stu.PunchOutTime.Hours) + ":" + FormatToTwoDigit(stu.PunchOutTime.Minutes) + ":" + FormatToTwoDigit(stu.PunchOutTime.Seconds);
                    if (stu.IsPresent) csv.Append("BD^" + count + "^" + att.SelectedBatch.BatchCode + "^A^CAN^" + stu.CandidateCode + "^P^" + attendancesDate + "^" + punchInTime + "+0530^" + punchOutTime + "+0530");
                    else csv.Append("BD^" + count + "^" + att.SelectedBatch.BatchCode + "^A^CAN^" + stu.CandidateCode + "^A^^");
                    if (!att.Students.Last().CandidateCode.Equals(stu.CandidateCode))
                        csv.Append("\n");
                    count++;
                }
                var today = att.AttendancesDate;
                var attLog = db.AttendanceLog.Where(al => (DbFunctions.TruncateTime(al.AttendancesDate) <= today) && (al.batchId == att.SelectedBatch.BatchId)).FirstOrDefault();
                if (attLog == null)
                {
                    attLog = new AttendanceLog();
                }
                attLog.batchId = att.SelectedBatch.BatchId;
                attLog.CreateDate = System.DateTime.Now;
                attLog.AttendancesDate = att.AttendancesDate;
                attLog.AttendanceCsv = csv.ToString();
                // attLog.ObjJson = new JavaScriptSerializer().Serialize(att.SelectedBatch);
                db.AttendanceLog.Add(attLog);
                if (attLog.AttendanceId > 0)
                    db.Entry(attLog).State = EntityState.Modified;
                db.SaveChanges();

                return File(new System.Text.UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "attendances" + attendancesDate + ".csv");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult DownloadPreviousAttendance()
        {
            Attendance att = new Attendance();
            return View(att);

        }

        public ActionResult DownloadPreviousAttCsv(FormCollection fc)
        {
            var trainingCenterId = fc["SelectedTc.TrainingCenterId"];
            var batchId = Convert.ToInt32( fc["SelectedBatch.BatchId"]);
            var date = Convert.ToDateTime(fc["AttendancesDate"]);
            var attLog = db.AttendanceLog.Where(al => (DbFunctions.TruncateTime(al.AttendancesDate) <= date) && (al.batchId == batchId)).FirstOrDefault();
            if (attLog == null)
            {
                 return File(new System.Text.UTF8Encoding().GetBytes("System Did Not Have Attendance Record For Date : " + fc["AttendancesDate"]), "text/txt", "AttendanceNotFound.txt");
            }
            return File(new System.Text.UTF8Encoding().GetBytes(attLog.AttendanceCsv.ToString()), "text/csv", "attendances" + attLog.AttendancesDate.Date.ToString() + ".csv");
          }
        public JsonResult GetBatchesByCenterId(int CenterId)
        {
            var batches = db.Batches.Where(b => b.TrainingCenterId == CenterId).ToList();
            return Json(batches.Select(b => new { Id = b.BatchId, Time = b.BatchCode + " ( " + b.StartTime.ToString() + " - " + b.EndTime.ToString() + " ) " }).ToList(),
                JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetStudentsByBatchId(int tcId = 2, int batchId = 1)
        {
            var tpId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
            Attendance att = (tcId > 0 && batchId > 0) ? new Attendance(tcId, batchId) : (tcId > 0) ? new Attendance(tcId) : new Attendance();
            Random random = new Random();
            att.Students.ForEach(s =>
            {
                s.IsPresent = true;
                s.PunchInTime = new TimeSpan(
                   Convert.ToInt32(random.Next(att.SelectedBatch.StartTime.Hours, att.SelectedBatch.StartTime.Hours)),
                   Convert.ToInt32(random.Next(att.SelectedBatch.StartTime.Minutes - 20, att.SelectedBatch.StartTime.Minutes + 5)),
                   Convert.ToInt32(random.Next(1, 60)));
                s.PunchOutTime = new TimeSpan(
    Convert.ToInt32(random.Next(att.SelectedBatch.EndTime.Hours, att.SelectedBatch.EndTime.Hours)),
    Convert.ToInt32(random.Next(att.SelectedBatch.EndTime.Minutes, att.SelectedBatch.EndTime.Minutes + 10)),
    Convert.ToInt32(random.Next(1, 60)));
            });

            return PartialView(att);
        }
        private string FormatToTwoDigit(string num)
        {
            if (num.Contains('.')) { num = num.Split('.').First(); }
            return Convert.ToInt32(num).ToString("00");
        }
        private string FormatToTwoDigit(int num)
        {
            return num.ToString("00");
        }
    }

}