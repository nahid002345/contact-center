//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class CCACTIONMAP
    {
        public long ID { get; set; }
        public long ACTIONID { get; set; }
        public Nullable<long> ROLEID { get; set; }
        public Nullable<long> MEMOTYPEID { get; set; }
        public Nullable<bool> ISACTIVE { get; set; }
        public string CREATEDBY { get; set; }
        public System.DateTime CREATEDON { get; set; }
    
        public virtual CCENUMERATION CCENUMERATION { get; set; }
        public virtual CCROLE CCROLE { get; set; }
    }
}
