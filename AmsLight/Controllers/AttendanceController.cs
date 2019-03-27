using AmsLight.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AmsLight.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
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
            try
            {
                att.SelectedBatch = db.Batches.Find(att.SelectedBatch.BatchId);
                att.SelectedTc = db.TrainingCenters.Find(att.SelectedTc.TrainingCenterId);
                var csv = new StringBuilder();
                int count = 3;
                var CandidateCode = string.Empty; // Populated by Excel
                var attendancesDate = att.AttendancesDate.Year.ToString() + att.AttendancesDate.Month.ToString() + att.AttendancesDate.Day.ToString();
                csv.Append("FH^1^1.0.0^TP^12^2018121719:42:38+0530^45^34173cb38f07f89ddbebc2ac9128303f\n");
                csv.Append("BH^2^TPC^ 00123221\n");
                foreach (var stu in att.Students)
                {
                    var punchInTime = attendancesDate + stu.PunchInTime.Hours + stu.PunchInTime.Minutes + stu.PunchInTime.Seconds;
                    var punchOutTime = attendancesDate + stu.PunchOutTime.Hours + ":" + stu.PunchOutTime.Minutes + ":" + stu.PunchOutTime.Seconds;
                    if (count == 3) csv.Append("BD^" + count + "^ " + att.SelectedBatch.BatchCode + "^A^" + "TRN^" + att.SelectedBatch.Trainer1 + "^20185754178^P^" + attendancesDate + "^" + punchInTime + "0530^" + punchOutTime + "+0530\n");
                    else if (count == 4) csv.Append("BD^" + count + "^ " + att.SelectedBatch.BatchCode + "^A^" + "TRN^" + att.SelectedBatch.Trainer2 + "^20185754178^P^" + attendancesDate + "^" + punchInTime + "0530^" + punchOutTime + "+0530\n");
                    else if (stu.IsPresent) csv.Append("BD^" + count + "^ " + att.SelectedBatch.BatchCode + "^A^CAN^" + stu.CandidateCode + "^P^" + attendancesDate + "^" + punchInTime + "+0530^" + punchOutTime + "+0530\n");
                    else csv.Append("BD^" + count + "^ " + att.SelectedBatch.BatchCode + "^A^CAN^" + stu.CandidateCode + "^A^\n");
                    count++;
                }
                System.IO.File.WriteAllText("D:/StudentAtt.csv", csv.ToString());
                return File(new System.Text.UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "attendances" + attendancesDate + ".csv");
            }
            catch
            {
                return RedirectToAction("Index");
            }
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
                   Convert.ToInt32(random.Next(att.SelectedBatch.StartTime.Hours - 1, att.SelectedBatch.StartTime.Hours)),
                   Convert.ToInt32(random.Next(att.SelectedBatch.StartTime.Minutes - 30, att.SelectedBatch.StartTime.Minutes + 30)),
                   Convert.ToInt32(random.Next(1, 60)));
                s.PunchOutTime = new TimeSpan(
    Convert.ToInt32(random.Next(att.SelectedBatch.EndTime.Hours - 1, att.SelectedBatch.EndTime.Hours)),
    Convert.ToInt32(random.Next(att.SelectedBatch.EndTime.Minutes - 30, att.SelectedBatch.EndTime.Minutes + 10)),
    Convert.ToInt32(random.Next(1, 60)));
            });
            return PartialView(att);
        }
    }

}