using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPractice.ViewModels
{
    public class User_A_RViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime JobStartDate { get; set; }

        public DateTime CreateAt { get; set; }
        public string EmailAddress { get; set; }
        public int YearsWorked { get; set; }
        public int AnnualDayLeft { get; set; }
        public int TypeID { get; set; }
        public string Description { get; set; }
        public int TotalDays { get ; set; }
        public int Status { get; set; }
        public DateTime GoingDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int TotalAnnualDayUsed { get; set; }
        public int AnnualDayUsed { get; set; }
    }
}