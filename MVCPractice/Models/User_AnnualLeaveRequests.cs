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
    
    public partial class User_AnnualLeaveRequests
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int TypeID { get; set; }
        public int AnnualYear { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime GoingDate { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public int TotalDay { get; set; }
        public int TotalWorkDay { get; set; }
        public string Description { get; set; }
        public int ManagerID { get; set; }
        public System.DateTime ManagerDate { get; set; }
        public int DirectorID { get; set; }
        public System.DateTime DirectorDate { get; set; }
        public int AdminID { get; set; }
        public System.DateTime AdminDate { get; set; }
        public bool IsCancel { get; set; }
        public string CancelMessage { get; set; }
        public int CancelUserID { get; set; }
        public System.DateTime CancelDate { get; set; }
        public int Status { get; set; }
        public Nullable<bool> IsForm { get; set; }
    }
}
