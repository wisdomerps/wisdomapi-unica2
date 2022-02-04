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
    public class MasterUnitsectDeleteController : ApiController
    {
        // GET: api/MasterUnitsectUpdate
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CreateLeave/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MasterUnitsectUpdate
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


                    string PK = "FNHSysUnitSectId";
                    string _filed = "";
                    int _sysCmpId = 0;  //FNHSysCmpId
                    int fnhsysmasterId = 0;

                    string msgCode = "";
                    string msgDesc = "";
                    ActionMasterModel a = new ActionMasterModel();

                    if (MasterUnitSect.FTUnitSectCode.ToString() != "" && MasterUnitSect.FNHSysUnitSectId.ToString() != "")
                    {
                        //VerrifyData  
                        if (MasterClass.VerrifyUsing(_table, _filed, MasterUnitSect.FTUnitSectCode, MasterUnitSect.FNHSysCmpId, MasterUnitSect.FNHSysUnitSectId))
                        {                           
                            //update
                            if (MasterClass.DeleteMaster(_table, MasterUnitSect.FTUnitSectCode, MasterUnitSect.FNHSysCmpId, PK, MasterUnitSect.FTUnitSectNameTH, MasterUnitSect.FTUnitSectNameEN, MasterUnitSect.FTRemark, MasterUnitSect.username, MasterUnitSect.FNHSysUnitSectId, ref msgDesc))
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
                            a.Messege = "Already used. Can not delete.";
                            a.FNHSysMasterID = 0;
                        }
                    }
                    else
                    {
                        a.Status = false;
                        a.StatusCode = int.Parse(msgCode);
                        a.Messege = "Not found data.";
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


        // PUT: api/MasterUnitsectUpdate/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MasterUnitsectUpdate/5
        public void Delete(int id)
        {
        }
    }
}
