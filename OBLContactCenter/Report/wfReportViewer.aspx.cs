using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OBLContactCenter.Report
{
    public partial class wfReportViewer : System.Web.UI.Page
    {
        public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
        {

            // local variable fornetwork credential.
            private string _UserName;
            private string _PassWord;
            private string _DomainName;
            public CustomReportCredentials(string UserName, string PassWord, string DomainName)
            {
                _UserName = UserName;
                _PassWord = PassWord;
                _DomainName = DomainName;
            }
            public WindowsIdentity ImpersonationUser
            {
                get
                {
                    return null; // not use ImpersonationUser
                }
            }
            public ICredentials NetworkCredentials
            {
                get
                {

                    // use NetworkCredentials
                    return new NetworkCredential(_UserName, _PassWord, _DomainName);
                }
            }
            public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
            {

                // not use FormsCredentials unless you have implements a custom autentication.
                authCookie = null;
                user = password = authority = null;
                return false;
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(!string.IsNullOrEmpty(Request.QueryString["PATH"]))
                    ShowRpt(Request.QueryString["PATH"].ToString());
            }
        }

        protected void ShowRpt(string rptPath)
        {
            IReportServerCredentials irsc = new CustomReportCredentials("Administrator", "Asdf123", "");
            this.ReportViewer1.ServerReport.ReportServerCredentials = irsc;

            this.ReportViewer1.ServerReport.ReportServerUrl = new Uri(@"http://10.156.0.131/ReportServer");
            this.ReportViewer1.ServerReport.ReportPath = rptPath;
            this.ReportViewer1.ServerReport.Refresh();
            
        }
    }
}