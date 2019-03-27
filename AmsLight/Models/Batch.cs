using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AmsLight.Models
{
    [Table("Batch")]
    public class Batch
    {
        [Key]
        public int BatchId { get; set; }


        [Required]
        public string BatchCode { get; set; }

        [Required]
        public int TpId { get; set; }
        public string Center { get; set; }

        [Required]
        [DisplayName("Start Timing")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}", ApplyFormatInEditMode = true)]
        public System.TimeSpan StartTime { get; set; }

        [Required]
        [DisplayName("Start Timing")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}", ApplyFormatInEditMode = true)]
        public System.TimeSpan EndTime { get; set; }

        [Required]
        public string Trainer1 { get; set; }

        [Required]
        public string Trainer2 { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }

        public int TrainingCenterId { get; set; }

        [ForeignKey("TrainingCenterId")]
        public virtual TrainingCenter TrainingCenter { get; set; }

        public virtual List<Student> Students { get; set; }
    }
}