﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Local_TestEntities1 : DbContext
    {
        public Local_TestEntities1()
            : base("name=Local_TestEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Local_User> Local_User { get; set; }
        public virtual DbSet<Local_User_AnnualLeaveRequests> Local_User_AnnualLeaveRequests { get; set; }
    }
}
