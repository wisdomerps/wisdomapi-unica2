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
    public class MasterCLeavelUpdateController : ApiController
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
        public IHttpActionResult Post(MasterCLeavelModel MasterCLeavel)
        {
            try
            {
                if (MasterCLeavel.FTCLevelCode != "")
                {
                    string _table = "TCNMCLevel";
                    //TCNMCLevel
                    //TCNMCmp
                    //TCNMDivision
                    //TCNMDepartment
                    //TCNMSect
                    //TCNMUnitSect
                    //TCNMPosition
                    //


                    string PK = "FNHSysCLevelId";
                    string _filed = "";
                    int _sysCmpId = 0;  //FNHSysCmpId
                    int fnhsysmasterId = 0;

                    string msgCode = "";
                    string msgDesc = "";
                    ActionMasterModel a = new ActionMasterModel();

                    if (MasterCLeavel.FTCLevelCode.ToString() != "" && MasterCLeavel.FNHSysCLevelId.ToString() != "")
                    {
                        //VerifyData  
                        if (MasterClass.VerrifyDuplication(_table, _filed, MasterCLeavel.FTCLevelCode, _sysCmpId, MasterCLeavel.FNHSysCLevelId))
                        {
                           
                            //update
                            if (MasterClass.UpdateMaster(_table, MasterCLeavel.FTCLevelCode, _sysCmpId, PK, MasterCLeavel.FTCLevelNameTH, MasterCLeavel.FTCLevelNameEN, MasterCLeavel.FTRemark, MasterCLeavel.username, 0, MasterCLeavel.FNHSysCLevelId, MasterCLeavel.FTStateActive, ref msgDesc))
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
