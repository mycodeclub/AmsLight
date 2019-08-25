using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AmsLight.Models
{
    [Table("Student")]
    public partial class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public int TpId { get; set; }
        [Required]
        [DisplayName("Candidate Code")]
        public string CandidateCode { get; set; }
        public virtual Batch Batch { get; set; }


        [DisplayName("Punch Out Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}", ApplyFormatInEditMode = true)]
        [NotMapped]
        public TimeSpan PunchInTime { get; set; }

        [DisplayName("Punch Out Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}", ApplyFormatInEditMode = true)]
        [NotMapped]
        public TimeSpan PunchOutTime { get; set; }

        [Required]
        [NotMapped]

        public bool IsPresent { get; set; }

        [DisplayName("Father Name")]
        public string Father_Husband_Name { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }
        public string MobileNo { get; set; }
        public string AadhaarId { get; set; }

    }
}