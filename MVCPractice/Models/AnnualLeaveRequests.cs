//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCPractice.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AnnualLeaveRequests
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public System.DateTime GoingDate { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatetDate { get; set; }
        public int Status { get; set; }
        public int Days { get; set; }
        public int WorkDays { get; set; }
        public int TotalAnnualDays { get; set; }
        public int AnnualYear { get; set; }
    }
}
