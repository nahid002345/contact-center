using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OBLContactCenter.EFnClass;

namespace OBLContactCenter.Models
{
    public class Dashboard
    {
        public string USERID { get; set; }
        public string USERNAME { get; set; }
        public string GROUP { get; set; }
        public string ROLE { get; set; }
        public long UrgentMemo { get; set; }
        public long HourCrossedMemo { get; set; }
        public long ReferMemo { get; set; }
        public long OtherGroupMemo { get; set; }
        public List<CCENUMERATION> NewsTicker { get; set; }
        public List<DashGroupTicket> OtherGrpList { get; set; }
        
    }

    public class DashGroupTicket
    {
        public string GroupName { get; set; }
        public string TotalTicketNo { get; set; }
        public string GroupId { get; set; }
        public string SrchCategory { get; set; }
    }
}