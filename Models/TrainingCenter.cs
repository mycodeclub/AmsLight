using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsLight.Models
{
    [Table("TrainingCenter")]
    public class TrainingCenter
    {
        [Key]
        public int TrainingCenterId { get; set; }
        [Required]
        public int TpId { get; set; }
        [Required]
        public string CenterCode { get; set; }
        public string CenterName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Zip { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Batch> Batches { get; set; }
    }
}