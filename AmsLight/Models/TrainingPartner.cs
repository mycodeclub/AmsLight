using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsLight.Models
{
    [Table("TrainingPartner")]
    public class TrainingPartner
    {
        [Key]
        public int TpId { get; set; }
        [Display(Name = "Name")]
        [Required]
        public string TpName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        public string Zip { get; set; }
 
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Last Renewed")]
        public Nullable<DateTime> SubscriptionStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Last Date")]
        public Nullable<DateTime> SubscriptionEndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }
        public virtual ICollection<TrainingCenter> TrainingCenters { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public Login Login { get; set; }

    }
}