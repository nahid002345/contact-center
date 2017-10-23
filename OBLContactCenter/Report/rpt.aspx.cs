using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBLContactCenter.Views.Report
{
    public partial class rpt : System.Web.UI.Page
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

                // not use FormsCredentials unless you have implements acustom autentication.
                authCookie = null;
                user = password = authority = null;
                return false;
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            String rName = "";
            if (!IsPostBack)
            {
                //lblRptName.Text = "OBL Staff Leave Application Report";
                IReportServerCredentials irsc = new CustomReportCredentials("Administrator", "@dm1nP@ss", "");
                //ReportViewer1.ServerReport.ReportServerCredentials = irsc;


                ////rName = Request.QueryString["rptName"].ToString();
                ////rName = rName.Trim();

                //ReportViewer1.ServerReport.ReportServerUrl = new Uri(@"http://10.156.0.105/ReportServer");

                //ReportViewer1.ServerReport.ReportPath = "/RptOBLContactCenter/RptDetailReport";// + rName;
                //ReportViewer1.ShowToolBar = true;
                //ReportViewer1.Visible = true;

                //if (Request.QueryString["showName"].ToString() != String.Empty && Request.QueryString["showName"].ToString() != "")
                //{
                //    if (Request.QueryString["showName"].ToString().ToLower() == "staff leave application report")
                //    {
                //        Microsoft.Reporting.WebForms.ReportParameter[] parameter = new Microsoft.Reporting.WebForms.ReportParameter[2];
                //        parameter[0] = new Microsoft.Reporting.WebForms.ReportParameter();
                //        parameter[0].Name = "EmployeeID";
                //        parameter[0].Visible = false;
                //        parameter[0].Values.Add(Request.QueryString["EmployeeID"].ToString());

                //        parameter[1] = new Microsoft.Reporting.WebForms.ReportParameter();
                //        parameter[1].Name = "CaseID";
                //        parameter[1].Visible = false;
                //        parameter[1].Values.Add(Request.QueryString["CaseID"].ToString());

                //        ReportViewer1.ServerReport.SetParameters(parameter);
                //        //ReportViewer1.LocalReport.SetParameters(parameter);
                //    }

                //    else if (Request.QueryString["showName"].ToString().ToLower() == "quiz report")
                //    {
                //    }
                //    else if (Request.QueryString["showName"].ToString().ToLower() == "branch unauthorize list")
                //    {
                //        Microsoft.Reporting.WebForms.ReportParameter[] parameter = new Microsoft.Reporting.WebForms.ReportParameter[1];
                //    }
                //    else if (Request.QueryString["showName"].ToString().ToLower() == "dept unauthorize List")
                //    {
                //        // Microsoft.Reporting.WebForms.ReportParameter[] parameter = new Microsoft.Reporting.WebForms.ReportParameter[1];
                //    }
                //    else if (Request.QueryString["showName"].ToString().ToLower() == "south zone employee total marks")
                //    {
                //        // Microsoft.Reporting.WebForms.ReportParameter[] parameter = new Microsoft.Reporting.WebForms.ReportParameter[1];
                //    }
                //    else if (Request.QueryString["showName"].ToString().ToLower() == "south zone employee particular marks")
                //    {
                //        // Microsoft.Reporting.WebForms.ReportParameter[] parameter = new Microsoft.Reporting.WebForms.ReportParameter[1];
                //    }
                //    //strTitle = "Report Preview Of " + Request.QueryString["showName"].ToString() + ".";
                //}
            }
            

        }
    }
}