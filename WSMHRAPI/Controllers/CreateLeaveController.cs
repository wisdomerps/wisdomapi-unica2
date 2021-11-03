using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http;
using WSMHRAPI.HRFunction;
using WSMHRAPI.Models;

namespace WSMHRAPI.Controllers
{
    public class CreateLeaveController : ApiController
    {
        // GET: api/CreateLeave
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CreateLeave/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CreateLeave
        public IHttpActionResult Post(CreateLeaveRequestModel createleave)
        {
            try
            {
                if (createleave.EmployeeId > 0)
                {

                    //VerrifyData 



                    //string _cmd;
                    //_cmd = "Exec  dbo.SP_CreateLeave ";
                    //_cmd += "  @EmpId=" + createleave.EmployeeId ;
                    //_cmd += "  ,@WorkShiftId=" + createleave.WorkShiftId;
                    //_cmd += "  ,@LeaveTypeId=" + createleave.LeaveTypeId;
                    //_cmd += "  ,@LeaveMethod=" + createleave.LeaveMethod;
                    //_cmd += "  ,@StartDate='" + createleave.StartDate + "'";
                    //_cmd += "  ,@EndDate='" + createleave.EndDate + "'";
                    //_cmd += "  ,@Remark='" + createleave.Remark + "'";

                    //WSM.Conn.SQLConn wsm = new WSM.Conn.SQLConn();

                    //wsm.ExecuteOnly(_cmd, WSM.Conn.DB.DataBaseName.DB_HR);
                    byte[] attfile = null;
                    try
                    {
                        attfile = Convert.FromBase64String(createleave.FileBase64);
                    }
                    catch (Exception ex) { }


                    //  string attfiletype = "";

                    ActionModel a = new ActionModel();
                    string msgCode = "";
                    string msgDesc = "";
                    int NextApprovalID = 0;

                    if (HRClass.CreateLeave(createleave.EmployeeId.ToString(), createleave.WorkShiftId.ToString(), createleave.LeaveTypeId.ToString()
                        , createleave.LeaveDayState, createleave.LeaveMethod.ToString(), createleave.LeaveMinutes, createleave.StartDate, createleave.EndDate, createleave.StartTime, createleave.EndTime, createleave.Remark, attfile, createleave.ExtentionFile, ref msgCode, ref msgDesc , ref NextApprovalID))
                    {
                        //Request.CreateResponse(HttpStatusCode.OK);
                        a.Status = true;
                        a.StatusCode = 200;
                        a.Messege = msgDesc;
                        a.NextApprovalID = NextApprovalID;
                        return Ok(a);
                    }
                    else
                    {
                           
                        a.Status = false;
                        a.StatusCode = int.Parse(msgCode);
                        a.Messege = msgDesc;
                        a.NextApprovalID = 0;

                      //  Request.CreateResponse(HttpStatusCode.NotFound, a);
                        return Ok(a);
                    }
                }
                else
                {
                    return Ok(404);
                }
            }
            catch(Exception ex)
            {
                 Request.CreateResponse(HttpStatusCode.NotFound);
                return Ok(404);
            }                    

        }


        // PUT: api/CreateLeave/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CreateLeave/5
        public void Delete(int id)
        {
        }
    }
}
