using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data;
using Microsoft.VisualBasic;

namespace WSMHRAPI.MasterFunction
{
    public class MasterClass
    {
        public const string FormatDateDB = "Convert(varchar(10),Getdate(),111)";
        public const string FormatTimeDB = "Convert(varchar(8),Getdate(),114)";

        public static bool VerrifyDuplication( string _Table, string PKsysIDName, string FTCode, int _FNHSysCmpId = 0, int FNHSysMasterID = 0)
        {
            try
            {
                bool SetCheckPay = true;
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                string _checkFieldCode = "";
                switch (_Table)
                {

                    case "TCNMCLevel":
                        _checkFieldCode = "FTCLevelCode";
                        break;
                    case "TCNMCmp":
                        _checkFieldCode = "FTCmpCode";
                        break;
                    case "TCNMDivision":
                        _checkFieldCode = "FTDivisonCode";
                        break;
                    case "TCNMDepartment":
                        _checkFieldCode = "FTDeptCode";
                        break;
                    case "TCNMSect":
                        _checkFieldCode = "FTSectCode";
                        break;
                    case "TCNMUnitSect":
                        _checkFieldCode = "FTUnitSectCode";
                        break;
                    case "TCNMPosition":
                        _checkFieldCode = "FTPositCode";
                        break;
                }


                int _N = 0;

                string _Qry = "";
                _Qry =  @"SELECT    COUNT(" + _checkFieldCode + ") As N FROM " +   _Table  + "";
                _Qry += Constants.vbCrLf + "  FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo." + _Table  ;
                _Qry += Constants.vbCrLf + "  WHERE  " + _checkFieldCode + " = '"  + FTCode  + "'";
                if (_FNHSysCmpId > 0)
                {
                    _Qry += Constants.vbCrLf + " AND FNHSYSCMPID = " + _FNHSysCmpId.ToString();
                }
                if (FNHSysMasterID > 0)
                {
                    _Qry += Constants.vbCrLf + " AND " + PKsysIDName + " <> " + FNHSysMasterID.ToString();
                }

                _N = int.Parse(Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER, 0));

                if (_N > 0)
                {
                    return false;
                }
                else {
                    return true;
                }
                //msgCode = "1";
                //msgDesc = "2";
               
            }
            catch (Exception ex)
            {
                //msgCode = "1";
                //msgDesc = "2";
                return false;
            }

        }


        public static bool VerrifyUsing(string _Table, string PKsysIDName, string FTCode, int _FNHSysCmpId = 0, int FNHSysMasterID = 0)
        {
            try
            {
                bool SetCheckPay = true;
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                string _checkFieldCode = "";
                string _checkFieldCodeOrg = "";
                switch (_Table)
                {

                    case "TCNMCLevel":
                        _checkFieldCode = "FTCLevelCode";
                        break;
                    case "TCNMCmp":
                        _checkFieldCode = "FTCmpCode";
                        break;
                    case "TCNMDivision":
                        _checkFieldCode = "FTDivisonCode";
                        break;
                    case "TCNMDepartment":
                        _checkFieldCode = "FTDeptCode";
                        break;
                    case "TCNMSect":
                        _checkFieldCode = "FTSectCode";
                        break;
                    case "TCNMUnitSect":
                        _checkFieldCode = "FTUnitSectCode";
                        break;
                    case "TCNMPosition":
                        _checkFieldCode = "FTPositCode";
                        break;
                }


                int _N = 0;

                string _Qry = "";
                _Qry = @"SELECT    COUNT(*) As N  ";
                _Qry += Constants.vbCrLf + "  FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo.THRMEMPLOYEE  ";
                
                _Qry += Constants.vbCrLf + "  WHERE  " + PKsysIDName + " = '" + FNHSysMasterID + "'";
                if (_FNHSysCmpId > 0)
                {
                    _Qry += Constants.vbCrLf + " AND FNHSYSCMPID = " + _FNHSysCmpId.ToString();
                }
               

                _N = int.Parse(Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER, 0));

                if (_N > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public static bool CreateMaster(string _Table, string FTCode, int _FNHSysCmpId, string  PKsysIDName , string NameTH, string NameEN, string Remark, string username , int _FNEmployeeFormatType, ref  int fnhsysmasterId, ref string msgDesc)
        {
            try
            {
                bool SetCheckPay = true;
                string _Qry = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();


                string CmpCode = "";
                CmpCode = Cnn.GetField("SELECT  FTCmpCode FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo.TCNMCmp WHERE FNHSysCmpId=" + (_FNHSysCmpId), WSM.Conn.DB.DataBaseName.DB_MASTER, "");

                //get sysID
                int sysIID = 0;
                int _FNHSysempId = 0;
               // if (CmpCode != "0") { 
                sysIID = TL.RunID.GetRunNoID(_Table, PKsysIDName, WSM.Conn.DB.DataBaseName.DB_MASTER, CmpCode);
           // }


                // Get FN Seq  Max 
                float FNSeq = 0;
                string _FNSeq = "0";
                _Qry = "SELECT TOP 1  FNSeq +1   FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo." + _Table + " ORDER BY  FNSeq DESC ";
                _FNSeq = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER, "0");

                FNSeq = float.Parse(_FNSeq);

                int _N = 0;
                _Qry = @" INSERT INTO [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo." + _Table;
                _Qry += Constants.vbCrLf + " ( FTInsUser,FDInsDate,FTInsTime,FNSeq,FTStateActive ";

                switch (PKsysIDName)
                {
                   
                    case "FNHSysCLevelId":
                        _Qry += Constants.vbCrLf + " ,FNHSysCLevelId,FTCLevelCode ,FTCLevelNameTH,FTCLevelNameEN, FTRemark ";
                        break;

                    case "FNHSysDeptId":
                        _Qry += Constants.vbCrLf + " ,FNHSysDeptId,FTDeptCode ,FTDeptDescTH,FTDeptDescEN, FTNote, FNHSysCmpId ";
                        break;

                    case "FNHSysDivisonId":
                        _Qry += Constants.vbCrLf + " ,FNHSysDivisonId,FTDivisonCode ,FTDivisonNameTH,FTDivisonNameEN, FTRemark ";
                        break;
                    case "FNHSysSectId":
                        _Qry += Constants.vbCrLf + " ,FNHSysSectId,FTSectCode ,FTSectNameTH,FTSectNameEN, FTRemark, FNHSysCmpId  ";
                        break;
                    case "FNHSysUnitSectId":
                        _Qry += Constants.vbCrLf + " ,FNHSysUnitSectId,FTUnitSectCode ,FTUnitSectNameTH,FTUnitSectNameEN, FTRemark ";
                        break;
                  
                    case "FNHSysPositId":
                             _Qry += Constants.vbCrLf + " ,FNHSysPositId,FTPositCode ,FTPositNameTH,FTPositNameEN, FTRemark, FNHSysCmpId, FNEmployeeFormatType ";
                        break;  
                    case "FNHSysCmpId":
                        _Qry += Constants.vbCrLf + " ,FNHSysCmpId,FTCmpCode ,FTCmpNameTH,FTCmpNameEN, FTRemark ";
                        break;
                }

                _Qry += Constants.vbCrLf + " )";
                _Qry += Constants.vbCrLf + " VALUES ('" + username + "'," + FormatDateDB + "";
                _Qry += Constants.vbCrLf + " ," + FormatTimeDB + "";
                _Qry += Constants.vbCrLf + " ," + FNSeq + "";
                _Qry += Constants.vbCrLf + " ,'1'";
                _Qry += Constants.vbCrLf + " ," + sysIID + "";
                _Qry += Constants.vbCrLf + " ,'" + FTCode + "'"; 
                 _Qry += Constants.vbCrLf + " ,'" + NameTH + "'";
                _Qry += Constants.vbCrLf + " ,'" + NameEN + "'";
                _Qry += Constants.vbCrLf + " ,'" + Remark + "'";

                switch (PKsysIDName)
                {

                    case "FNHSysPositId":
                        _Qry += Constants.vbCrLf + " ," + _FNHSysCmpId + "," + _FNEmployeeFormatType + "";
                        break;
                        
                    case "FNHSysDeptId":
                        _Qry += Constants.vbCrLf + " ," + _FNHSysCmpId + "";
                        break;
                    case "FNHSysSectId":
                        _Qry += Constants.vbCrLf + " ," + _FNHSysCmpId + "";
                        break;
                       
            }
                _Qry += Constants.vbCrLf + " )";
                if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER) == false)
                {

                   // msgCode = "404";
                   // msgDesc = "Can not create Employee.";
                    return false;
                }

                msgDesc = "Already create.";
                fnhsysmasterId = sysIID;
                return true;
            }
            catch (Exception ex)
            {
                //msgCode = "1";
                msgDesc = ex.Message.ToString();
                return false;
            }

        }


        public static bool UpdateMaster(string _Table, string FTCode, int _FNHSysCmpId, string PKsysIDName, string NameTH, string NameEN, string Remark, string username, int _FNEmployeeFormatType,  int fnhsysmasterId, string FtstateActive, ref string msgDesc)
        {
            try
            {
                bool SetCheckPay = true;
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                String checkFieldCode = "";
                String FieldDescTH = "";
                String FieldDescEN = "";
                String FieldRemark = "";
                //TCNMCLevel
                //TCNMCmp
                //TCNMDivision
                //TCNMDepartment
                //TCNMSect
                //TCNMUnitSect
                //TCNMPosition

                switch (_Table)
                {

                    case "TCNMCLevel":
                        checkFieldCode = "FTCLevelCode";
                        FieldDescTH = "FTCLevelNameTH";
                        FieldDescEN = "FTCLevelNameEN";
                        break;
                    case "TCNMCmp":
                        checkFieldCode = "FTCmpCode";
                        FieldDescTH = "FTCmpNameTH";
                        FieldDescEN = "FTCmpNameEN";
                        break;
                    case "TCNMDivision":
                        checkFieldCode = "FTDivisonCode";
                        FieldDescTH = "FTDivisonNameTH";
                        FieldDescEN = "FTDivisonNameEN";
                        break;
                    case "TCNMDepartment":
                        checkFieldCode = "FTDeptCode";
                        FieldDescTH = "FTDeptDescTH";
                        FieldDescEN = "FTDeptDescEN";
                        break;
                    case "TCNMSect":
                        checkFieldCode = "FTSectCode";
                        FieldDescTH = "FTSectNameTH";
                        FieldDescEN = "FTSectNameEN";
                        break;
                    case "TCNMUnitSect":
                        checkFieldCode = "FTUnitSectCode";
                        FieldDescTH = "FTUnitSectNameTH";
                        FieldDescEN = "FTUnitSectNameEN";
                        break;
                    case "TCNMPosition":
                        checkFieldCode = "FTPositCode";
                        FieldDescTH = "FTPositNameTH";
                        FieldDescEN = "FTPositNameEN";
                        break;
                }

                int _N = 0;

                string _Qry = "";
                _Qry = @" UPDATE     [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo." + _Table;
                _Qry += Constants.vbCrLf + " SET ";

                _Qry += Constants.vbCrLf +""+ checkFieldCode  +" = '" + FTCode + "' ";
                _Qry += Constants.vbCrLf +","+ FieldDescTH + " = '" + NameTH + "' ";
                _Qry += Constants.vbCrLf +","+ FieldDescEN + " = '" + NameEN + "' ";
                if (_Table == "TCNMDepartment")
                {
                    _Qry += Constants.vbCrLf + "," + "FTNote " + " = '" + Remark + "' ";
                }
                else
                {
                    _Qry += Constants.vbCrLf + "," + "FTRemark " + " = '" + Remark + "' ";
                }
                _Qry += Constants.vbCrLf + ","+"FTStateActive " + " = '" + FtstateActive + "' ";
                _Qry += Constants.vbCrLf + "," + " FTUpdUser = '" + username + "' ";
                _Qry += Constants.vbCrLf + "," + " FDUpdDate = " + FormatDateDB + " ";
                _Qry += Constants.vbCrLf + "," + " FTUpdTime  = " + FormatTimeDB + " ";
                switch (_Table)
                {

                    case "TCNMPosition":
                        _Qry += Constants.vbCrLf + "," + " FNEmployeeFormatType  = " + _FNEmployeeFormatType + " ";
                        break;
                }

                        _Qry += Constants.vbCrLf + "  WHERE  " + PKsysIDName + " = '" + fnhsysmasterId + "'";
            
                if (_FNHSysCmpId > 0)
                {
                    _Qry += Constants.vbCrLf + "AND FNHSYSCMPID = " + _FNHSysCmpId.ToString();
                }
                if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER) == false)
                {

                    // msgCode = "404";
                    // msgDesc = "Can not create Employee.";
                    return false;
                }
               msgDesc = "Already update.";

                    return true;
               
                //msgCode = "1";
               

            }
            catch (Exception ex)
            {
                //msgCode = "1";
                //msgDesc = "2";
                return false;
            }

        }


        public static bool DeleteMaster(string _Table, string FTCode, int _FNHSysCmpId, string PKsysIDName, string NameTH, string NameEN, string Remark, string username, int fnhsysmasterId, ref string msgDesc)
        {
            try
            {
                bool SetCheckPay = true;
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                String checkFieldCode = "";
                //TCNMCLevel
                //TCNMCmp
                //TCNMDivision
                //TCNMDepartment
                //TCNMSect
                //TCNMUnitSect
                //TCNMPosition

                switch (_Table)
                {

                    case "TCNMCLevel":
                        checkFieldCode = "FTCLevelCode";
                        break;
                    case "TCNMCmp":
                        checkFieldCode = "FTCmpCode";
                        break;
                    case "TCNMDivision":
                        checkFieldCode = "FTDivisonCode";
                        break;
                    case "TCNMDepartment":
                        checkFieldCode = "FTDeptCode";
                        break;
                    case "TCNMSect":
                        checkFieldCode = "FTSectCode";
                        break;
                    case "TCNMUnitSect":
                        checkFieldCode = "FTUnitSectCode";
                        break;
                    case "TCNMPosition":
                        checkFieldCode = "FTPositCode";
                        break;
                }

                int _N = 0;

                string _Qry = "";
                _Qry = @"DELETE    FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo." + _Table;
                _Qry += Constants.vbCrLf + "  WHERE  " + PKsysIDName + " = '" + fnhsysmasterId + "'";
                if (_FNHSysCmpId > 0)
                {
                    _Qry += Constants.vbCrLf + " AND FNHSYSCMPID = " + _FNHSysCmpId.ToString();
                }
                if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER))
                {
                    msgDesc = "Already delete.";
                    return true;
                }
                else
                {
                    return false;
                }

               

            }
            catch (Exception ex)
            {
                //msgCode = "1";
                //msgDesc = "2";
                return false;
            }

        }


    }
}