<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfReportViewer.aspx.cs" EnableEventValidation="false" Inherits="OBLContactCenter.Report.wfReportViewer" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="36000" EnablePartialRendering="false"></asp:ScriptManager> 
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="600px" ProcessingMode="Remote" AsyncRendering="false" ></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
