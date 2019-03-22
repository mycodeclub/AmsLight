using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AmsLight.Models
{
    [NotMapped]
    public class Attendance
    {
        [Required]
        [Display(Name = "Attendances Date")]
        [DataType(DataType.Date)]

        public DateTime AttendancesDate { get; set; }
        public List<TrainingCenter> TrainingCenters { get; set; }
        public List<Batch> Batches { get; set; }
        public TrainingCenter SelectedTc { get; set; }
        public Batch SelectedBatch { get; set; }
        public List<Student> Students { get; set; }
        private AmsDbContext db = new AmsDbContext();
        int tpId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        public Attendance()
        {
            TrainingCenters = db.TrainingCenters.Where(tc => tc.TpId == tpId).ToList();
            SelectedTc = null;
            Batches = new List<Batch>();
            Students = new List<Student>();
            SelectedBatch = null;
            AttendancesDate = DateTime.Now;

        }
        public Attendance(int tcId)
        {
            TrainingCenters = db.TrainingCenters.Where(tc => tc.TpId == tpId).ToList();
            SelectedTc = db.TrainingCenters.Find(tcId);
            Batches = db.Batches.Where(b => b.TrainingCenterId == tcId).ToList();
            Students = new List<Student>();
            SelectedBatch = null;
            AttendancesDate = DateTime.Now;

        }
        public Attendance(int tcId, int batchId)
        {
            TrainingCenters = db.TrainingCenters.Where(tc => tc.TpId == tpId).ToList();
            SelectedTc = db.TrainingCenters.Find(tcId);
            Batches = db.Batches.Where(b => b.TrainingCenterId == tcId).ToList();
            Students = db.Students.Where(s => s.BatchId == batchId).ToList();
            SelectedBatch = db.Batches.Find(batchId);
            AttendancesDate = DateTime.Now;
        }
    }
}