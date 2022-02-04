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
    public class MasterCLeavelDeleteController : ApiController
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
            { string msgCode = "";
                    string msgDesc = "";
                    ActionMasterModel a = new ActionMasterModel();
                //if (MasterCLeavel.FTCLevelCode != "")
                //{
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

                   

                    if (MasterCLeavel.FTCLevelCode.ToString() != "" && MasterCLeavel.FNHSysCLevelId.ToString() != "")
                    {
                        //VerifyData  
                        if (MasterClass.VerrifyUsing(_table, _filed, MasterCLeavel.FTCLevelCode, _sysCmpId, MasterCLeavel.FNHSysCLevelId))
                        {
                           
                            //update
                            if (MasterClass.DeleteMaster(_table, MasterCLeavel.FTCLevelCode, _sysCmpId, PK, MasterCLeavel.FTCLevelNameTH, MasterCLeavel.FTCLevelNameEN, MasterCLeavel.FTRemark, MasterCLeavel.username, MasterCLeavel.FNHSysCLevelId, ref msgDesc))
                            {
                                a.Status = true;
                                a.StatusCode = 200;
                                a.Messege = msgDesc;
                                a.FNHSysMasterID = fnhsysmasterId;

                            }
                            else
                            {
                                a.Status = false;
                                a.StatusCode = 501;
                                a.Messege = "Error";
                                a.FNHSysMasterID = 0;
                            }                                                                                                                                                                                                                                                                                                                                                                                                        
                        }
                        else
                        {
                            a.Status = false;
                            a.StatusCode = 202;
                            a.Messege = "Code has been duplicated.";
                            a.FNHSysMasterID = 0;
                        }
                    }
                    else
                    {
                        a.Status = false;
                        a.StatusCode = 500;
                        a.Messege = "Not found data.";
                        a.FNHSysMasterID = 0;
                    }

                   

                    return Ok(a);
                //}
                //else
                //{
                //    a.Status = false;
                //    a.StatusCode = 500;
                //    a.Messege = "Not found data.";
                //    a.FNHSysMasterID = 0;
                //    return Ok(a);
                //}
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
