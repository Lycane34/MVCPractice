using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPractice.ViewModels
{
    public class UserDetailsViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? TypeID { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public DateTime? GoingDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}