using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSMHRAPI.Models;
using WSMHRAPI.HRFunction;

namespace WSMHRAPI.Controllers
{
    public class ActionLeaveController : ApiController
    {
        // GET: api/ActionLeave
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ActionLeave/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ActionLeave
        public IHttpActionResult Post(LeaveActionModel ApproveLeave)
        {
            try  
            {
                //d
                ActionModel a = new ActionModel();
                int msgCode = 0;
                string msgDesc = "";

                if (ApproveLeave.EmployeeId > 0)
                {

                    if (HRClass.SendApprove(ApproveLeave.EmployeeId, ApproveLeave.LeaveTypeId
                                            , ApproveLeave.StartDate, ApproveLeave.EndDate, ApproveLeave.ApproverId, ApproveLeave.FTStateType, ApproveLeave.ActionType, ref msgCode, ref msgDesc))
                    {
                        //Request.CreateResponse(HttpStatusCode.OK);
                        a.Status = true;
                        a.StatusCode = msgCode;
                        a.Messege = msgDesc;
                        return Ok(a);
                    }
                    else
                    {
                        a.Status = false;
                        a.StatusCode = msgCode;
                        a.Messege = msgDesc;
                        return Ok(a);
                    }

                }
                else
                {
                    a.Status = false;
                    a.StatusCode = 404;
                    a.Messege = msgDesc;
                    return Ok(a);
                }
            }
            catch (Exception ex)
            {
                Request.CreateResponse(HttpStatusCode.NotFound);
                return Ok(404);
            }

        }




        // PUT: api/ActionLeave/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ActionLeave/5
        public void Delete(int id)
        {
        }
    }
}
