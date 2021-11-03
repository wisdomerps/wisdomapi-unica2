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
    public class UpdateEmployeeCodeController : ApiController
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
        public IHttpActionResult Post(UpdateEmpCodeModel updateEmpCodeModel)
        {
            try
            {
                if (updateEmpCodeModel.FNHSysEmpID > 0)
                {

                    //VerrifyData 


                    //byte[] attfile = null;
                    //try
                    //{
                    //    attfile = Convert.FromBase64String(createleave.FileBase64);
                    //}
                    //catch (Exception ex) { }


                    //  string attfiletype = "";

                    ActionUpdateEmpCodeModel a = new ActionUpdateEmpCodeModel();

                    int FNHSysEmpID = 0;
                    string FTEmpCode = "";
                    string msgCode = "";
                    string msgDesc = "";

                    if (HRClass.UpdateEmpCode(updateEmpCodeModel, ref FNHSysEmpID, ref FTEmpCode, ref msgCode, ref msgDesc))
                    {
                        //Request.CreateResponse(HttpStatusCode.OK);
                        a.Status = true;
                        a.StatusCode = 200;
                        a.FNHSysEmpID = FNHSysEmpID;
                        a.FTEmpCode = FTEmpCode;
                        a.Messege = msgDesc;
                        return Ok(a);
                    }
                    else
                    {
                           
                        a.Status = false;
                        a.StatusCode = int.Parse(msgCode);
                        a.Messege = msgDesc;


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
