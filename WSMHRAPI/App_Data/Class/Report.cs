using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.CrystalReports.Engine;
using System.Web;

namespace WSM.Conn
{
    public  class Report

    {
        public WSM.RefreshReportJSON CreatPdfReport(string cmpid,string username, string foldername,string fileRpt , string formula ,WSMERPAPI.UFuncs.eLang rptLang =WSMERPAPI.UFuncs.eLang.TH )   {

            WSM.RefreshReportJSON ObjRet = new WSM.RefreshReportJSON() { ExportState = false,Message="",Report="" };
            string reportstring = "";
            try {
                CrystalDecisions.CrystalReports.Engine.ReportDocument FileReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
               
                string guidstring  = Guid.NewGuid().ToString();

                string pathfile = HttpContext.Current.Server.MapPath("~/Reports/" + foldername + "/" + fileRpt );// (Path.Combine(HttpContext.Current.Server.MapPath("~") + "Reports", fileRpt)) & ".rpt";

                FileReport.Load(pathfile);

      
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in FileReport.Database.Tables) {

                        TableLogOnInfo myTableLogonInfo = myTable.LogOnInfo;

                        ConnectionInfo myConnectionInfo = new ConnectionInfo();
                        myConnectionInfo.DatabaseName = myTableLogonInfo.ConnectionInfo.DatabaseName;
                        myConnectionInfo.UserID = WSM.Conn.DB.UserName ;
                        myConnectionInfo.Password = WSM.Conn.DB.UserPassword;
                        myConnectionInfo.ServerName = WSM.Conn.DB.ServerName;

                        myTableLogonInfo.ConnectionInfo = myConnectionInfo;
                        myTable.ApplyLogOnInfo(myTableLogonInfo);

                    };

                   
                    foreach (CrystalDecisions.CrystalReports.Engine.ReportDocument mysubreport in FileReport.Subreports)
                    {


                        foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in mysubreport.Database.Tables)
                        {

                            TableLogOnInfo myTableLogonInfo = myTable.LogOnInfo;

                            ConnectionInfo myConnectionInfo = new ConnectionInfo();
                            myConnectionInfo.DatabaseName = myTableLogonInfo.ConnectionInfo.DatabaseName;
                            myConnectionInfo.UserID = WSM.Conn.DB.UserName;
                            myConnectionInfo.Password = WSM.Conn.DB.UserPassword;
                            myConnectionInfo.ServerName = WSM.Conn.DB.ServerName;

                            myTableLogonInfo.ConnectionInfo = myConnectionInfo;
                            myTable.ApplyLogOnInfo(myTableLogonInfo);

                        };

                    };

                if (FileReport.ParameterFields.Count > 0) {

                    string cmdstring = "";
                    cmdstring = "SELECT  FTParameterName ,";

                    switch (rptLang)
                    {
                        case WSMERPAPI.UFuncs.eLang.TH:
                            cmdstring = cmdstring + " FTLangTH AS FTLang ";
                            break;
                        case WSMERPAPI.UFuncs.eLang.EN:
                            cmdstring = cmdstring + " FTLangEN AS FTLang ";
                            break;
                        case WSMERPAPI.UFuncs.eLang.KM:
                            cmdstring = cmdstring + " FTLangKM AS FTLang ";
                            break;
                        case WSMERPAPI.UFuncs.eLang.VT:
                            cmdstring = cmdstring + " FTLangVT AS FTLang ";
                            break;
                        case WSMERPAPI.UFuncs.eLang.BM:
                            cmdstring = cmdstring + " FTLangBM AS FTLang ";
                            break;
                        case WSMERPAPI.UFuncs.eLang.LAO:
                            cmdstring = cmdstring + " FTLangLAO AS FTLang ";
                            break;
                        case WSMERPAPI.UFuncs.eLang.CH:
                            cmdstring = cmdstring + " FTLangCH AS FTLang ";
                            break;
                        default:
                            cmdstring = cmdstring + " FTLangTH AS FTLang ";
                            break;
                    };

                    cmdstring = cmdstring + " FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_LANG) + "].dbo.HSysReportLanguage WITH(NOLOCK)";
                    cmdstring = cmdstring + " WHERE FTReportName='" + WSMERPAPI.UFuncs.rpQuoted(fileRpt) + "'  ";

                    WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                    System.Data.DataTable dtprm = Cnn.GetDataTable(cmdstring, WSM.Conn.DB.DataBaseName.DB_HR, "Table1");
                    dtprm.Rows.Add("FNLang", rptLang);
                    dtprm.Rows.Add("FNHSysCmpID", cmpid);
                    dtprm.Rows.Add("FTUserLogIn", username);
        
                  
                    for (int i = 0; i <= FileReport.ParameterFields.Count - 1; i++) {
                        string prmaname = FileReport.ParameterFields[i].Name;

                        try {
                            if (dtprm.Select("FTParameterName='" + WSMERPAPI.UFuncs.rpQuoted(FileReport.ParameterFields[i].Name) + "'").Length > 0)
                            {
                                foreach (System.Data.DataRow R in dtprm.Select("FTParameterName='" + WSMERPAPI.UFuncs.rpQuoted(FileReport.ParameterFields[i].Name) + "'"))
                                {

                                    FileReport.SetParameterValue(FileReport.ParameterFields[i].Name, R["FTLang"].ToString());

                                }

                            }
                            else
                            {

                                if (FileReport.ParameterFields[i].Name.Contains("Pm-") == false)
                                {
                                    try {
                                        FileReport.SetParameterValue(FileReport.ParameterFields[i].Name, "");
                                    } catch {
                                        FileReport.SetParameterValue(FileReport.ParameterFields[i].Name, "0");

                                    }
                                 

                                };

                            }

                        }
                        catch(Exception ex2) {
                            string msg = ex2.Message;

                        }
                      
                       
                    };
                    dtprm.Dispose();

                };
         
                    string pathtmp = HttpContext.Current.Server.MapPath("~/ReportsTmp/" + guidstring + ".pdf");

                    if (File.Exists(pathtmp)) {
                    File.Delete(pathtmp);
                }
                   
                    CrystalDecisions.Shared.ExportOptions CrExportOptions;
                    CrystalDecisions.Shared.DiskFileDestinationOptions CrDiskFileDestinationOptions = new CrystalDecisions.Shared.DiskFileDestinationOptions();
                    CrystalDecisions.Shared.PdfRtfWordFormatOptions CrFormatTypeOptions = new CrystalDecisions.Shared.PdfRtfWordFormatOptions();

                    CrDiskFileDestinationOptions.DiskFileName = pathtmp;
                    CrExportOptions = FileReport.ExportOptions;

                    CrExportOptions.ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;

                    if (formula != "") {

                        if ( FileReport.RecordSelectionFormula != "") {

                              FileReport.RecordSelectionFormula = "(" + FileReport.RecordSelectionFormula + " )  AND  " + formula;

                         }else
                        {
                             FileReport.RecordSelectionFormula = formula;
                        };

                     };

                    FileReport.Export();
                    FileReport.Close();

                    reportstring = Convert.ToBase64String(System.IO.File.ReadAllBytes(pathtmp));
                    File.Delete(pathtmp);

                ObjRet.Message ="";
                ObjRet.Report = reportstring;
            } catch(Exception ex) {
                string err = ex.Message;
                ObjRet.Message = ex.Message;
                reportstring = "";
            }

            return ObjRet;
        }
    }
   
}
