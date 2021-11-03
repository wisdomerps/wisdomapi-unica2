using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace WSMHRAPI.Controllers
{
    public class KPITimeController : ApiController
    {

        // GET: api/GetUserJSon/5
        [HttpGet]
        [Route("api/KPITime/{cmpid},{startyear},{startmonth},{endyear},{endmonth},{empid},{StateToBase}")]
        public HttpResponseMessage Get(string cmpid, string startyear, string startmonth, string endyear, string endmonth, string empid, string StateToBase)
        {

            System.Data.DataSet dts = new System.Data.DataSet("JsonDs");
            System.Data.DataTable dt = null;


            int tcmpid = (int)Microsoft.VisualBasic.Conversion.Val(cmpid);
            int tempid = (int)Microsoft.VisualBasic.Conversion.Val(empid);
            int tStateToBase = (int)Microsoft.VisualBasic.Conversion.Val(StateToBase);
            string syearmonth = startyear + '/' + startmonth;
            string eyearmonth = endyear + '/' + endmonth;

            string stringcmd = "";
            stringcmd = "EXEC [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.SP_GET_KPITIME " + tcmpid.ToString() + ",'" + syearmonth +  "','" + eyearmonth + "'," + tempid+ "," +  tStateToBase;
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            dt = Cnn.GetDataTable(stringcmd, WSM.Conn.DB.DataBaseName.DB_HR, "TableKPI");        
            dts.Tables.Add(dt.Copy());

            string jsondata = JsonConvert.SerializeObject(dts);

            dt = null;
            dts = null;

            return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent(jsondata, System.Text.Encoding.UTF8, "application/json") };

        }

        // GET: api/GetUserJSon/5
        [HttpGet]
        [Route("api/KPITimeToBase/{cmpid},{startyear},{startmonth},{endyear},{endmonth},{empid}")]
        public HttpResponseMessage KPITimeToBaseGet(string cmpid, string startyear, string startmonth, string endyear, string endmonth, string empid)
        {

            System.Data.DataSet dts = new System.Data.DataSet("JsonDs");
            System.Data.DataTable dt = null;
            System.Data.DataTable dtresualt = new System.Data.DataTable();
            dtresualt.Columns.Add("Message", typeof(string));
            dtresualt.Columns.Add("FNTotalRec", typeof(int));

            int tcmpid = (int)Microsoft.VisualBasic.Conversion.Val(cmpid);
            int tempid = (int)Microsoft.VisualBasic.Conversion.Val(empid);
            string syearmonth = startyear + '/' + startmonth;
            string eyearmonth = endyear + '/' + endmonth;

            string stringcmd = "";
            stringcmd = "EXEC [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.SP_GET_KPITIME " + tcmpid.ToString() + ",'" + syearmonth + "','" + eyearmonth + "'," + tempid + ",1";
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            dt = Cnn.GetDataTable(stringcmd, WSM.Conn.DB.DataBaseName.DB_HR, "TableKPI");

            if (dt.Rows.Count > 0)
            {
                dtresualt.Rows.Add("Complete", dt.Rows.Count);
            }
            else {
                dtresualt.Rows.Add("Not Found Rec", 0);
            };

            dts.Tables.Add(dtresualt.Copy());

            string jsondata = JsonConvert.SerializeObject(dts);

            dt = null;
            dtresualt = null;
            dts = null;
           
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent(jsondata, System.Text.Encoding.UTF8, "application/json") };

        }

    }
}
