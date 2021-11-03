using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data;
using Microsoft.VisualBasic;


namespace WSMHRAPI.HRFunction
{
    public class HRClass
    {

        public const string FormatDateDB = "Convert(varchar(10),Getdate(),111)";
        public const string FormatTimeDB = "Convert(varchar(8),Getdate(),114)";

        public static bool CreateLeave(string _FNHSysempId, string _FNHSysWorkShift, string LeaveTypeId, int LeaveDayState, string LeaveMethod, int LeaveMinutes, string StartDate, string Enddate, string StartTime, string EndTime, string Remark, byte[] attFile, string ExtentionFile, ref string msgCode, ref string msgDesc, ref int NextApprovalID)
        {
            try
            {



                string _Qry = "";

                if (VerrifyLeave(_FNHSysempId, _FNHSysWorkShift, LeaveTypeId, LeaveMethod, StartDate, Enddate, StartTime, EndTime, ref msgCode, ref msgDesc))
                {

                    if (LeaveTypeId == "98")
                    {
                        //if (VacationLeave_Check(_FNHSysempId, _FNHSysWorkShift, LeaveTypeId, LeaveMethod, StartDate, Enddate, ref msgCode, ref msgDesc))
                        //{
                        //"คุณมีวันพักร้อนเหลือ ต้องการใช้วันพักร้อนแทนวันลากิจหรือไม่"
                        //}
                    }

                    Boolean _checkpay = false;

                    string FTStateNotMergeHoliday = "0";
                    string FTStateLeavepay = "0";
                    string FTStateCalSSo = "0";
                    string FTStateMedicalCertificate = "0";
                    string FTStateDeductVacation = "0";

                    if (Checkpay(LeaveTypeId, _FNHSysempId, StartDate, Enddate, StartTime, EndTime,  LeaveMethod,  LeaveMinutes, ref FTStateNotMergeHoliday,  ref FTStateLeavepay, ref FTStateCalSSo, ref FTStateMedicalCertificate, ref FTStateDeductVacation))
                    {
                        
                    }
                    else
                    {
                        return false;
                        //"พบข้อมูลการลาเกินกำหนด !!!"
                        msgCode = "";
                        msgDesc = "พบข้อมูลการลาเกินกำหนด !!!";
                    }

                    //ตรวจสอบการรูดบัตร


                    //คุณต้องการบันทึกการลาใช่หรือไม่


                    //check total time 


                    // Save
                    if (SaveDataLeave(LeaveTypeId, _FNHSysempId, StartDate, Enddate, StartTime, EndTime, Remark, attFile, ExtentionFile, LeaveDayState, LeaveMethod, LeaveMinutes,  FTStateNotMergeHoliday,  FTStateLeavepay,  FTStateCalSSo,  FTStateMedicalCertificate,  FTStateDeductVacation, ref  NextApprovalID))
                    {

                        msgCode = "";
                        msgDesc = "บันทึกการลาเรียบร้อยแล้ว!!!";
                      
                        return true;
                    }
                    else
                    {

                        msgCode = "";
                        msgDesc = "ไม่สามารถบันทึกการลาได้ !!!";
                        NextApprovalID = 0;
                        return false;
                    }

                }
                else
                {

                    //msgCode = "";
                    //msgDesc = "พบข้อมูลการลาเกินกำหนด !!!";
                    NextApprovalID = 0;
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;

                msgDesc = ex.Message.ToString();
            }


        }



        public static bool VerrifyLeave(string _FNHSysempId, string _FNHSysWorkShift, string LeaveTypeId, string LeaveMethod, string StartDate, string Enddate, string StartTime, string EndTime, ref string msgCode, ref string msgDesc)
        {

            try
            {
                if (CheckDate(StartDate) != "" && CheckDate(Enddate) != "")
                {
                    string _sdate = ConvertEnDB(StartDate); //onvertEnDB(StartDate);

                    string _edate = ConvertEnDB(Enddate);


                    if ((UDayDiff(DateInterval.Year, _sdate, _edate) == 0 && LeaveTypeId == "98") || LeaveTypeId != "98")
                    {
                        return true;
                    }
                    else
                    {
                        msgDesc = "ไม่สามารถลาข้ามปีได้ กรุณาแบ่งการลา !!!";
                        return false;
                    }

                }
                else
                {

                    msgDesc = "กรุณาทำการระบุข้อมูลเวลา!!!";
                    return false;

                }


            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool VacationBalanceLeave_Check(string _FNHSysempId, string _FNHSysWorkShift, string LeaveTypeId, string LeaveMethod, string StartDate, string Enddate, string StartTime, string EndTime, ref string msgCode, ref string msgDesc)
        {
            //check ลากิจ 
            return false;
        }

  

        private static bool Checkpay(string leavekey, string _FNHSysempId, string StartDate, string Enddate, string StartTime, string EndTime, string LeaveMethod, int LeaveMinutes, ref string FTStateNotMergeHoliday, ref string FTStateLeavepay, ref string FTStateCalSSo, ref string FTStateMedicalCertificate, ref string FTStateDeductVacation)
        {
            bool SetCheckPay = true;
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            string _Qry;
            DataTable _dt;
            string _DateReset;
            string _MsgRet = "";
            string _Msg = "";
            double _Leave = 0;
            double _LeavePay = 0;
            double _GLeave = 0;
            double _GLeavePay = 0;
            int _Month = 0;
            string _FTStaHoliday = "0";

            int TotalLeave = 0;

            string _CalType = "";
            _Qry = " SELECT  ET.FNCalType ";
            _Qry += Constants.vbCrLf + " FROM            THRMEmployee AS M INNER JOIN";
            _Qry += Constants.vbCrLf + "    [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo.THRMEmpType AS ET ON M.FNHSysEmpTypeId = ET.FNHSysEmpTypeId";
            _Qry += Constants.vbCrLf + " WHERE (M.FNHSysEmpID =" + _FNHSysempId + ") ";

            _CalType = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

            if (UDayDiff(DateInterval.Year, StartDate.ToString(), Enddate.ToString()) > 0)
            {
                _Qry = " SELECT CASE WHEN RiGHT(FTCurrenDate,5) >=FTLeaveReset THEN LEFT(FTCurrenDate,4) ELSE  LEFT(FTBefore,4)  END +'/' + FTLeaveReset";
                _Qry += Constants.vbCrLf + "  FROM";
                _Qry += Constants.vbCrLf + " (";
                _Qry += Constants.vbCrLf + " SELECT  TOP 1 Convert(varchar(10),DateAdd(YEAR,1,GetDate()),111)  AS FTCurrenDate ,Convert(varchar(10),DateAdd(YEAR,-1,GetDate()),111) AS FTBefore,L.FTLeaveReset";
                _Qry += Constants.vbCrLf + " FROM            THRMConfigLeave  AS L WITH (NOLOCK)  INNER JOIN THRMEmployee AS M WITH(NOLOCK )";
                _Qry += Constants.vbCrLf + "  ON  L.FNHSysEmpTypeId=M.FNHSysEmpTypeId";
                _Qry += Constants.vbCrLf + "  WHERE   M.FNHSysEmpID=" + _FNHSysempId + " ";
                _Qry += Constants.vbCrLf + " ) As T";

            }
            else
            {
                _Qry = " SELECT CASE WHEN RiGHT(FTCurrenDate,5) >=FTLeaveReset THEN LEFT(FTCurrenDate,4) ELSE  LEFT(FTBefore,4)  END +'/' + FTLeaveReset";
                _Qry += Constants.vbCrLf + "  FROM";
                _Qry += Constants.vbCrLf + " (";
                _Qry += Constants.vbCrLf + " SELECT  TOP 1 Convert(varchar(10),GetDate(),111)  AS FTCurrenDate ,Convert(varchar(10),DateAdd(YEAR,-1,GetDate()),111) AS FTBefore,L.FTLeaveReset";
                _Qry += Constants.vbCrLf + " FROM            THRMConfigLeave  AS L WITH (NOLOCK)  INNER JOIN THRMEmployee AS M WITH(NOLOCK )";
                _Qry += Constants.vbCrLf + "  ON  L.FNHSysEmpTypeId=M.FNHSysEmpTypeId";
                _Qry += Constants.vbCrLf + "  WHERE   M.FNHSysEmpID=" + _FNHSysempId + " ";
                _Qry += Constants.vbCrLf + " ) As T";
            }
            _DateReset = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

            string _FTStateDeductVacation = "";

            _Qry = @" SELECT TOP 1   L.FTLeaveReset, 
                      L.FNLeaveRight, L.FNLeavePay,ISNULL(L.FTStaHoliday,'') AS FTStaHoliday,ISNULL(L.FTStateDeductVacation,'0') AS FTStateDeductVacation 
                     FROM            THRMConfigLeave  AS L WITH (NOLOCK)  INNER JOIN THRMEmployee AS M WITH(NOLOCK )
                      ON  L.FNHSysEmpTypeId=M.FNHSysEmpTypeId
                      WHERE  M.FNHSysEmpID=" + _FNHSysempId + @" 
                      AND L.FTLeaveCode='" + leavekey + "'";

            _dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR);


            foreach (DataRow R in _dt.Rows)
            {
                _Leave = Double.Parse(R["FNLeaveRight"].ToString());
                _LeavePay = Double.Parse(R["FNLeavePay"].ToString());
                _FTStaHoliday = R["FTStaHoliday"].ToString();
                _FTStateDeductVacation = R["FTStateDeductVacation"].ToString();
            }
            string cmd = "";
            if (leavekey == "98")
            {

                string VacationLeaveType = "";
                cmd = " SELECT TOP 1 FTCfgData";
                cmd += Constants.vbCrLf + "  FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SECURITY) + "].dbo.TSESystemConfig AS Z WITH(NOLOCK) ";
                cmd += Constants.vbCrLf + " WHERE  (FTCfgName = N'VacationLeaveType')";

                VacationLeaveType = Cnn.GetField(cmd, WSM.Conn.DB.DataBaseName.DB_SECURITY, "0");

                if (VacationLeaveType == "1")
                {
                    cmd = "   SELECT  TOP 1  dbo.FN_Get_Emp_Vacation_Th(FNHSysEmpID,FNHSysEmpTypeId,ISNULL(FDDateStart,''),ISNULL(FDDateEnd,''),ISNULL(FDDateProbation,'')" + ",'" + ConvertEnDB(Enddate) + "'" + ") AS FNEmpVacation";
                    cmd += Constants.vbCrLf + "   FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee  AS M WITH(NOLOCK)";
                    cmd += Constants.vbCrLf + "  WHERE  M.FNHSysEmpID=" + _FNHSysempId + " ";
                }
                else
                {
                    cmd = "   SELECT  TOP 1  dbo.FN_Get_Emp_Vacation(FNHSysEmpID,FNHSysEmpTypeId,ISNULL(FDDateStart,''),ISNULL(FDDateEnd,''),ISNULL(FDDateProbation,'')) AS FNEmpVacation";
                    cmd += Constants.vbCrLf + "   FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee  AS M WITH(NOLOCK)";
                    cmd += Constants.vbCrLf + "  WHERE  M.FNHSysEmpID=" + _FNHSysempId + " ";
                }

                _Leave = Double.Parse(Cnn.GetField(cmd, WSM.Conn.DB.DataBaseName.DB_HR, "0"));
                _LeavePay = _Leave;
            }

            _Leave = _Leave * 480;
            _LeavePay = _LeavePay * 480;
            _GLeave = 0;
            _GLeavePay = 0;

            _Qry = "  SELECT        SUM(FNTotalMinute) AS FNTotalMinute,Sum(FNTotalPayMinute) As FNTotalPayMinute ";
            _Qry += Constants.vbCrLf + "   FROM THRTTransLeave WITH (NOLOCK)";
            _Qry += Constants.vbCrLf + " WHERE (FNHSysEmpID = " + _FNHSysempId + ") ";
            _Qry += Constants.vbCrLf + " AND (FTDateTrans >= N'" + _DateReset + "')";
            _Qry += Constants.vbCrLf + " AND (FTLeaveType = '" + leavekey + "')";
            _Qry += Constants.vbCrLf + " AND FTDateTrans <'" + ConvertEnDB(StartDate) + "' ";

            if (leavekey == "0")
            {
                _Qry += Constants.vbCrLf + " AND (ISNULL(FNLeaveSickType,0) = 1  OR  (ISNULL(FNLeaveSickType,0)=0 AND FNTotalPayMinute >0 ))  ";
            }


            _dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR);


            if (_dt.Rows.Count > 0)
            {
                foreach (DataRow dr in _dt.Rows)
                {
                    if (dr["FNTotalMinute"].ToString() != "") { _GLeave = double.Parse(dr["FNTotalMinute"].ToString()); }
                    if (dr["FNTotalPayMinute"].ToString() != "") { _GLeavePay = double.Parse(dr["FNTotalPayMinute"].ToString()); }
                }
            }


            if (leavekey == "98")
            {
                _Qry = "Select   SUM(FNLeaveTotalDay * FNLeaveTotalTimeMin) AS FNTotalMinute ";
                _Qry += Constants.vbCrLf + "   FROM THRTLeaveAdvanceDaily WITH (NOLOCK)";
                _Qry += Constants.vbCrLf + " WHERE (FNHSysEmpID = " + _FNHSysempId + ") ";
                _Qry += Constants.vbCrLf + " AND (FTStartDate >= N'" + _DateReset + "')";
                _Qry += Constants.vbCrLf + " AND FTLeaveType='98'";
                _Qry += Constants.vbCrLf + " OR (FTLeaveType <> '98' and  isnull(FTStateDeductVacation,'0' ) = '1'  and   FTStartDate >= N'" + _DateReset + "' and FNHSysEmpID = " + _FNHSysempId + ")";

                _dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR);
                double totalmin = 0;
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in _dt.Rows)
                    {
                        if (dr["FNTotalMinute"].ToString() != "") { totalmin = double.Parse(dr["FNTotalMinute"].ToString()); }

                        _Leave = _Leave - totalmin;
                    }
                }
            }

            if (leavekey == "98")
            {
                TotalLeave = int.Parse(_Leave.ToString());
            }
            else
            {
                if (SetCheckPay)
                {
                    if (_Leave < _GLeave)
                    {
                        return false;
                        //"พบข้อมูลการลาเกินกำหนด !!!";
                    }
                    TotalLeave = int.Parse((_Leave - _GLeave).ToString());
                }
            }


            if (TotalLeave <= 0)
            {
                TotalLeave = 0;
            }


            try
            {
                if (leavekey == "97")
                {

                }
                else
                {
                    // _GLeave = _GLeave + (FTNetDay.Value * ocetotaltime.Value)
                    _GLeave = _GLeave + (int.Parse(LeaveMethod) * LeaveMinutes);
                }
            }
            catch (Exception ex)
            {
            }

            Boolean _FTStateLeavepay = true;
            Boolean _FTStateNotMergeHoliday = true;
            Boolean _FTStateCalSSo = true;

            //FTStateLeavepay.Enabled = True
            if (SetCheckPay)
            {
                _FTStateLeavepay = false;

                if (ChkPayLeave(leavekey, _FNHSysempId))
                {
                    //If(FNLeaveDay.SelectedIndex <> 3 And(_LeavePay >= _GLeavePay + (FTNetDay.Value * ocetotaltime.Value))) Then
                    //    FTStateLeavepay.Checked = (_LeavePay >= _GLeavePay + (FTNetDay.Value * ocetotaltime.Value))
                    //Else
                    _FTStateLeavepay = (_LeavePay >= _GLeave);
                }
                else
                {
                    _FTStateLeavepay = false;
                }
            }

            _FTStateNotMergeHoliday = (_FTStaHoliday == "1") || (leavekey == "98");
            _FTStateCalSSo = _FTStateLeavepay;

             if (_FTStateNotMergeHoliday) { FTStateNotMergeHoliday="1"; } else { FTStateNotMergeHoliday = "0"; }
            if (_FTStateLeavepay) { FTStateLeavepay = "1"; } else { FTStateLeavepay = "0"; }
            if (_FTStateCalSSo) { FTStateCalSSo = "1"; } else { FTStateCalSSo = "0"; }

            FTStateDeductVacation = _FTStateDeductVacation;
            


            return true;

        }

        private static bool ChkPayLeave(string leavekey, string _FNHSysempId)
        {
            try
            {
                string _Qry = "";
                string FNLeavePay = "";

                if (leavekey == "98")
                {
                    return true;
                }
                else
                {
                    WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                    _Qry = "SELECT Top 1  L.FNLeavePay FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMConfigLeave AS L WITH(NOLOCK) INNER JOIN   ";
                    _Qry += Constants.vbCrLf + " [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee AS E WITH(NOLOCK) ON L.FNHSysEmpTypeId = E.FNHSysEmpTypeId ";
                    _Qry += Constants.vbCrLf + "  WHERE E.FNHSysEmpID=" + _FNHSysempId + " ";
                    _Qry += Constants.vbCrLf + " and FTLeaveCode = '" + leavekey + "'";

                    FNLeavePay = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "0");


                    if (FNLeavePay == "0")
                    {
                        return false;

                    }
                    else
                    {
                        return true;
                    }

                    // Return Val(WSM.Conn.SQLConn.GetField(_Qry, Conn.DB.DataBaseName.DB_HR, "0")) > 0

                }



            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool VerrifyDay(string _FNHSysempId, string LeaveTypeId, string StartDate, string Enddate, ref string msgCode, ref string msgDesc, ref int FTNetDay)
        {
            try
            {
                string _Qry = "";
                DataTable _dt = new DataTable();

                DataTable _dtHoliday = new DataTable();
                DataTable _EmpTypeWeekly = new DataTable();
                DataTable _dtWeekend = new DataTable();
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                string fnhsyscmpid = "0";
                string FNHSysEmpTypeId = "0";

                _Qry = " SELECT fnhsyscmpid , FNHSysEmpTypeId FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee WHERE fnhsysempid = " + _FNHSysempId + " ";
                _dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR);

                foreach (DataRow dr in _dt.Rows)
                {
                    fnhsyscmpid = dr["fnhsyscmpid"].ToString();
                    FNHSysEmpTypeId = dr["FNHSysEmpTypeId"].ToString();
                }


                _Qry = "SELECt   FDHolidayDate  ";
                _Qry += Constants.vbCrLf + "  FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmpTypeWeeklySpecial WITH(NOLOCK) ";
                _Qry += Constants.vbCrLf + "   WHERE FDHolidayDate>='" + ConvertEnDB(StartDate) + "' ";
                _Qry += Constants.vbCrLf + "   AND FDHolidayDate<='" + ConvertEnDB(Enddate) + "' ";
                _Qry += Constants.vbCrLf + "   AND FNHSysEmpTypeId=" + FNHSysEmpTypeId + " ";

                _EmpTypeWeekly = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER);

                _Qry = "   SELECT    Top 1   FTSunday,FTMonday, FTTuesday, FTWednesday, ";
                _Qry += Constants.vbCrLf + "   FTThursday, FTFriday, FTSaturday";
                _Qry += Constants.vbCrLf + "   FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployeeWeekly  As W WITH(NOLOCK) ";
                _Qry += Constants.vbCrLf + " WHERE FNHSysEmpID=" + FNHSysEmpTypeId + " ";
                _dtWeekend = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR);

                if (_dtWeekend.Rows.Count <= 0)
                {

                }
                else
                {
                    _EmpTypeWeekly.Rows.Clear();
                }



                _Qry = "SELECt   FDHolidayDate  ";
                _Qry += Constants.vbCrLf + " FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo.THRMHoliday WITH(NOLOCK) ";
                _Qry += Constants.vbCrLf + "  WHERE FDHolidayDate>='" + ConvertEnDB(StartDate) + "' ";
                _Qry += Constants.vbCrLf + "  AND FDHolidayDate<='" + ConvertEnDB(Enddate) + "'  AND FTStateActive='1' AND FNHSysCmpId=" + fnhsyscmpid;
                _dtHoliday = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER);

                DateTime _NextProcDate = Convert.ToDateTime(StartDate);
                DateTime _EndProcDate = Convert.ToDateTime(Enddate);

                double nNextDay = 0;
                int _TotalDay = 0;
                bool _SkipProcess = false;
                int _WeekEnd;

                string FTStateNotMergeHoliday = "1";

                if (_NextProcDate.ToString() != "" && _EndProcDate.ToString() != "")
                {

                    foreach (DateTime day in EachDay(_NextProcDate, _EndProcDate))
                    {
                        // _WeekEnd = DayOfWeek((_NextProcDate), Microsoft.VisualBasic.FirstDayOfWeek.Sunday);
                        _WeekEnd = Weekday(_NextProcDate, DayOfWeek.Sunday);
                        _SkipProcess = false;

                        if (FTStateNotMergeHoliday == "1")
                        {

                            if (_SkipProcess == false)
                            {
                                foreach (DataRow Rday in _dtWeekend.Rows)
                                {
                                    if (_WeekEnd.ToString() == "0")
                                    {
                                        _SkipProcess = true;
                                    }

                                }



                                foreach (DataRow Dr in _dtHoliday.Select("   FDHolidayDate  = '" + ConvertEnDB(_NextProcDate) + "' "))
                                {
                                    _SkipProcess = true;

                                }

                                foreach (DataRow Dr in _EmpTypeWeekly.Select("   FDHolidayDate  = '" + ConvertEnDB(_NextProcDate) + "' "))
                                {
                                    _SkipProcess = true;

                                }

                            }


                        }
                        if (_SkipProcess == false)
                        {
                            _TotalDay = _TotalDay + 1;
                        }


                        _NextProcDate = _NextProcDate.AddDays(1);
                    }

                }


                FTNetDay = _TotalDay;


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }



        }
        
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        //public int Weekday(DateTime DateValue, FirstDayOfWeek DayOfWeek = FirstDayOfWeek.Sunday)
        //{
        //    return 1;
        //}

        private static int Weekday(DateTime date, DayOfWeek startDay)
        {
            int diff;
            DayOfWeek dow = date.DayOfWeek;
            diff = dow - startDay;
            if (diff < 0)
            {
                diff += 7;
            }
            return diff;
        }

        public static string GETUserName(string fnhsysempid)
        {
            try
            {
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();


                string _qry = "";

                _qry = @"SELECT FTUserName  ";
                _qry += @" FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SECURITY) + "].dbo.TSEUserLogin ";
                _qry += @" WHERE FNHSysEmpID = " + fnhsysempid;
                fnhsysempid = Cnn.GetField(_qry, WSM.Conn.DB.DataBaseName.DB_HR, "");
                return fnhsysempid;
            }
            catch (Exception ex)
            {
                return "";
            }




        }

      

        public static bool SaveDataLeave(string leavekey, string _FNHSysempId, string StartDate, string Enddate, string StartTime, string EndTime, string remark, byte[] attFile, string ExtentionFile, int LeaveDayState, string  LeaveMethod, int LeaveMinutes,  string FTStateNotMergeHoliday,  string FTStateLeavepay,  string FTStateCalSSo,  string FTStateMedicalCertificate,  string FTStateDeductVacation, ref int NextApprovalID)
        {
            try
            {

                string _Qry = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                
                string FTNetDay = LeaveMethod;
                

                string FTSTime = StartTime;
                string FTETime = EndTime;

                //check holiday 
                //check flag pay



                double FNNetTime = 0.00; //hh.mm
                int ocetotaltime = 0; // mm

                ocetotaltime = LeaveMinutes;


                FNNetTime = LeaveMinutes / 60 + ((LeaveMinutes % 60) * 0.01);



                //string FTStateNotMergeHoliday = "1";
                //string FTStateLeavepay = "";
                //string FTStateCalSSo = "0";
                //string FTStateMedicalCertificate = "0";


                // string FNLeaveDay = "0";



                string username = GETUserName(_FNHSysempId);



                if (leavekey == "97")
                {
                    FTStateNotMergeHoliday = "0";
                }

                _Qry = "SELECT TOP 1 FNHSysEmpID FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily WITH(NOLOCK)";
                _Qry += Constants.vbCrLf + "WHERE FNHSysEmpID = " + _FNHSysempId + " ";
                _Qry += Constants.vbCrLf + "AND FTStartDate = '" + ConvertEnDB(StartDate) + "'";
                _Qry += Constants.vbCrLf + "AND FTEndDate = '" + ConvertEnDB(Enddate) + "'";
                _Qry += Constants.vbCrLf + " AND FTLeaveType = '" + leavekey + "'";

                if (Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "") == "")
                {
                    _Qry = " INSERT INTO [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily (FTInsUser, FTInsDate, FTInsTime";
                    _Qry += Constants.vbCrLf + " , FNHSysEmpID, FTStartDate, FTEndDate,FTHoliday, FNLeaveTotalDay";
                    _Qry += Constants.vbCrLf + " , FTLeaveType,  FTLeavePay";
                    _Qry += Constants.vbCrLf + " , FTLeaveStartTime, FTLeaveEndTime, FNLeaveTotalTime,FNLeaveTotalTimeMin, FTLeaveNote";
                    _Qry += Constants.vbCrLf + " ,FTApproveState,FTStaCalSSO,FTStaLeaveDay,FTStateMedicalCertificate,FTMedicalCertificateName, FTStateDeductVacation ";
                    if (ExtentionFile != null)
                    {
                        _Qry += Constants.vbCrLf + " , FBFileRef, FBFile ";
                    }

                    _Qry += Constants.vbCrLf + " , FTSendApproveState,FDSendApproveDate,FTSendApproveTime,FTSendApproveBy, FTRequestFromSystem )";

                    _Qry += Constants.vbCrLf + " VALUES ('" + username + "'," + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + " ," + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + " ,'" + _FNHSysempId + "'";
                    _Qry += Constants.vbCrLf + " ,'" + ConvertEnDB(StartDate) + "'";
                    _Qry += Constants.vbCrLf + " ,'" + ConvertEnDB(Enddate) + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateNotMergeHoliday.ToString() + "'";
                    _Qry += Constants.vbCrLf + " ," + LeaveMethod + "";
                    _Qry += Constants.vbCrLf + " ,'" + leavekey + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateLeavepay + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTSTime + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTETime + "'";
                    _Qry += Constants.vbCrLf + " ," + FNNetTime + "";
                    _Qry += Constants.vbCrLf + " ," + ocetotaltime + "";
                    _Qry += Constants.vbCrLf + " ,N'" + remark + "'";
                    _Qry += Constants.vbCrLf + " ,'0'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateCalSSo + "'";
                    _Qry += Constants.vbCrLf + " ,'" + LeaveDayState.ToString() + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateMedicalCertificate + "',''";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateDeductVacation + "'";
                    
                    if (ExtentionFile != null)
                    {
                        _Qry += Constants.vbCrLf + " ,@attfile";
                        _Qry += Constants.vbCrLf + " ,'" + ExtentionFile + "'";
                    }
                    _Qry += Constants.vbCrLf + " ,'" + "1" + "'";
                    _Qry += Constants.vbCrLf + " ," + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + " ," + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + " ,'" + username + "'";

                    _Qry += Constants.vbCrLf + " ,'PMDS'";

                    _Qry += Constants.vbCrLf + ")";



                }
                else
                {
                    _Qry = " UPDATE [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily SET ";
                    _Qry += Constants.vbCrLf + " FTUpdUser = '" + username + "'";
                    _Qry += Constants.vbCrLf + " ,FTUpdDate = " + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + " ,FTUpdTime = " + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + " ,FTHoliday = '" + FTStateNotMergeHoliday + "'";
                    _Qry += Constants.vbCrLf + " ,FNLeaveTotalDay = " + LeaveMethod + "";
                    _Qry += Constants.vbCrLf + " ,FTLeaveType = '" + leavekey + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeavePay = '" + FTStateLeavepay + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeaveStartTime = '" + FTSTime + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeaveEndTime = '" + FTETime + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeaveNote = N'" + remark + "'";
                    _Qry += Constants.vbCrLf + " ,FNLeaveTotalTime='" + FNNetTime + "'";
                    _Qry += Constants.vbCrLf + " ,FNLeaveTotalTimeMin='" + ocetotaltime + "'";
                    _Qry += Constants.vbCrLf + " ,FTApproveState='0'  , FTMngApproveState='0'  ,FTDirApproveState = '0'";

                    _Qry += Constants.vbCrLf + ", FDMngApproveDate =NULL";
                    _Qry += Constants.vbCrLf + ", FTMngApproveTime =NULL";
                    _Qry += Constants.vbCrLf + ", FTMngApproveBy =NULL";

                    _Qry += Constants.vbCrLf + ", FDDirApproveDate =NULL";
                    _Qry += Constants.vbCrLf + ", FTDirApproveBy =NULL";
                    _Qry += Constants.vbCrLf + ", FTDirApproveTime =NULL";


                    _Qry += Constants.vbCrLf + " , FTStaCalSSO='" + FTStateCalSSo + "'";
                    _Qry += Constants.vbCrLf + " , FTStaLeaveDay='" + LeaveDayState.ToString() + "'";
                    _Qry += Constants.vbCrLf + " ,FTStateMedicalCertificate= '" + FTStateMedicalCertificate + "'";
                    _Qry += Constants.vbCrLf + " ,FTMedicalCertificateName='" + "" + "' ";
                    _Qry += Constants.vbCrLf + " ,FTStateDeductVacation= '" + FTStateDeductVacation + "'";
                    

                    if (ExtentionFile != null)
                    {
                        _Qry += Constants.vbCrLf + ", FBFileRef = @attfile ";
                        _Qry += Constants.vbCrLf + ", FBFile = '" + ExtentionFile + "'";
                    }
                    _Qry += Constants.vbCrLf + ", FTSendApproveState = '1'";
                    _Qry += Constants.vbCrLf + ", FDSendApproveDate = " + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + ", FTSendApproveTime=" + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + ", FTSendApproveBy='" + username + "'";
                    _Qry += Constants.vbCrLf + ", FTRequestFromSystem='PMDS'";

                    _Qry += Constants.vbCrLf + " WHERE FNHSysEmpID = " + _FNHSysempId + "";
                    _Qry += Constants.vbCrLf + " AND FTStartDate = '" + ConvertEnDB(StartDate) + "'";
                    _Qry += Constants.vbCrLf + " AND FTEndDate = '" + ConvertEnDB(Enddate) + "'";
                    _Qry += Constants.vbCrLf + " AND FTLeaveType = '" + leavekey + "'";

                    

                }

                System.Data.SqlClient.SqlConnection _Cnn = new System.Data.SqlClient.SqlConnection();
                System.Data.SqlClient.SqlCommand _Cmd = new System.Data.SqlClient.SqlCommand();

                WSM.Conn.SQLConn sQLConn = new WSM.Conn.SQLConn();
                try
                {


                    if (_Cnn.State == ConnectionState.Open) { _Cnn.Close(); };
                    _Cnn.ConnectionString = WSM.Conn.DB.ConnectionString(WSM.Conn.DB.DataBaseName.DB_HR);
                    _Cnn.Open();
                    _Cmd = _Cnn.CreateCommand();
                    _Cmd.CommandTimeout = 0;
                    _Cmd.CommandType = CommandType.Text;
                    _Cmd.CommandText = _Qry;
                    if (ExtentionFile != null)
                    {
                        _Cmd.Parameters.Add("@attfile", SqlDbType.VarBinary).Value = attFile;
                    }
                    _Cmd.ExecuteNonQuery();
                    _Cmd.Parameters.Clear();

                    _Cmd.Dispose();
                    _Cnn.Dispose();

                    DataTable dt = new DataTable();
                    String FNHSysEmpIDu1 = "0";
                    String FNHSysEmpIDu2 = "0";


                    _Qry = "  SELECT ISNULL(u1.FNHSysEmpID,0) AS [FNHSysEmpIDu1], ISNULL(u2.FNHSysEmpID,0) AS [FNHSysEmpIDu2]  ";
                    _Qry += Constants.vbCrLf + " FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee M ";
                    _Qry += Constants.vbCrLf + "  LEFT OUTER JOIN [HITECH_SECURITY].[dbo].[TSEUserLogin] as u1 on u1.FTUserName=M.FTUserNameChk  ";
                    _Qry += Constants.vbCrLf + " LEFT OUTER JOIN [HITECH_SECURITY].[dbo].[TSEUserLogin] as u2 on u2.FTUserName=M.FTUserNameMngFac ";
                    _Qry += Constants.vbCrLf + " WHERE M.FNHSysEmpId='" + _FNHSysempId + "'";

                    dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

                    foreach (DataRow dr in dt.Rows)
                    {
                        FNHSysEmpIDu1 = dr["FNHSysEmpIDu1"].ToString();
                        FNHSysEmpIDu2 = dr["FNHSysEmpIDu2"].ToString();
                    }

                    NextApprovalID = int.Parse(FNHSysEmpIDu1);


                    return true;
                }
                catch (Exception ex)
                {
                    _Cmd.Dispose();
                    _Cnn.Dispose();
                    //Interaction.MsgBox(ex.Message);
                    return false;
                }
                // Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_HR);

                //get  approval

               

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

       


            public static bool SendApprove(int _FNHSysempId, int leavekey, string StartDate, string Enddate, int _FNHSysempId_Appr, string FTStateType, int ActionType, ref int msgCode, ref string msgDesc , ref int  NextApprovalID)
        {
            try
            {
                string _Qry = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                string usernameAppr = "";

                if (ActionType.ToString() == "2") //Remove Leave
                {
                    //check FTApproveState 

                    string FTApproveState = "";

                    _Qry = "SELECT ISNULL(FTApproveState,'0') AS FTApproveState FROM   [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily";
                    _Qry += Constants.vbCrLf + "  WHERE FNHSysEmpId = " + _FNHSysempId + "";
                    _Qry += Constants.vbCrLf + " AND FTStartDate = '" + ConvertEnDB(StartDate) + "'";
                    _Qry += Constants.vbCrLf + " AND FTEndDate = '" + ConvertEnDB(Enddate) + "'";
                    _Qry += Constants.vbCrLf + " AND FTLeaveType = '" + leavekey + "'";


                    FTApproveState = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

                    if (FTApproveState != "1")
                    {
                        _Qry = "DELETE FROM   [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily";
                        _Qry += Constants.vbCrLf + "  WHERE FNHSysEmpId = " + _FNHSysempId + "";
                        _Qry += Constants.vbCrLf + " AND FTStartDate = '" + ConvertEnDB(StartDate) + "'";
                        _Qry += Constants.vbCrLf + " AND FTEndDate = '" + ConvertEnDB(Enddate) + "'";
                        _Qry += Constants.vbCrLf + " AND FTLeaveType = '" + leavekey + "'";

                        if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_HR) == false)
                        {
                            msgCode = 404;
                            msgDesc = "Can not remove leave. Because aleady HR Approved.";
                            return false;
                        }
                        msgCode = 200;
                        msgDesc = "Already remove leave.";
                        return true;
                    }
                    else
                    {
                        msgCode = 404;
                        msgDesc = "Can not remove leave. Because aleady HR Approved.";
                        return false;
                    }



                }
                else
                {
                    //Action Approve Cancel

                    usernameAppr = GETUserName(_FNHSysempId_Appr.ToString());

                    DataTable dt = new DataTable() ;
                    String FNHSysEmpIDu1 = "0";
                    String FNHSysEmpIDu2 = "0";


                    if (FTStateType.ToString() == "1")
                    {
                        //CHECK DIRECTor Approval 

                        _Qry = "  SELECT ISNULL(u1.FNHSysEmpID,0) AS [FNHSysEmpIDu1], ISNULL(u2.FNHSysEmpID,0) AS [FNHSysEmpIDu2]  ";
                        _Qry += Constants.vbCrLf + " FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee M ";
                       _Qry += Constants.vbCrLf + "  LEFT OUTER JOIN [HITECH_SECURITY].[dbo].[TSEUserLogin] as u1 on u1.FTUserName=M.FTUserNameChk  ";
                        _Qry += Constants.vbCrLf + " LEFT OUTER JOIN [HITECH_SECURITY].[dbo].[TSEUserLogin] as u2 on u2.FTUserName=M.FTUserNameMngFac ";
                        _Qry += Constants.vbCrLf + " WHERE M.FNHSysEmpId='" + _FNHSysempId + "'";

                        dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

                        foreach (DataRow dr in dt.Rows)
                        {
                            FNHSysEmpIDu1 = dr["FNHSysEmpIDu1"].ToString();
                            FNHSysEmpIDu2 = dr["FNHSysEmpIDu2"].ToString();
                        }


                    }




                    _Qry = "Update [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily";

                    if (FTStateType.ToString() == "0")
                    {

                        _Qry += Constants.vbCrLf + " Set FTDirApproveState='" + ActionType + "'";
                        _Qry += Constants.vbCrLf + ", FTDirApproveBy='" + usernameAppr + "'";
                        _Qry += Constants.vbCrLf + ", FDDirApproveDate=" + FormatDateDB + "";
                        _Qry += Constants.vbCrLf + ", FTDirApproveTime=" + FormatTimeDB + "";
                    }
                    else
                    {
                        if (FNHSysEmpIDu2 == "0")
                        {
                            //กรณี มีแต่หัวหน้างาน

                            _Qry += Constants.vbCrLf + " Set FTMngApproveState='" + ActionType + "'";
                            _Qry += Constants.vbCrLf + ", FTMngApproveBy='" + usernameAppr + "'";
                            _Qry += Constants.vbCrLf + ", FDMngApproveDate=" + FormatDateDB + "";
                            _Qry += Constants.vbCrLf + ", FTMngApproveTime=" + FormatTimeDB + "";
                            _Qry += Constants.vbCrLf + " , FTDirApproveState='" + ActionType + "'";
                            _Qry += Constants.vbCrLf + ", FTDirApproveBy='" + usernameAppr + "'";
                            _Qry += Constants.vbCrLf + ", FDDirApproveDate=" + FormatDateDB + "";
                            _Qry += Constants.vbCrLf + ", FTDirApproveTime=" + FormatTimeDB + "";
                        }
                        else
                        {


                            _Qry += Constants.vbCrLf + " Set FTMngApproveState='" + ActionType + "'";
                            _Qry += Constants.vbCrLf + ", FTMngApproveBy='" + usernameAppr + "'";
                            _Qry += Constants.vbCrLf + ", FDMngApproveDate=" + FormatDateDB + "";
                            _Qry += Constants.vbCrLf + ", FTMngApproveTime=" + FormatTimeDB + "";
                        }
                       
                    }



                    _Qry += Constants.vbCrLf + " WHERE FNHSysEmpId='" + _FNHSysempId + "'";
                    _Qry += Constants.vbCrLf + " and FTStartDate='" + ConvertEnDB(StartDate) + "'";
                    _Qry += Constants.vbCrLf + " and  FTEndDate='" + ConvertEnDB(Enddate) + "'";

                    if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_HR) == false)
                    {

                        msgCode = 404;
                        msgDesc = "Not found Leave. Can not approve leave.";
                        NextApprovalID = 0;
                        return false;
                    }

                    msgCode = 200;
                    msgDesc = "Already Approve.";
                    if (FTStateType.ToString() == "1")
                    {
                        NextApprovalID = int.Parse(FNHSysEmpIDu2);
                    }
                    else
                    {
                        NextApprovalID = 0;
                    }
                   
                    return true;
                }

            }
            catch (Exception ex)
            {
                msgCode = 404;
                msgDesc = "Found Error System.";
                NextApprovalID = 0;
                return false;
            }
        }

        public static string CheckDate(object Obj)
        {
            try
            {
                CultureInfo _Culture = new CultureInfo("en-US", true);
                _Culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                _Culture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";

                System.Threading.Thread.CurrentThread.CurrentCulture = _Culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = _Culture;



                string _Date = "";
                _Date = Strings.Format(Convert.ToDateTime(Obj), "dd/MM/yyyy");

                return _Date;
            }
            catch //(Exception ex)
            {
                return "";
            }
        }

        public static long UDayDiff(DateInterval _Type, string ObjStartDate, string ObjEndDate)
        {

            try
            {
                string _SDate = ConvertEnDB(ObjStartDate);
                string _EDate = ConvertEnDB(ObjEndDate);

                if (!string.IsNullOrEmpty(_SDate) & !string.IsNullOrEmpty(_EDate))
                {
                    return DateAndTime.DateDiff(_Type, Convert.ToDateTime(_SDate), Convert.ToDateTime(_EDate));
                }
                else
                {
                    return 0;
                }

            }
            catch
            {
                return 0;
            }

        }

        public static string ConvertEnDB(object DataDate)
        {
            string strDate = "";

            try
            {
                strDate = CheckDate(DataDate);

                if ((Convert.ToInt32(Strings.Mid(strDate, 7, 4)) > 0) & (Convert.ToInt32(Strings.Mid(strDate, 7, 4)) > (2300)))
                {
                    strDate = (Strings.Mid(strDate, 1, 2)) + "/" + (Strings.Mid(strDate, 4, 2)) + "/" + (Convert.ToInt32(Strings.Mid(strDate, 7, 4)) - 543).ToString("0000");
                }
                else
                {
                    strDate = (Strings.Mid(strDate, 1, 2)) + "/" + (Strings.Mid(strDate, 4, 2)) + "/" + (Strings.Mid(strDate, 7, 4));
                }

                strDate = Strings.Mid(strDate, 7, 4) + "/" + Strings.Mid(strDate, 4, 2) + "/" + Strings.Mid(strDate, 1, 2);
            }
            catch //(Exception ex)
            {
                strDate = "";
            }


            return strDate;

        }


        public static bool CreateEmp(WSMHRAPI.Models.CreateEmpModel createEmpModel, ref int msgFNHSysEmpID, ref string msgFTEmpCode, ref string msgCode, ref string msgDesc)
        {
            try
            {
                string _Qry = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                if (Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "") == "")
                {
                    //
                    string CmpCode = "";
                    CmpCode = Cnn.GetField("SELECT  FTCmpCode FROM  " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + ".dbo.TCNMCmp WHERE FNHSysCmpId=" + (createEmpModel.FNHSysCmpId), WSM.Conn.DB.DataBaseName.DB_MASTER, "0");


                    //get fnhsysempid 
                  int _FNHSysempId = 0;
                    _FNHSysempId = TL.RunID.GetRunNoID("THRMEmployee", "FNHSysEmpID", WSM.Conn.DB.DataBaseName.DB_HR, CmpCode);

                    string _CmpH = "";
                    _CmpH = Cnn.GetField("SELECT TOP 1 FTDocRun FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + ".dbo.TCNMCmp WHERE FNHSysCmpId=" + (createEmpModel.FNHSysCmpId) + " ", WSM.Conn.DB.DataBaseName.DB_SYSTEM, "");

                    string _FNProDay = "";
                    _FNProDay = Cnn.GetField("SELECT  FNProDay FROM  " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + ".dbo.THRMEmpType WHERE FNHSysCmpId=" + (createEmpModel.FNHSysCmpId) + " AND FNHSysEmpTypeId=" + (createEmpModel.FNHSysEmpTypeId) + "", WSM.Conn.DB.DataBaseName.DB_MASTER, "0");

                    //get ftempcode
                    string _EmpCode = "";
                    _EmpCode = HRFunction.Document.GetDocumentNo("HITECH_HR", "THRMEMPLOYEE", "", true, _CmpH, createEmpModel.FDDateStart);


                    _Qry = " INSERT INTO [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee (FTInsUser, FDInsDate, FTInsTime";
                    _Qry += Constants.vbCrLf + " , FNHSysEmpID , FNHSysCmpId,FTEmpCode,FNHSysEmpTypeId , FNHSysPreNameId ";
                    _Qry += Constants.vbCrLf + " ,FTEmpNameTH , FTEmpSurnameTH, FTEmpNicknameTH ";
                    _Qry += Constants.vbCrLf + " , FTEmpNameEN, FTEmpSurnameEN, FTEmpNicknameEN ";
                    _Qry += Constants.vbCrLf + " , FNEmpSex, FDDateStart ";

                    _Qry += Constants.vbCrLf + " ,FDDateProbation, FTProbationSta, FNEmpStatus ";

                    _Qry += Constants.vbCrLf + " , FCWeight,FCHeight,FDBirthDate, FTEmpIdNo, FTEmpIdNoBy ";
                    _Qry += Constants.vbCrLf + " , FDDateIdNoAssign, FDDateIdNoEnd ";
                    _Qry += Constants.vbCrLf + " , FTAddrNo , FTAddrMoo,FTAddrSoi, FTAddrRoad, FTAddrTumbol, FTAddrAmphur , FTAddrProvince, FTAddrPostCode, FTAddrTel ";
                    _Qry += Constants.vbCrLf + " , FTAddrNo1 , FTAddrMoo1, FTAddrSoi1, FTAddrRoad1, FTAddrTumbol1, FTAddrAmphur1 , FTAddrProvince1, FTAddrPostCode1, FTAddrTel1 ";
                    _Qry += Constants.vbCrLf + " , FTFatherName , FTFatherCareer, FNFatherLife ";
                    _Qry += Constants.vbCrLf + " , FTMotherName , FTMotherCareer ,FNMotherLife ";
                    _Qry += Constants.vbCrLf + " , FTTaxNo, FTSocialNo ";
                    _Qry += Constants.vbCrLf + " , FNHSysNationalityId , FNHSysRaceId, FNHSysReligionId, FNHSysBldId ";
                    _Qry += Constants.vbCrLf + " , FNMilitary ";
                    _Qry += Constants.vbCrLf + " , FNMaritalStatus , FTMateName, FTMateCareer, FTMateAddrWork ";

                    _Qry += Constants.vbCrLf + " , FNHSysShiftID ";
                    _Qry += Constants.vbCrLf + " , FNHSysPositId,FNHSysDivisonId, FNHSysDeptId , FNHSysSectId, FNHSysUnitSectId ";
                    _Qry += Constants.vbCrLf + "  , FNHSysPositIdOrg, FNHSysCLevelIdOrg, FNHSysCountryIdOrg, FNHSysCmpIdOrg";
                    _Qry += Constants.vbCrLf + " , FNHSysDivisonIdOrg, FNHSysDeptIdOrg, FNHSysSectIdOrg, FNHSysUnitSectIdOrg )";



                    _Qry += Constants.vbCrLf + " VALUES ('" + createEmpModel.username + "'," + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + " ," + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + " ,'" + _FNHSysempId + "'";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysCmpId + "";
                    _Qry += Constants.vbCrLf + " ,'" + _EmpCode + "'";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysEmpTypeId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysPreNameId + "";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpNameTH + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpSurnameTH + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpNicknameTH + "'";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpNameEN + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpSurnameEN + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpNicknameEN + "'";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FNEmpSex + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FDDateStart + "'";

                    _Qry += Constants.vbCrLf + " ,DATEADD(DAY," + _FNProDay + ",'" + createEmpModel.FDDateStart + "') , 0 , 0 ";

                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FCWeight + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FCHeight + "";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FDBirthDate + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpIdNo + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTEmpIdNoBy + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FDDateIdNoAssign + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FDDateIdNoEnd + "'";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrNo + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrMoo + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrSoi + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrRoad + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrTumbol + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrAmphur + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrProvince + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrPostCode + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrTel + "'";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrNo1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrMoo1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrSoi1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrRoad1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrTumbol1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrAmphur1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrProvince1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrPostCode1 + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTAddrTel1 + "'";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTFatherName + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTFatherCareer + "'";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNFatherLife + "";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTMotherName + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTMotherCareer + "'";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNMotherLife + "";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTTaxNo + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTSocialNo + "'";

                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysNationalityId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysRaceId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysReligionId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysBldId + "";

                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNMilitary + "";

                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FNMaritalStatus + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTMateName + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTMateCareer + "'";
                    _Qry += Constants.vbCrLf + " ,'" + createEmpModel.FTMateAddrWork + "'";

                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysShiftID + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysPositId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysDivisonId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysDeptId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysSectId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysUnitSectId + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysPositIdOrg + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysCLevelIdOrg + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysCountryIdOrg + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysCmpIdOrg + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysDivisonIdOrg + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysDeptIdOrg + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysSectIdOrg + "";
                    _Qry += Constants.vbCrLf + " ," + createEmpModel.FNHSysUnitSectIdOrg + "";

                    _Qry += Constants.vbCrLf + ")";


                    if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_HR) == false)
                    {

                        msgCode = "404";
                        msgDesc = "Can not create Employee.";
                        return false;
                    }



                    if (createEmpModel.CreateEmpChilds.Count > 0)
                    {
                        List<Models.CreateEmpChild> Childs = createEmpModel.CreateEmpChilds;
                        for (int i = 0; i < Childs.Count; i++)
                        {
                            int empid = Childs[i].EmployeeId;

                            _Qry = "INSERT INTO THRMEmployeeChild (FTInsUser, FDInsDate, FTInsTime, FNHSysEmpID ";
                            _Qry += Constants.vbCrLf + " , FNSeqNo, FTChildName, FDChildBirthDate, FTChildSex, FTStudySta, FTStateNotDisTax) ";
                            _Qry += Constants.vbCrLf + " VALUES ('" + createEmpModel.username + "'," + FormatDateDB + "";
                            _Qry += Constants.vbCrLf + " ," + FormatTimeDB + "";
                            _Qry += Constants.vbCrLf + " ,'" + _FNHSysempId + "'";
                            _Qry += Constants.vbCrLf + " ,'" + Childs[i].FNSeqNo + "'";
                            _Qry += Constants.vbCrLf + " ,'" + Childs[i].FTChildName + "'";
                            _Qry += Constants.vbCrLf + " ,'" + Childs[i].FDChildBirthDate + "'";
                            _Qry += Constants.vbCrLf + " ,'" + Childs[i].FTChildSex + "'";
                            _Qry += Constants.vbCrLf + " ,'" + Childs[i].FTStudySta + "'";
                            _Qry += Constants.vbCrLf + " ,'" + Childs[i].FTStateNotDisTax + "'";

                            Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_HR);
                        }

                    }

                    msgFNHSysEmpID = _FNHSysempId;
                    msgFTEmpCode = _EmpCode;
                    msgCode = "200";
                    msgDesc = "";
                    return true;

                }


                return false;
            }
            catch { return false; }

        }


        public static bool UpdateEmpCode(WSMHRAPI.Models.UpdateEmpCodeModel updateEmpCodeModel, ref int msgFNHSysEmpID, ref string msgFTEmpCode, ref string msgCode, ref string msgDesc)
        {
            try
            {
                string _Qry = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                if (Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "") == "")
                {
                    string _CmpH = "";
                    _CmpH = Cnn.GetField("SELECT TOP 1 FTDocRun FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + ".dbo.TCNMCmp WHERE FNHSysCmpId=" + (updateEmpCodeModel.FNHSysCmpId) + " ", WSM.Conn.DB.DataBaseName.DB_SYSTEM, "");

                    string _FNProDay = "";
                    _FNProDay = Cnn.GetField("SELECT  FNProDay FROM  " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + ".dbo.THRMEmpType WHERE FNHSysCmpId=" + (updateEmpCodeModel.FNHSysCmpId) + " AND FNHSysEmpTypeId=" + (updateEmpCodeModel.FNHSysEmpTypeId) + "", WSM.Conn.DB.DataBaseName.DB_MASTER, "0");

                    //get ftempcode
                    string _EmpCode = "";
                    _EmpCode = HRFunction.Document.GetDocumentNo("HITECH_HR", "THRMEMPLOYEE", "", true, _CmpH, updateEmpCodeModel.FDDateStart);


                    _Qry = " UPDATE [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee ";
                    _Qry += Constants.vbCrLf + " SET FTEmpCode = '" + _EmpCode + "'";
                    _Qry += Constants.vbCrLf + " , FDDateStart ='" + updateEmpCodeModel.FNHSysEmpID + "'";
                    _Qry += Constants.vbCrLf + " , FDDateProbation =  DATEADD(DAY, " + _FNProDay + ", '" + updateEmpCodeModel.FDDateStart + "') ";
                    _Qry += Constants.vbCrLf + " , FTUpdUser ='" + updateEmpCodeModel.username + "'";
                    _Qry += Constants.vbCrLf + " , FDUpdDate =" + FormatDateDB + " ";
                    _Qry += Constants.vbCrLf + " , FTUpdTime =" + FormatTimeDB + " ";


                    _Qry += Constants.vbCrLf + " WHERE FNHSysEmpID = " + updateEmpCodeModel.FNHSysEmpID;

                    if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_HR) == false)
                    {

                        msgCode = "404";
                        msgDesc = "Can not update Employee Code.";
                        return false;
                    }
                    else
                    {
                        msgFNHSysEmpID = updateEmpCodeModel.FNHSysEmpID;
                        msgFTEmpCode = _EmpCode;
                        msgCode = "200";
                        msgDesc = "Already update Employee Code.";
                        return true;
                    }
                }
                return false;

            }
            catch
            {
                return false;
            }


        }
    }


}