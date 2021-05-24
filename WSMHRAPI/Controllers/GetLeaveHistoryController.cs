using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace WSMHRAPI.Controllers
{

    public class GetLeaveHistoryController : ApiController
    {
        //
        //// GET: api/LeaveHistory
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/LeaveHistory/5
        // GET: api/PermissionLeave/5
        public HttpResponseMessage Get(string id)
        {
            try
            {


                bool regisstate = false;
                string msgerror = "";


                string cmdstring = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                System.Data.DataTable dt;

                System.Data.DataSet dts = new System.Data.DataSet("JsonDs");

                dt = ShowLeaveHistory(id);


                dts.Tables.Add(dt.Copy());

                string jsondata = JsonConvert.SerializeObject(dts);

                dt = null;
                dts = null;

                return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent(jsondata, System.Text.Encoding.UTF8, "application/json") };



            }
            catch (Exception ex)
            {

                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + ex.Message + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };

            }
        }


        public static DataTable ShowLeaveHistory(string EmpCode)
        {
            DataTable dt = new DataTable();
            string _Qry;
            string tResetLeave = "";

            string tEmpType = "";
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();



            _Qry = @"SELECT Convert(datetime,A.FTStartDate) As FTStartDate
                    , Convert(datetime, A.FTEndDate) As FTEndDate
                     , CASE WHEN ISNULL(A.FTHoliday,'0') = '1' THEN  '1'  ELSE '0'  END AS  FTHoliday
                    ,A.FTLeaveType
                    , A.FNLeaveTotalDay, B.FTNameTH AS FTLeaveTypeName
                    , CASE WHEN ISNULL(FTLeavePay, '0') = '1' THEN '1' ELSE '0' END AS FTLeavePay
                    , FTLeaveStartTime , FTLeaveEndTime, FNLeaveTotalTime, FTLeaveNote,A.FTLeaveType AS FTLeaveTypeCode
                    ,CASE WHEN ISNULL(FTApproveState, '0') = '1' THEN  '1'  ELSE '0'  END AS  FTApproveState
                    ,CASE WHEN ISNULL(FTStaCalSSO, '0') = '1' THEN  '1'  ELSE '0'  END AS  FTStaCalSSO
                      ,CASE WHEN ISNULL(FTStaLeaveDay, '-1') <= '0' THEN '0' ELSE FTStaLeaveDay END As FTStaLeaveDay,ISNULL(A.FTStateDeductVacation, '0') AS FTStateDeductVacation
                        , ISNULL(A.FTMngApproveState, '0') AS FTMngApproveState
                         , ISNULL(A.FTMngApproveBy, '') AS FTMngApproveBy
                          , Convert(nvarchar(10), convert(datetime, A.FDMngApproveDate), 103) AS FDMngApproveDate
                          , ISNULL(A.FTMngApproveTime, '') AS FTMngApproveTime
                           , ISNULL(A.FTSendApproveState, '0') AS FTSendApproveState
                            , ISNULL(A.FTSendApproveBy, '') AS FTSendApproveBy
                             , Convert(nvarchar(10), convert(datetime, A.FDSendApproveDate), 103) AS FDSendApproveDate
                             , ISNULL(A.FTSendApproveTime, '') AS FTSendApproveTime
                              , ISNULL(C.FTNameTH, '') AS FTStaLeaveDayName
                               , ISNULL(A.FTDirApproveState, '0') AS FTDirApproveState
                                , ISNULL(A.FTDirApproveBy, '') AS FTDirApproveBy
                                 , Convert(nvarchar(10), convert(datetime, A.FDDirApproveDate), 103) AS FDDirApproveDate
                                 , ISNULL(A.FTDirApproveTime, '') AS FTDirApproveTime
                                  , ISNULL(A.FTStateMedicalCertificate, '0') AS FTStateMedicalCertificate
                                   , ISNULL(A.FTUpdUser, A.FTInsUser) AS FTInsUser
                                    , CASE WHEN ISNULL(A.FTUpdUser,'') = '' THEN A.FTInsDate ELSE A.FTUpdDate END AS FTInsDate
                                     , CASE WHEN ISNULL(A.FTUpdUser,'') = '' THEN A.FTInsTime ELSE A.FTUpdTime END  AS FTInsTime
                      FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + @".dbo.THRTLeaveAdvanceDaily As A WITH(NOLOCK) Left Outer Join(SELECt * FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SYSTEM) + @".dbo.HSysListData AS LT WITH(NOLOCK) WHERE FTListName = 'FNLeaveType') As B ON A.FTLeaveType = Convert(varchar(50), B.FNListIndex)
                      Left Outer Join(SELECt * FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SYSTEM) + @".dbo.HSysListData AS LD WITH(NOLOCK)  WHERE FTListName = 'FNLeaveDay') As C ON A.FTStaLeaveDay = Convert(varchar(50), C.FNListIndex)
                      WHERE FNHSysEmpID = " + EmpCode + @"
                    ORDER BY A.FTStartDate DESC ";


            dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

            return dt;
        }


            //public IHttpActionResult Get(string id, int CmpId)
            //{

            //    DataTable dt = new System.Data.DataTable();
            //    string _cmd;
            //    _cmd = "Select  *  from dbo.fn_LeaveHistory('" + id + "')";
            //    WSM.Conn.SQLConn qLConn = new WSM.Conn.SQLConn();
            //    dt = qLConn.GetDataTable(_cmd, WSM.Conn.DB.DataBaseName.DB_HR);
            //    return Ok(dt);

            //}

            // POST: api/LeaveHistory
            public void Post([FromBody]string value)
        {
        }

        // PUT: api/LeaveHistory/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/LeaveHistory/5
        public void Delete(int id)
        {
        }
    }
}
