﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OBLContactCenter.EFnClass
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OBLCONTACTCENTEREntities : DbContext
    {
        public OBLCONTACTCENTEREntities()
            : base("name=OBLCONTACTCENTEREntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CCACTIONMAP> CCACTIONMAPs { get; set; }
        public DbSet<CCCALLCATAGORY> CCCALLCATAGORies { get; set; }
        public DbSet<CCCALLTYPE> CCCALLTYPEs { get; set; }
        public DbSet<CCENUMERATION> CCENUMERATIONs { get; set; }
        public DbSet<CCEVENTLOG> CCEVENTLOGs { get; set; }
        public DbSet<CCGROUP> CCGROUPs { get; set; }
        public DbSet<CCMEMOATTACHMENT> CCMEMOATTACHMENTs { get; set; }
        public DbSet<CCMEMOLOG> CCMEMOLOGs { get; set; }
        public DbSet<CCMEMOMASTER> CCMEMOMASTERs { get; set; }
        public DbSet<CCMENU> CCMENUs { get; set; }
        public DbSet<CCROLE> CCROLEs { get; set; }
        public DbSet<CCROLEMENU> CCROLEMENUs { get; set; }
        public DbSet<CCUSER> CCUSERs { get; set; }
        public DbSet<TblAudit> TblAudits { get; set; }
        public DbSet<CCACTIONGROUPMAP> CCACTIONGROUPMAPs { get; set; }
    }
}
