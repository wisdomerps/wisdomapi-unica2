using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSMHRAPI.Models;
using WSMHRAPI.HRFunction;
using Newtonsoft.Json;

namespace WSMHRAPI.Controllers
{
    public class ValidateDayLeaveController : ApiController
    {
        // GET: api/ValidateDayLeave
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ValidateDayLeave/5
        public string Get(LeaveModel id)
        {
            return "value";
        }

        // POST: api/ValidateDayLeave
      
        public IHttpActionResult Post(LeaveModel _leave)
        {
            try
            {
                if (_leave.EmployeeId > 0)
                {
                    ActionModel a = new ActionModel();
                    string msgCode = "";
                    string msgDesc = "";
                    int FTNetDay = 0;
                    if (HRClass.VerrifyDay(_leave.EmployeeId.ToString(), _leave.LeaveTypeId.ToString(), _leave.StartDate, _leave.EndDate, ref msgCode, ref msgDesc, ref FTNetDay))
                    {
                        //  ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, FTNetDay));
                        //  return Ok(FTNetDay);

                        //string jsondata = JsonConvert.SerializeObject(FTNetDay);
                        object dayleave = new { day = FTNetDay };

                        return Ok(dayleave);
                        //var unserializedContent = JsonConvert.DeserializeObject(FTNetDay);
                       // return Json(unserializedContent);
                        //return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent(jsondata, System.Text.Encoding.UTF8, "application/json") };
                    }                     
                    else
                    {
                        a.Status = false;
                        a.StatusCode = int.Parse(msgCode);
                        a.Messege = msgDesc;

                        return Ok(a);
                     //   Request.CreateResponse(HttpStatusCode.NotFound, a);
                    }


                }
                else
                {
                    return Ok();
                }
            }
            catch(Exception rt)
            {
                return Ok(rt.Message);// Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }



        // PUT: api/ValidateDayLeave/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ValidateDayLeave/5
        public void Delete(int id)
        {
        }
    }
}
