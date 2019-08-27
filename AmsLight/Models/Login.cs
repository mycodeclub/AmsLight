using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AmsLight.Models
{
    [Table("Login")]
    public class Login
    {
        [Key]
        public int LoginId { get; set; }
        public int TypeId { get; set; }
        public int TpId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [NotMapped]
        public string RememberMe { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [NotMapped]
        public virtual TrainingPartner TrainingPartner { get; set; }
    }


}