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

    public class GetListDataController : ApiController
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

                if (id == "FNMilitary" || id == "FNMaritalStatus")
                {

                    dt = ShowListData(id);


                    dts.Tables.Add(dt.Copy());

                    string jsondata = JsonConvert.SerializeObject(dts);

                    dt = null;
                    dts = null;

                    return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent(jsondata, System.Text.Encoding.UTF8, "application/json") };

                }
                else
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + "Not Found List data" + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };
                }

            }
            catch (Exception ex)
            {

                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + ex.Message + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };

            }
        }


        public static DataTable ShowListData(string FTListName)
        {
            DataTable dt = new DataTable();
            string _Qry;
            string tResetLeave = "";

            string tEmpType = "";
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();



            _Qry = @" SELECT   [FNListIndex],[FTNameTH]  ,[FTNameEN]
                      FROM " + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SYSTEM) + @".dbo.HSysListData 
              
                      WHERE FTListName = '" + FTListName + "'";


            dt = Cnn.GetDataTable(_Qry, WSM.Conn.DB.DataBaseName.DB_SYSTEM, "");

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
