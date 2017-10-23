using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OBLContactCenter.EFnClass;

namespace OBLContactCenter.Models
{
    public class UserInfo
    {
        public long ID { get; set; }
        public string USERID { get; set; }
        public string USERNAME { get; set; }
        public string GROUP { get; set; }
        public string ROLE { get; set; }
        public string IMAGEPATH { get; set; }
        
        public List<CCMEMOMASTER> PendingTaskList { get; set; }
        public List<CCMEMOMASTER> PriorityTaskList { get; set; }

    }
}