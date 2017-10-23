using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OBLContactCenter.Models
{
    public class RoleMenu
    {

        public long ID { get; set; }
        public string MenuName { get; set; }
        public bool? IsParent { get; set; }
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public bool? IsAssigned { get; set; }
    }
}