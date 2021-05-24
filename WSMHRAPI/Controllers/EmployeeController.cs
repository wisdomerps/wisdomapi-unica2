using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSMERPAPI;

namespace WSMHRAPI.Controllers
{
    public class EmployeeController : ApiController
    {
        // GET: api/Employee
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Employee/5
        public HttpResponseMessage Get(string id)
        {
            try
            {


                bool regisstate = false;
                string msgerror = "";
                //*

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

        // POST: api/Employee
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Employee/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Employee/5
        public void Delete(int id)
        {
        }
    }
}
