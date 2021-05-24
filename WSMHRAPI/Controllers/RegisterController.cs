using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSMERPAPI;

namespace WSMHRAPI.Controllers
{
    public class RegisterController : ApiController
    {
        // GET: api/Register
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Register/5
        public HttpResponseMessage Get(string id)
        {
            try
            {


                bool regisstate = false;
                string msgerror = "";

      
                    string cmdstring = "";
                    WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                    System.Data.DataTable dt;

                    cmdstring = "select top 1 FTEmpCode ,FNHSysEmpID, FTEmpNameTH, FTEmpSurnameTH, FTEmpNameEN, FTEmpSurnameEN, FDDateStart, FDDateEnd, FNEmpStatus, FTEmpIdNo, FTMobile, FDBirthDate ";
                    cmdstring += " from  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee AS X WITH(NOLOCK)  ";
                    cmdstring += " WHERE FTEmpCode='" + UFuncs.rpQuoted(id) + "'  ";

                    dt = Cnn.GetDataTable(cmdstring, WSM.Conn.DB.DataBaseName.DB_HR);

                    if (dt.Rows.Count > 0)
                    {

                        string empcode = "";
                        string empname = "";
                        string empsurname = "";
                        string empidcard = "";
                        string empphone = "";
                        string empbirthday = "";


                        foreach (System.Data.DataRow R in dt.Rows)
                        {
                            empcode = R["FTEmpCode"].ToString();
                            empname = R["FTEmpNameTH"].ToString();
                            empsurname = R["FTEmpSurnameTH"].ToString();
                            empidcard = R["FTEmpIdNo"].ToString();
                            empphone = R["FTMobile"].ToString();
                            empbirthday = R["FDBirthDate"].ToString();


                            break;
                        };

                       
                         regisstate = true;

              


                    }
                    else
                    {

                        msgerror = "Data Incorrect !!! ";
                    };

  

                if (regisstate)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "1" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + "Data Correct" + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };
                }
                else
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + msgerror + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };
                }


            }
            catch (Exception ex)
            {

                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + ex.Message + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };

            }
        }

        // POST: api/Register
        public HttpResponseMessage Post([FromBody]string value)
        {

            try
            {
                WSM.UserRegisterInfo Udata;
                try
                {
                    string JsonValue = value.ToString();
                    Udata = JsonConvert.DeserializeObject<WSM.UserRegisterInfo>(JsonValue);
                }
                catch
                {
                    Udata = new WSM.UserRegisterInfo() { EmpCode = "", EmpName = "", EmpSurName = "", EmpIdCard = "", EmpPhone="", EmpBirthday="" };
                }

                bool  regisstate = false ;
                string msgerror = "";
        
                if (Udata.EmpCode != "")
                {

                    string cmdstring = "";
                    WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
                    System.Data.DataTable dt;

                    cmdstring = "select top 1 FTEmpCode ,FNHSysEmpID, FTEmpNameTH, FTEmpSurnameTH, FTEmpNameEN, FTEmpSurnameEN, FDDateStart, FDDateEnd, FNEmpStatus, FTEmpIdNo, FTMobile, FDBirthDate ";
                    cmdstring += " from  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].dbo.THRMEmployee AS X WITH(NOLOCK)  ";
                    cmdstring += " WHERE FTEmpCode='" + UFuncs.rpQuoted(Udata.EmpCode) + "'  ";

                    dt = Cnn.GetDataTable(cmdstring, WSM.Conn.DB.DataBaseName.DB_HR);

                    if (dt.Rows.Count > 0) {

                        string empcode = "";
                        string empname = "";
                        string empsurname = "";
                        string empidcard = "";
                        string empphone = "";
                        string empbirthday = "";


                        foreach (System.Data.DataRow R in dt.Rows) {
                            empcode = R["FTEmpCode"].ToString();
                            empname = R["FTEmpNameTH"].ToString();
                            empsurname = R["FTEmpSurnameTH"].ToString();
                            empidcard = R["FTEmpIdNo"].ToString();
                            empphone = R["FTMobile"].ToString();
                            empbirthday = R["FDBirthDate"].ToString();
                           

                            break;
                        };

                        if (Udata.EmpCode == empcode && Udata.EmpName  == empname && Udata.EmpSurName  == empsurname && Udata.EmpIdCard  == empidcard && Udata.EmpPhone  == empphone && Udata.EmpBirthday  == empbirthday)  {
                            regisstate = true;

                        } else {
                            msgerror = "Data Incorrect !!! ";
                        }


                    }
                    else {

                        msgerror = "Data Incorrect !!! ";
                    };

                }
                else {

                    msgerror = "Data Incorrect !!! ";

                }

                if (regisstate)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "1" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + "Data Correct" + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };
                }
                else
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + msgerror + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };
                }


            }
            catch (Exception ex)
            {

                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable, Content = new StringContent("{" + (char)34 + "Status" + (char)34 + ": " + (char)34 + "0" + (char)34 + "," + (char)34 + "Refer" + (char)34 + ": " + (char)34 + ex.Message + (char)34 + "}", System.Text.Encoding.UTF8, "application/json") };

            }

        }

        // PUT: api/Register/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Register/5
        public void Delete(int id)
        {
        }
    }
}
