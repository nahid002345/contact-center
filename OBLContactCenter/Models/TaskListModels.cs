using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OBLContactCenter.EFnClass;

namespace OBLContactCenter.Models
{
    public class TaskList
    {

        public long USERID { get; set; }
        public long USERGROUPID { get; set; }
        public List<CCMEMOMASTER> CCMEMOLIST { get; set; }
        public int? TASKTYPE { get; set; }
        public string TASKTYPENAME { get; set; }

        public List<CCMEMOMASTER> CATEGORIGEDMEMOLIST { get; set; }
    }
    public class TASKOCCUPIEDINFO
    {
        public string UserId { get; set; }
        public string OccupiedBy { get; set; }
        public string BrowserId { get; set; }
        public string TokenType { get; set; }
        public string TokenId { get; set; }
        public DateTime TaskTakenDate { get; set; }
    }
    public static class TASKTAKENList
    {
        public static List<TASKOCCUPIEDINFO> oTaskTakenList = new List<TASKOCCUPIEDINFO>();
    }
    public class TaskInfo
    {
        public CCMEMOMASTER CCMEMO { get; set; }
        public TASKOCCUPIEDINFO TaskOccupiedBy { get; set; }
        public bool IsTaskBusy { get; set; }
        //public 
    }
}