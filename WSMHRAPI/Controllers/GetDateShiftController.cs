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
    public class GetDateShiftController : ApiController
    {
        // GET: api/GetDateShift
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetDateShift/5
        public HttpResponseMessage Get(string EmployeeId, string LeaveStartDate, string LeaveEndDate)
        {
            try
            {


                bool regisstate = false;
                string msgerror = "";


                string cmdstring = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                System.Data.DataTable dt;

                System.Data.DataSet dts = new System.Data.DataSet("JsonDs");

                dt = GetDateShiftInfo(EmployeeId, LeaveStartDate, LeaveEndDate);


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
        public static DataTable GetDateShiftInfo(string EmpCode, string StartDate, string EndDate)
        {
            DataTable dt = new DataTable();
            string _Qry;
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            _Qry = @" SELECT  M.FNHSysEmpID, FDShiftDate, M.FNHSysShiftID
                      FROM  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + @"].dbo.THRMEmployeeMoveShift AS M WITH (NOLOCK)
                     INNER JOIN THRMTimeShift AS S WITH (NOLOCK) ON M.FNHSysShiftID = S.FNHSysShiftID
                    WHERE  M.FNHSysEmpID =" + double.Parse(EmpCode) +@"
                     AND M.FDShiftDate >= '"+ StartDate  + "'    AND M.FDShiftDate <= '"+ EndDate  + "'";
          

            dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "");

            return dt;
        }

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
