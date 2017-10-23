using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace OBLContactCenter.Models
{
    public class RoleAction
    {

        public long ActionID { get; set; }
        public string ActionName { get; set; }
        public long? MemoTypeID { get; set; }
        public string MemoTypeName { get; set; }
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public bool? IsAssigned { get; set; }


    }
}