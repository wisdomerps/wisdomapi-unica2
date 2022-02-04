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
using WSMHRAPI.MasterFunction;

namespace WSMHRAPI.Controllers
{
    public class MasterPositionCreateController : ApiController
    {
        // GET: api/MasterDepartmentCreate
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MasterDepartmentCreate/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MasterDepartmentCreate
        public IHttpActionResult Post(MasterPositionModel MD)
        {
            try
            {
                if (MD.FTPositCode != "")
                {
                    string _table = "TCNMPosition";
                    //TCNMCLevel
                    //TCNMCmp
                    //TCNMDivision
                    //TCNMDepartment
                    //TCNMSect
                    //TCNMUnitSect
                    //TCNMPosition
                    //

                    string PK = "FNHSysPositId";
                    int _sysCmpId = 0;  //FNHSysCmpId
                    int fnhsysmasterId = 0;

                    string msgCode = "";
                    string msgDesc = "";
                    ActionMasterModel a = new ActionMasterModel();
                    //VerrifyData 
                    if (MasterClass.VerrifyDuplication(_table, PK, MD.FTPositCode, MD.FNHSysCmpId))
                    {
                        //create
                        if (MasterClass.CreateMaster(_table, MD.FTPositCode, MD.FNHSysCmpId, PK, MD.FTPositNameTH, MD.FTPositNameEN, MD.FTRemark, MD.username, MD.FNEmployeeFormatType, ref fnhsysmasterId, ref msgDesc))
                        {
                            a.Status = true;
                            a.StatusCode = 200;
                            a.Messege = msgDesc;
                            a.FNHSysMasterID = fnhsysmasterId;

                        }
                        else
                        {
                            a.Status = false;
                            a.StatusCode = int.Parse(msgCode);
                            a.Messege = msgDesc;
                            a.FNHSysMasterID = 0;
                        }

                    }
                    else
                    {
                        a.Status = false;
                        a.StatusCode = int.Parse(msgCode);
                        a.Messege = "Code has been duplicated.";
                        a.FNHSysMasterID = 0;
                    }

                    return Ok(a);
              
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


        // PUT: api/MasterDepartmentCreate/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MasterDepartmentCreate/5
        public void Delete(int id)
        {
        }
    }
}
