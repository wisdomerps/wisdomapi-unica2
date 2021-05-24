using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WSMHRAPI.Controllers
{
    public class GetListSubordinateLeaveApproveController : ApiController
    {
        // GET: api/GetListSubordinateLeaveApprove
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetListSubordinateLeaveApprove/5
        public IHttpActionResult Get(string id, int CmpId)
        {

            DataTable dt = new System.Data.DataTable();
            string _cmd;
            _cmd = "Select  *  from dbo.fn_GetListSubordinateLeaveApprove('" + id + "')";
            WSM.Conn.SQLConn qLConn = new WSM.Conn.SQLConn();
            dt = qLConn.GetDataTable(_cmd, WSM.Conn.DB.DataBaseName.DB_HR);
            return Ok(dt);

        }
        // POST: api/GetListSubordinateLeaveApprove
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/GetListSubordinateLeaveApprove/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GetListSubordinateLeaveApprove/5
        public void Delete(int id)
        {
        }
    }
}
