using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OBLContactCenter.EFnClass;
using OBLContactCenter.Models;

namespace OBLContactCenter.Controllers
{
    
    public class AppBaseController : Controller
    {

        
        //
        // GET: /AppBase/
        public OBLCONTACTCENTEREntities oDBContext = new OBLCONTACTCENTEREntities();
        public CCUSER oCurrentUser = null;
        public static List<TASKOCCUPIEDINFO> oTaskTakenList;
        public int ExpiredTimeoutToken = 15;
        public string ExceptionMsg(Exception ex)
        {
            return (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
        }

        public bool IsTokenBusy(string TId)
        {
            bool result = false;
            if (oTaskTakenList != null && oTaskTakenList.Exists(t => t.TokenId == TId))
                result = true;
            return result;
        }

        public void RemoveOverTimeOccupiedToken()
        {
            if (oTaskTakenList != null)
            oTaskTakenList.RemoveAll(t => (DateTime.Now - t.TaskTakenDate).Minutes > ExpiredTimeoutToken);
        }

        public void RemoveOccupiedTokenByUIdBrowserId(string UId,string BrId)
        {
            if (oTaskTakenList != null)
            oTaskTakenList.RemoveAll(t => t.UserId == UId && t.BrowserId == BrId);
        }

        public void RemoveOccupiedTokenByUId(string UId)
        {
            if (oTaskTakenList != null)
                oTaskTakenList.RemoveAll(t => t.UserId == UId );
        }

        public void RemoveOccupiedTokenByUIdTokenId(string UId,string TId)
        {
            if (oTaskTakenList != null)
                oTaskTakenList.RemoveAll(t => t.UserId == UId && t.TokenId == TId);
        }
        public void AddOccupiedTask(CCUSER UId, string TId)
        {
            TASKOCCUPIEDINFO oTASKOCCUPIEDINFO = new TASKOCCUPIEDINFO();
            oTASKOCCUPIEDINFO.UserId = UId.USERID;
            oTASKOCCUPIEDINFO.OccupiedBy = UId.EMPLOYEENAME;
            oTASKOCCUPIEDINFO.BrowserId = Request.Browser.Id.ToString();
            oTASKOCCUPIEDINFO.TaskTakenDate = DateTime.Now;
            oTASKOCCUPIEDINFO.TokenId = TId;
            oTASKOCCUPIEDINFO.TokenType = "task";
            if (oTaskTakenList == null) oTaskTakenList = new List<TASKOCCUPIEDINFO>();
            oTaskTakenList.Add(oTASKOCCUPIEDINFO);
        }

        public bool CheckOccupiedTokenByUIdTokenId(string UId, string TId)
        {
            bool result = false;
            if (oTaskTakenList != null)
                result=oTaskTakenList.Exists(t => t.UserId == UId && t.TokenId == TId);

            return result;
        }
    }
}
