using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AmsLight.Models
{
    [Table("AttendanceLog")]
    public class AttendanceLog
    {
        [Key]
        public Int64 AttendanceId { get; set; }
        public int batchId { get; set; }
        public string AttendanceCsv { get; set; }
        public string ObjJson { get; set; }
        public DateTime AttendancesDate { get; set; }
        public DateTime CreateDate { get; set; }
        
    }
}