using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace OBLContactCenter.Models
{
    public class ReportingServicesReportViewModel
    {
        
        public ReportingServicesReportViewModel(String reportPath,List<ReportParameter> Parameters)
        {
            ReportPath = reportPath;
            parameters = Parameters.ToArray();
        }
        public ReportingServicesReportViewModel()
        {         
  
        }
        

        
        public ReportServerCredentials ServerCredentials { get { return new ReportServerCredentials(); } }
        public String ReportPath { get; set; }
        public Uri ReportServerURL { get { return new Uri(WebConfigurationManager.AppSettings["ReportServerUrl"]); } }
        public ReportParameter[] parameters { get; set; }
        private string UploadDirectory = HttpContext.Current.Server.MapPath("~/App_Data/UploadTemp/");
        private string TempDirectory = HttpContext.Current.Server.MapPath("~/tempFiles/");
        
    }

    [Serializable]
    public sealed class ReportServerCredentials : IReportServerConnection2//IReportServerCredentials
    {

        #region Private Properties
        private string _username;
        private string _password;
        private string _domain;
        #endregion Private Properties

        #region Public Properties
        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get { return new NetworkCredential(_username, _password, _domain); }
        }
        #endregion Public Properties

        #region Constructor
        public ReportServerCredentials(string userName, string password, string domain)
        {
            _username = userName;
            _password = password;
            _domain = domain;
        }
        public ReportServerCredentials()
        {
            var appSetting = WebConfigurationManager.AppSettings;
            _username = appSetting["ReportServerUser"];
            _password = appSetting["ReportServerPassword"];
            _domain = appSetting["ReportServerDomain"];
        }
        #endregion Constructor

        #region Public Method
        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;
            return false;
        }
        #endregion Public Method


        public IEnumerable<Cookie> Cookies { get { return null; } }


        public IEnumerable<string> Headers { get { return null; } }

        public Uri ReportServerUrl { get { return new Uri(WebConfigurationManager.AppSettings["ReportServerUrl"]); } }


        public int Timeout { get { return 60000; } }
    }
}