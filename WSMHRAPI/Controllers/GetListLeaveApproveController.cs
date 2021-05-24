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

    public class GetListLeaveApproveController : ApiController
    {
        // GET: api/ListLeaveApprove
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/ListLeaveApprove/5
        public HttpResponseMessage Get(string id, string lang)
        {
            try
            {


                bool regisstate = false;
                string msgerror = "";


                string cmdstring = "";
                WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                System.Data.DataTable dt;

                System.Data.DataSet dts = new System.Data.DataSet("JsonDs");
                if (lang == "")
                {
                    lang = "TH";
                }

                string username = "";

                cmdstring = @" SELECT FTUserName
                                  FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SECURITY) + @"].[dbo].[TSEUserLogin]
                                                  WHERE   M.FNHSysEmpID=" + id + @"";

                username = Cnn.GetField(cmdstring, WSM.Conn.DB.DataBaseName.DB_HR, "");


                string stringcmd = "";
                stringcmd = "EXEC [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.SP_LeaveApproved '" + username + "','"+ lang +"'";
            
                dt = Cnn.GetDataTable(stringcmd, WSM.Conn.DB.DataBaseName.DB_HR, "TableListApprove");
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





        // GET: api/GetListLeaveApprove/5
        //public IHttpActionResult Get(string id, int CmpId)
        //{

        //    DataTable dt = new System.Data.DataTable();

        //    string stringcmd = "";
        //    stringcmd = "EXEC [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.SP_GET_KPITIME " + tcmpid.ToString() + ",'" + syearmonth + "','" + eyearmonth + "'," + tempid + "";
        //    WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

        //    string _cmd;
        //    _cmd = "Select  *  from dbo.fn_GetListLeaveApprove('" + id + "')";
        //    WSM.Conn.SQLConn qLConn = new WSM.Conn.SQLConn();
        //    dt = qLConn.GetDataTable(_cmd, WSM.Conn.DB.DataBaseName.DB_HR);
        //    return Ok(dt);

        //}

        // POST: api/ListLeaveApprove
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ListLeaveApprove/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ListLeaveApprove/5
        public void Delete(int id)
        {
        }
    }
}
