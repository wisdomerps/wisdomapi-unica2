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
    public class MasterUnitsectCreateController : ApiController
    {
        // GET: api/MasterUnitsectCreate
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
        public IHttpActionResult Post(MasterUnitSectModel MasterUnitSect)
        {
            try
            {
                if (MasterUnitSect.FTUnitSectCode != "")
                {
                    string _table = "TCNMUnitSect";
                    //TCNMCLevel
                    //TCNMCmp
                    //TCNMDivision
                    //TCNMDepartment
                    //TCNMSect
                    //TCNMUnitSect
                    //TCNMPosition
                    //

                    string PK = "FNHSysUnitsectId";
                    int _sysCmpId = 0;  //FNHSysCmpId
                    int fnhsysmasterId = 0;

                    string msgCode = "";
                    string msgDesc = "";
                    ActionMasterModel a = new ActionMasterModel();
                    //VerrifyData 
                    if (MasterClass.VerrifyDuplication(_table, PK, MasterUnitSect.FTUnitSectCode, MasterUnitSect.FNHSysCmpId))
                    {
                        //create
                        if (MasterClass.CreateMaster(_table, MasterUnitSect.FTUnitSectCode, MasterUnitSect.FNHSysCmpId, PK, MasterUnitSect.FTUnitSectNameTH, MasterUnitSect.FTUnitSectNameEN, MasterUnitSect.FTRemark, MasterUnitSect.username, 0, ref fnhsysmasterId, ref msgDesc))
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


        // PUT: api/MasterUnisectCreate/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MasterUnisectCreate/5
        public void Delete(int id)
        {
        }
    }
}
