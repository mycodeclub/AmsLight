using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AmsLight.Models
{
    [NotMapped]
    public class CandidateExcel
    {

        [Required]
        [DisplayName("Excel Upload")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }
        public string ValidExcelFormets = @"xls, xlsx";

        public string Message { get; set; }

        public int BatchId { get; set; }


    }
}