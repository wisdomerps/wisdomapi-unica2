using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using WSM;
using WSMERPAPI;


namespace WSMHRAPI.Controllers
{
    public class MasterUnisectController : ApiController
    {
        // GET: api/MasterUnisect
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MasterUnisect/5
        public string Get(int id)
        {
            return "value";
        }

        private string verrifyAdd(string id, string Code , String cmpID )
        {
            String _Qry = "";
            String FTCode = "";

            string _Allow = "0";


            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            if (id == "" && Code != "" && cmpID != "")
            {
                _Qry = "SELECT FTUnitSectCode FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].[dbo].[TCNMUnitSect] WHERE  FTUnitSectCode =" + Code + "AND FNHSysCmpId = " + cmpID;
                FTCode = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_MASTER, "");
           
           
            }
            else
            { 


            }
            if (FTCode == "") {
                _Allow = "1";
            }

            return _Allow;


        }


        public HttpResponseMessage Post([FromBody]string value)
        {
            try
            {
                //String A = "";
                //A = value;
                String _Qry = "";

                String _Code = "";
                String _Messege = "";


                WSM.MasterUnisect Udata;
                try
                {
                    string JsonValue = value.ToString();
                    Udata = JsonConvert.DeserializeObject<WSM.MasterUnisect>(JsonValue);

                    //Add Data
                    if (Udata.FNHSysUnitSectId.ToString() == "" && Udata.FTUnitSectCode.ToString() != "" && Udata.FNHSysCmpId.ToString() != "")
                    {
                        if (verrifyAdd(Udata.FNHSysUnitSectId.ToString(), Udata.FTUnitSectCode.ToString(), Udata.FNHSysCmpId.ToString()) == "1")
                        {

                            _Qry = " INSERT INTO [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRTIncentive_Style_Hour (";
                            _Qry += "  FTInsUser, FDInsDate, FTInsTime, FTCalDate, FNHSysUnitSectId";
                            _Qry += ",  FNHSysStyleId, FTOrderNo, FTSubOrderNo, FNStateSewPack, FNSam, FNPricePerSam, FNPriceMultiple,  ";
                            _Qry += "  FNDisPrice,FNNetPrice, FNHour01Qty, FNHour02Qty, FNHour03Qty, FNHour04Qty, FNHour05Qty, FNHour06Qty, FNHour07Qty";


                        }
                        else
                        {
                            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + "Already Code" + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };
                        }


                    }

                    //Update Data
                    if (Udata.FNHSysUnitSectId.ToString() != "" && Udata.FTUnitSectCode.ToString() != "")
                    {
                        
                    }

                }
                catch (Exception ex)
                {
                   //Udata = new WSM.MasterUnisect() { EmpCode = "", EmpName = "", EmpSurName = "", EmpIdCard = "", EmpPhone = "", EmpBirthday = "" };
                }

                


                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + "" + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + ex.Message + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };

            }

        }



        // PUT: api/MasterUnisect/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MasterUnisect/5
        public void Delete(int id)
        {
        }

        [HttpPatch]
        // Patch: api/MasterUnisect/5
        public void Patch(int id)
        {

        }


    }
}
