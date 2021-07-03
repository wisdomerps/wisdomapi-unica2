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

        public static bool CreateLeave(string _FNHSysempId, string _FNHSysWorkShift, string LeaveTypeId, int LeaveDayState, string LeaveMethod, int LeaveMinutes, string StartDate, string Enddate, string StartTime, string EndTime, string Remark, byte[] attFile, string ExtentionFile, ref string msgCode, ref string msgDesc)
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

                    if (Checkpay(LeaveTypeId, _FNHSysempId, StartDate, Enddate, StartTime, EndTime))
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



                    // Save
                    if (SaveData(LeaveTypeId, _FNHSysempId, StartDate, Enddate, StartTime, EndTime, Remark, attFile, ExtentionFile))
                    {

                        msgCode = "";
                        msgDesc = "บันทึกการลาเรียบร้อยแล้ว!!!";
                        return true;
                    }
                    else
                    {

                        msgCode = "";
                        msgDesc = "ไม่สามารถบันทึกการลาได้ !!!";
                        return false;
                    }

                }
                else
                {

                    //msgCode = "";
                    //msgDesc = "พบข้อมูลการลาเกินกำหนด !!!";
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

        private static bool Checkpay(string leavekey, string _FNHSysempId, string StartDate, string Enddate, string StartTime, string EndTime)
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

            if (leavekey == "0") {
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
                    //_GLeave = _GLeave + (FTNetDay.Value * ocetotaltime.Value)
                }
            }
            catch (Exception ex)
            {
            }

            Boolean FTStateLeavepay = true;
            Boolean FTStateNotMergeHoliday = true;
            Boolean FTStateCalSSo = true;
            
            //FTStateLeavepay.Enabled = True
            if (SetCheckPay)
            {
                FTStateLeavepay = false;

                if (ChkPayLeave(leavekey, _FNHSysempId))
                {
                    //If(FNLeaveDay.SelectedIndex <> 3 And(_LeavePay >= _GLeavePay + (FTNetDay.Value * ocetotaltime.Value))) Then
                    //    FTStateLeavepay.Checked = (_LeavePay >= _GLeavePay + (FTNetDay.Value * ocetotaltime.Value))
                    //Else
                }
                else
                {
                    FTStateLeavepay = false;
                }
            }

            FTStateNotMergeHoliday = (_FTStaHoliday == "1") || (leavekey == "98");
            FTStateCalSSo = FTStateLeavepay;


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
                _Qry += Constants.vbCrLf + " and FTLeaveCode = '" + leavekey+ "'";

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
                        _WeekEnd = Weekday(_NextProcDate , DayOfWeek.Sunday);
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

        public static bool SaveData(string leavekey, string _FNHSysempId, string StartDate, string Enddate, string StartTime, string EndTime, string remark, byte[] attFile, string ExtentionFile)
        {
            try
            {

                string _Qry = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

                string FTStateNotMergeHoliday = "1";
                string FTNetDay = "1";
                string FTStateLeavepay = "";

                string FTSTime = StartTime;
                string FTETime = EndTime;

                //check holiday 
                //check flag pay



                int FNNetTime = 0; //hh
                int ocetotaltime = 0; // mm

                ocetotaltime = FNNetTime * 480;

                

                string FTStateCalSSo = "0";
                string FTStateMedicalCertificate = "0";
                string FNLeaveDay = "0";



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
                    _Qry += Constants.vbCrLf + " ,FTApproveState,FTStaCalSSO,FTStaLeaveDay,FTStateMedicalCertificate,FTMedicalCertificateName ";
                    if (ExtentionFile != null)
                    {
                        _Qry += Constants.vbCrLf + " , FBFileRef, FBFile ";
                    }

                    _Qry += Constants.vbCrLf + " , FTSendApproveState,FDSendApproveDate,FTSendApproveTime,FTSendApproveBy )";

                    _Qry += Constants.vbCrLf + " VALUES ('"+ username + "'," + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + " ," + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + " ,'" + _FNHSysempId + "'";
                    _Qry += Constants.vbCrLf + " ,'" + ConvertEnDB(StartDate) + "'";
                    _Qry += Constants.vbCrLf + " ,'" + ConvertEnDB(Enddate) + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateNotMergeHoliday.ToString()  + "'";
                    _Qry += Constants.vbCrLf + " ," + FTNetDay + "";
                    _Qry += Constants.vbCrLf + " ,'" + leavekey + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateLeavepay + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTSTime + "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTETime +  "'";
                    _Qry += Constants.vbCrLf + " ,"  +  FNNetTime + "";
                    _Qry += Constants.vbCrLf + " ," + ocetotaltime + "";
                    _Qry += Constants.vbCrLf + " ,N'" + remark + "'";
                    _Qry += Constants.vbCrLf + " ,'0'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateCalSSo + "'";
                    _Qry += Constants.vbCrLf + " ,'" +  FNLeaveDay +  "'";
                    _Qry += Constants.vbCrLf + " ,'" + FTStateMedicalCertificate + "','' ";
                    if (ExtentionFile != null)
                    {
                        _Qry += Constants.vbCrLf + " ,@attfile";
                        _Qry += Constants.vbCrLf + " ,'" + ExtentionFile + "'";
                    }
                    _Qry += Constants.vbCrLf + " ,'" + "1" + "'";
                    _Qry += Constants.vbCrLf + " ," + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + " ," + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + " ,'" + username + "'";



                    _Qry += Constants.vbCrLf + ")";



                }
                else
                {
                    _Qry = " UPDATE [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily SET ";
                    _Qry += Constants.vbCrLf + " FTUpdUser = '" + username + "'";
                    _Qry += Constants.vbCrLf + " ,FTUpdDate = " + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + " ,FTUpdTime = " + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + " ,FTHoliday = '" + FTStateNotMergeHoliday + "'";
                    _Qry += Constants.vbCrLf + " ,FNLeaveTotalDay = " + FTNetDay + "";
                    _Qry += Constants.vbCrLf + " ,FTLeaveType = '" + leavekey + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeavePay = '" + FTStateLeavepay + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeaveStartTime = '" + FTSTime + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeaveEndTime = '" + FTETime + "'";
                    _Qry += Constants.vbCrLf + " ,FTLeaveNote = N'" + remark + "'";
                    _Qry += Constants.vbCrLf + " ,FNLeaveTotalTime='" + FNNetTime+"'";
                    _Qry += Constants.vbCrLf + " ,FNLeaveTotalTimeMin='" + ocetotaltime+"'";
                    _Qry += Constants.vbCrLf + " ,FTApproveState='0'  , FTMngApproveState='0'  ,FTDirApproveState = '0'";

                    _Qry += Constants.vbCrLf + ", FDMngApproveDate =NULL";
                    _Qry += Constants.vbCrLf + ", FTMngApproveTime =NULL";
                    _Qry += Constants.vbCrLf + ", FTMngApproveBy =NULL";

                    _Qry += Constants.vbCrLf + ", FDDirApproveDate =NULL";
                    _Qry += Constants.vbCrLf + ", FTDirApproveBy =NULL";
                    _Qry += Constants.vbCrLf + ", FTDirApproveTime =NULL";


                    _Qry += Constants.vbCrLf + " , FTStaCalSSO='" + FTStateCalSSo + "'";
                    _Qry += Constants.vbCrLf + " , FTStaLeaveDay='" + FNLeaveDay + "'";
                    _Qry += Constants.vbCrLf + " ,FTStateMedicalCertificate= '" + FTStateMedicalCertificate + "'";
                    _Qry += Constants.vbCrLf + " ,FTMedicalCertificateName='" + "" + "' ";

                    if (ExtentionFile != null)
                    {
                        _Qry += Constants.vbCrLf + ", FBFileRef = @attfile ";
                        _Qry += Constants.vbCrLf + ", FBFile = '" + ExtentionFile + "'";
                    }
                    _Qry += Constants.vbCrLf + ", FTSendApproveState = '1'";
                    _Qry += Constants.vbCrLf + ", FDSendApproveDate = " + FormatDateDB + "";
                    _Qry += Constants.vbCrLf + ", FTSendApproveTime=" + FormatTimeDB + "";
                    _Qry += Constants.vbCrLf + ", FTSendApproveBy='" + username + "'";


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

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

          
        }


        public static bool SendApprove( int _FNHSysempId,int leavekey, string StartDate, string Enddate, int _FNHSysempId_Appr, string FTStateType, int ActionType, ref int msgCode, ref string msgDesc)
        {
            try
            {
                string _Qry = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                string usernameAppr = "";

                if (ActionType.ToString() == "2")
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

                     usernameAppr = GETUserName(_FNHSysempId_Appr.ToString());

                    _Qry = "Update [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTLeaveAdvanceDaily";

                    if (ActionType.ToString() == "0")
                    {

                        _Qry += Constants.vbCrLf + " Set FTDirApproveState='" + ActionType + "'";
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



                    _Qry += Constants.vbCrLf + " WHERE FNHSysEmpId='" + _FNHSysempId + "'";
                    _Qry += Constants.vbCrLf + " and FTStartDate='" + ConvertEnDB(StartDate) + "'";
                    _Qry += Constants.vbCrLf + " and  FTEndDate='" + ConvertEnDB(Enddate) + "'";

                    if (Cnn.ExecuteOnly(_Qry, WSM.Conn.DB.DataBaseName.DB_HR) == false)
                    {

                        msgCode = 404;
                        msgDesc = "Not found Leave. Can not approve leave.";
                        return false;
                    }

                    msgCode = 200;
                    msgDesc = "Already Approve.";
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                msgCode = 404;
                msgDesc = "Found Error System.";
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
    }

    


}