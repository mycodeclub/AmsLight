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
        public string TpName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Zip { get; set; }
        public Nullable<DateTime> SubscriptionStartDate { get; set; }
        public Nullable<DateTime> SubscriptionEndtDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public virtual ICollection<TrainingCenter> TrainingCenters { get; set; }
        public bool IsActive { get; set; }

    }
}