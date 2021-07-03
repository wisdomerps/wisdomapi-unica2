using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSMERPAPI;
using System.Threading;
using Newtonsoft.Json;
using System.Data;

namespace WSMHRAPI.Controllers
{
    public class GetPermissionLeaveController : ApiController
    {
        // GET: api/Employee
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PermissionLeave/5
        public HttpResponseMessage Get(string id, string year)
        {
            try
            {


                bool regisstate = false;
                string msgerror = "";


                string cmdstring = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                System.Data.DataTable dt;

                System.Data.DataSet dts = new System.Data.DataSet("JsonDs");

                dt = ShowLeaveInfo(id, year, "TH");


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
        public static DataTable ShowLeaveInfo(string EmpCode, string Year, string Lang)
        {
            DataTable dt = new DataTable();
            string _Qry;
            string tResetLeave = "";

            string tEmpType = "";
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            string sDate = "";
            string eDate = "";

            sDate = Year + "01/01";
            eDate = Year + "12/31";

            _Qry = @" SELECT FNHSysEmpTypeID
                      FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + @"].dbo.THRMEmployee 
                     WHERE FNHSysEmpID=" + double.Parse(EmpCode);
            tEmpType = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "0");


            _Qry = @" SELECT CASE WHEN RiGHT(FTCurrenDate,5) >=FTLeaveReset THEN LEFT(FTCurrenDate,4) ELSE  LEFT(FTBefore,4)  END +'/' + FTLeaveReset AS FTLeaveReset
              FROM
             (
                 SELECT  TOP 1 Convert(varchar(10),GetDate(),111)  AS FTCurrenDate ,Convert(varchar(10),DateAdd(YEAR,-1,GetDate()),111) AS FTBefore,L.FTLeaveReset
                 FROM  THRMConfigLeave  AS L WITH (NOLOCK)  INNER JOIN THRMEmployee AS M WITH(NOLOCK )
                  ON  L.FNHSysEmpTypeId=M.FNHSysEmpTypeId 
                  WHERE   M.FNHSysEmpID=" + EmpCode + @"
                 ) As T";

            tResetLeave = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");


            string VacationLeaveType = "";
            _Qry = @" SELECT TOP 1 FTCfgData 
                      FROM  ["
                        + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SECURITY) + @"].dbo.TSESystemConfig AS Z WITH(NOLOCK) 
                     WHERE  (FTCfgName = N'VacationLeaveType')";
            VacationLeaveType = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_SECURITY, "0");
            double LeaveVacation = 0;
            if ((VacationLeaveType == "1"))
            {
                _Qry = @"   SELECT  TOP 1  dbo.FN_Get_Emp_Vacation_Th(FNHSysEmpID,FNHSysEmpTypeId,ISNULL(FDDateStart,N''),ISNULL(FDDateEnd,N''),ISNULL(FDDateProbation,N''),'') AS FNEmpVacation 
                  FROM  " + (WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR)) + @".dbo.THRMEmployee  AS M WITH(NOLOCK) 
                  WHERE  M.FNHSysEmpID=" + double.Parse(EmpCode) + " ";
            }
            else
            {
                _Qry = @"   SELECT  TOP 1  dbo.FN_Get_Emp_Vacation(FNHSysEmpID,FNHSysEmpTypeId,ISNULL(FDDateStart,N''),ISNULL(FDDateEnd,N''),ISNULL(FDDateProbation,N'')) AS FNEmpVacation
                   FROM  " + (WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR)) + @".dbo.THRMEmployee  AS M WITH(NOLOCK) 
                      WHERE  M.FNHSysEmpID=" + double.Parse(EmpCode) + " ";
            }

            LeaveVacation = double.Parse(Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "0"));

            _Qry = @" SELECT FTLeaveCode,FTLeaveName, FNLeaveRight, FNLeaveUsed, FNLeaveBal, FNSortSeq FROM 
            (


            SELECT FTLeaveCode,FTLeaveName 

            ,ISNULL(FNLeaveRight,0) AS 'FNLeaveRight'
					            ,ISNULL(FNLeaveUsed,0) AS 'FNLeaveUsed'
					            ,ISNULL(FNLeaveBal,0) AS 'FNLeaveBal'



             FROM  (SELECT V_LeaveType.FTLeaveCode,FTLeaveName

            ,Cast((ISNULL(FNLeaveRight,0) * 480) AS numeric(18,0)) AS FNLeaveRight
            ,ISNULL(FNTotalMinute,0) AS FNLeaveUsed
            ,(Cast((ISNULL(FNLeaveRight,0) * 480) AS numeric(18,0))) - ISNULL(FNTotalMinute,0)   AS FNLeaveBal

             FROM
            (
                 SELECT CAST(FNListIndex AS varchar(3)) AS FTLeaveCode,FTNameTH  AS FTLeaveName 
               FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + @".dbo.V_LeaveType WITH(NOLOCK)
             WHERE FNListIndex<>98
            ) AS V_LeaveType
             Left Join
            (
             Select THRMConfigLeave.FTLeaveCode
            ,CASE WHEN ISNULL(THRMEmployeeLeave.FNLeaveRight,-1)=-1 THEN Cast(ISNULL(THRMConfigLeave.FNLeaveRight,0) AS numeric(18,0)) ELSE Cast(ISNULL(THRMEmployeeLeave.FNLeaveRight,0) AS numeric(18,0)) END AS FNLeaveRight
             FROM
            (
             SELECT FTLeaveCode,FNLeaveRight 
             FROM THRMConfigLeave WITH(NOLOCK) 
             WHERE FNHSysEmpTypeId=" + tEmpType + @" 
            ) THRMConfigLeave
             Left Join
            (
             SELECT FTLeaveCode,Cast(ISNULL(FNLeaveRight,0) AS numeric(18,2)) AS FNLeaveRight 
             FROM THRMEmployeeLeave WITH(NOLOCK) 
             WHERE  FNHSysEmpID=" + EmpCode + @"
             ) THRMEmployeeLeave 
              ON THRMConfigLeave.FTLeaveCode=THRMEmployeeLeave.FTLeaveCode 
             ) T ON V_LeaveType.FTLeaveCode=T.FTLeaveCode
             LEFT JOIN 
             (
             SELECT FTLeaveType,SUM(FNTotalMinute) AS FNTotalMinute 
             FROM THRTTransLeave  WITH(NOLOCK) 
             WHERE  FTLeaveType<>N'98'
             AND FNHSysEmpID=" + EmpCode + @" 
              AND FTDateTrans>=N'" + tResetLeave + @"' 
             GROUP BY FTLeaveType
            ) AS THRTTransLeave
             ON V_LeaveType.FTLeaveCode=THRTTransLeave.FTLeaveType) AS MM1

             UNION 
            SELECT FTLeaveCode,FTLeaveName 

          ,ISNULL(FNLeaveRight,0) AS 'FNLeaveRight'
					,ISNULL(FNLeaveUsed,0) AS 'FNLeaveUsed'
					,ISNULL(FNLeaveBal,0) AS 'FNLeaveBal'


             FROM (SELECT  V_LeaveType.FTLeaveCode,FTLeaveName

            ,Cast((ISNULL(FNLeaveRight," + LeaveVacation + @") * 480)  AS numeric(18,0)) AS FNLeaveRight
            ,ISNULL(FNTotalMinute,0) AS FNLeaveUsed 
            ,(Cast((ISNULL(FNLeaveRight," + LeaveVacation + @") * 480)  AS numeric(18,0))) -ISNULL(FNTotalMinute,0)   AS FNLeaveBal


             FROM
            (
             SELECT CAST(FNListIndex AS varchar(3)) AS FTLeaveCode,FTNameTH AS FTLeaveName 
             FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + @".dbo.V_LeaveType WITH(NOLOCK)
             WHERE FNListIndex=98
            ) AS V_LeaveType
             Left Join
            (
             Select THRMConfigLeave.FTLeaveCode
              ," + LeaveVacation + @" AS FNLeaveRight
             FROM
             (
             SELECT FTLeaveCode,FNLeaveRight 
             FROM THRMConfigLeave WITH(NOLOCK) 
             WHERE  FNHSysEmpTypeId=" + tEmpType + @"
              ) THRMConfigLeave 
             Left Join
              (
              SELECT FTLeaveCode," + LeaveVacation + @" AS FNLeaveRight 
             FROM THRMEmployeeLeave WITH(NOLOCK) 
             WHERE FNHSysEmpID=" + EmpCode + @" 
            ) THRMEmployeeLeave 
             ON THRMConfigLeave.FTLeaveCode=THRMEmployeeLeave.FTLeaveCode 
            ) T ON V_LeaveType.FTLeaveCode=T.FTLeaveCode 
              LEFT JOIN 
            (

             SELECT FTLeaveType,SUM(FNTotalMinute) AS FNTotalMinute 
             FROM THRTTransLeave  WITH(NOLOCK) 
             WHERE  FTLeaveType=N'98'
             AND FNHSysEmpID=" + EmpCode + @" 
             AND FTDateTrans>=N'" + tResetLeave + @"'
             GROUP BY FTLeaveType
             union all 
             SELECT '98' as  FTLeaveType,SUM(FNTotalMinute) AS FNTotalMinute 
             FROM THRTTransLeave  WITH(NOLOCK) 
             WHERE  FTLeaveType=N'999' AND isnull(FTStateDeductVacation,'0') = '1' 
             AND FNHSysEmpID=" + EmpCode + @"
             AND FTDateTrans>=N'" + tResetLeave + @"'
             GROUP BY FTLeaveType

            ) AS THRTTransLeave 
              ON V_LeaveType.FTLeaveCode=THRTTransLeave.FTLeaveType) AS MM2 

 ) AS TL

 LEFT  JOIN (SELECT   FNListIndex ,CASE WHEN ISNULL(FNSortSeq,0) >0 THEN ISNULL(FNSortSeq,0)  ELSE   FNListIndex END FNSortSeq
FROM  HITECH_SYSTEM.dbo.HSysListData WITH(NOLOCK)
where FTListName = 'fnleavetype'
 ) V_LeaveSeq ON TL.FTLeaveCode=V_LeaveSeq.FNListIndex

ORDER BY V_LeaveSeq.FNSortSeq ";




            //_Qry = @"SELECT FTLeaveCode,FTLeaveName 



            // , (( Convert(varchar(30),Convert(numeric(18,0),Floor((ISNULL(FNLeaveRight,0))/ 480.00))))
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),Floor(((ISNULL(FNLeaveRight,0)) % 480.00) / 60.00))),2)
            //   +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),(((ISNULL(FNLeaveRight,0)) % 480.00) % 60.00))),2))  AS FNLeaveRight

            // , (( Convert(varchar(30),Convert(numeric(18,0),Floor((ISNULL(FNLeaveUsed,0))/ 480.00))))
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),Floor(((ISNULL(FNLeaveUsed,0)) % 480.00) / 60.00))),2)
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),(((ISNULL(FNLeaveUsed,0)) % 480.00) % 60.00))),2))  AS FNLeaveUsed

            // , ((Convert(varchar(30),Convert(numeric(18,0),Floor((ISNULL(FNLeaveBal,0))/ 480.00))))
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),Floor(((ISNULL(FNLeaveBal,0)) % 480.00) / 60.00))),2)
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),(((ISNULL(FNLeaveBal,0)) % 480.00) % 60.00))),2))  AS FNLeaveBal

            // FROM  (SELECT V_LeaveType.FTLeaveCode,FTLeaveName

            //,Cast((ISNULL(FNLeaveRight,0) * 480) AS numeric(18,0)) AS FNLeaveRight
            //,ISNULL(FNTotalMinute,0) AS FNLeaveUsed
            //,(Cast((ISNULL(FNLeaveRight,0) * 480) AS numeric(18,0))) - ISNULL(FNTotalMinute,0)   AS FNLeaveBal

            // FROM
            //(
            //     SELECT CAST(FNListIndex AS varchar(3)) AS FTLeaveCode,FTNameTH  AS FTLeaveName 
            //   FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + @".dbo.V_LeaveType WITH(NOLOCK)
            // WHERE FNListIndex<>98
            //) AS V_LeaveType
            // Left Join
            //(
            // Select THRMConfigLeave.FTLeaveCode
            //,CASE WHEN ISNULL(THRMEmployeeLeave.FNLeaveRight,-1)=-1 THEN Cast(ISNULL(THRMConfigLeave.FNLeaveRight,0) AS numeric(18,0)) ELSE Cast(ISNULL(THRMEmployeeLeave.FNLeaveRight,0) AS numeric(18,0)) END AS FNLeaveRight
            // FROM
            //(
            // SELECT FTLeaveCode,FNLeaveRight 
            // FROM THRMConfigLeave WITH(NOLOCK) 
            // WHERE FNHSysEmpTypeId=" + tEmpType + @" 
            //) THRMConfigLeave
            // Left Join
            //(
            // SELECT FTLeaveCode,Cast(ISNULL(FNLeaveRight,0) AS numeric(18,2)) AS FNLeaveRight 
            // FROM THRMEmployeeLeave WITH(NOLOCK) 
            // WHERE  FNHSysEmpID=" + EmpCode + @"
            // ) THRMEmployeeLeave 
            //  ON THRMConfigLeave.FTLeaveCode=THRMEmployeeLeave.FTLeaveCode 
            // ) T ON V_LeaveType.FTLeaveCode=T.FTLeaveCode
            // LEFT JOIN 
            // (
            // SELECT FTLeaveType,SUM(FNTotalMinute) AS FNTotalMinute 
            // FROM THRTTransLeave  WITH(NOLOCK) 
            // WHERE  FTLeaveType<>N'98'
            // AND FNHSysEmpID=" + EmpCode + @" 
            //  AND FTDateTrans>=N'" + tResetLeave + @"' 
            // GROUP BY FTLeaveType
            //) AS THRTTransLeave
            // ON V_LeaveType.FTLeaveCode=THRTTransLeave.FTLeaveType) AS MM1

            // UNION 
            //SELECT FTLeaveCode,FTLeaveName 

            // , (Right( Convert(varchar(30),Convert(numeric(18,0),Floor((ISNULL(FNLeaveRight,0))/ 480.00))),2)
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),Floor(((ISNULL(FNLeaveRight,0)) % 480.00) / 60.00))),2)
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),(((ISNULL(FNLeaveRight,0)) % 480.00) % 60.00))),2))  AS FNLeaveRight

            //, (Right( Convert(varchar(30),Convert(numeric(18,0),Floor((ISNULL(FNLeaveUsed,0))/ 480.00))),2)
            // +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),Floor(((ISNULL(FNLeaveUsed,0)) % 480.00) / 60.00))),2)
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),(((ISNULL(FNLeaveUsed,0)) % 480.00) % 60.00))),2))  AS FNLeaveUsed

            // , (Right( Convert(varchar(30),Convert(numeric(18,0),Floor((ISNULL(FNLeaveBal,0) - (ISNULL(FNLeaveBal,0) % 480) )/ 480.00))),2)
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),Floor(((ISNULL(FNLeaveBal,0)) % 480.00) / 60.00))),2)
            //  +':'+  Right('00' + Convert(varchar(30),Convert(numeric(18,0),(((ISNULL(FNLeaveBal,0)) % 480.00) % 60.00))),2))  AS FNLeaveBal


            // FROM (SELECT  V_LeaveType.FTLeaveCode,FTLeaveName

            //,Cast((ISNULL(FNLeaveRight," + LeaveVacation + @") * 480)  AS numeric(18,0)) AS FNLeaveRight
            //,ISNULL(FNTotalMinute,0) AS FNLeaveUsed 
            //,(Cast((ISNULL(FNLeaveRight," + LeaveVacation + @") * 480)  AS numeric(18,0))) -ISNULL(FNTotalMinute,0)   AS FNLeaveBal


            // FROM
            //(
            // SELECT CAST(FNListIndex AS varchar(3)) AS FTLeaveCode,FTNameTH AS FTLeaveName 
            // FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + @".dbo.V_LeaveType WITH(NOLOCK)
            // WHERE FNListIndex=98
            //) AS V_LeaveType
            // Left Join
            //(
            // Select THRMConfigLeave.FTLeaveCode
            //  ," + LeaveVacation + @" AS FNLeaveRight
            // FROM
            // (
            // SELECT FTLeaveCode,FNLeaveRight 
            // FROM THRMConfigLeave WITH(NOLOCK) 
            // WHERE  FNHSysEmpTypeId=" + tEmpType + @"
            //  ) THRMConfigLeave 
            // Left Join
            //  (
            //  SELECT FTLeaveCode," + LeaveVacation + @" AS FNLeaveRight 
            // FROM THRMEmployeeLeave WITH(NOLOCK) 
            // WHERE FNHSysEmpID=" + EmpCode + @" 
            //) THRMEmployeeLeave 
            // ON THRMConfigLeave.FTLeaveCode=THRMEmployeeLeave.FTLeaveCode 
            //) T ON V_LeaveType.FTLeaveCode=T.FTLeaveCode 
            //  LEFT JOIN 
            //(

            // SELECT FTLeaveType,SUM(FNTotalMinute) AS FNTotalMinute 
            // FROM THRTTransLeave  WITH(NOLOCK) 
            // WHERE  FTLeaveType=N'98'
            // AND FNHSysEmpID=" + EmpCode + @" 
            // AND FTDateTrans>=N'" + tResetLeave + @"'
            // GROUP BY FTLeaveType
            // union all 
            // SELECT '98' as  FTLeaveType,SUM(FNTotalMinute) AS FNTotalMinute 
            // FROM THRTTransLeave  WITH(NOLOCK) 
            // WHERE  FTLeaveType=N'999' AND isnull(FTStateDeductVacation,'0') = '1' 
            // AND FNHSysEmpID=" + EmpCode + @"
            // AND FTDateTrans>=N'" + tResetLeave + @"'
            // GROUP BY FTLeaveType

            //) AS THRTTransLeave 
            //  ON V_LeaveType.FTLeaveCode=THRTTransLeave.FTLeaveType) AS MM2 ";


            dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

            return dt;
        }

           


        //    return dt;
        //}
    





        //POST: api/Employee
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Employee/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Employee/5
        public void Delete(int id)
        {
        }
    }
}
