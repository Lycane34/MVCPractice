using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPractice.ViewModels
{
    public class ConfirmRequestViewModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public DateTime GoingDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public int Days { get; set; }
        public int WorkDays { get; set; }
        public int AnnualYears { get; set; }
    }
}