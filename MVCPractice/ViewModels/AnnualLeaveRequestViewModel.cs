using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPractice.ViewModels
{
    public class AnnualLeaveRequestViewModel
    {
        [Required(ErrorMessage ="Gidiş Tarihi Girin.")]
        public DateTime RequestGoingDate { get; set; }
        [Required(ErrorMessage ="Dönüş Tarihi Girin.")]
        public DateTime RequestReturnDate { get; set; }
        [Required(ErrorMessage ="Açıklama Girin.")]

        public string RequestDescription { get; set; }
    }
}