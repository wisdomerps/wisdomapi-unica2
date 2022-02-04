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
    public class MasterPositionDeleteController : ApiController
    {
        // GET: api/MasterPositionDelete
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MasterPositionDelete/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MasterPositionDelete
        public IHttpActionResult Post(MasterPositionModel M)
        {
            try
            {
                if (M.FTPositCode != "")
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

                    if (M.FTPositCode.ToString() != "" && M.FNHSysPositId.ToString() != "")
                    {

                        //VerrifyData  
                        if (MasterClass.VerrifyUsing(_table, PK, M.FTPositCode, M.FNHSysCmpId, M.FNHSysPositId))
                        {
                            //verify Use

                            //update
                            if (MasterClass.DeleteMaster(_table, M.FTPositCode, M.FNHSysCmpId, PK, M.FTPositNameTH, M.FTPositNameEN, M.FTRemark, M.username, M.FNHSysPositId, ref msgDesc))
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


        // PUT: api/MasterDivisionUpdate/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MasterDivisionUpdate/5
        public void Delete(int id)
        {
        }
    }
}
