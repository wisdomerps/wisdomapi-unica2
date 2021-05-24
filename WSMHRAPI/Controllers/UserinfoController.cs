using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace WSMHRAPI.Controllers
{
    public class UserinfoController : ApiController
    {
        // GET: api/GetUserJSon
   

        // GET: api/GetUserJSon/5
        [HttpGet]
        [Route("api/Userinfo/{username},{userpassword}")]
        public HttpResponseMessage Get(string username, string userpassword)
        {

            int dEmpId = 0;
            bool dAuthentication = false;
            bool dAuthenusername = false;
            bool dAuthenpassword = false;
            List<WSM.UserDataCmpInfo> CmInfo;

            CmInfo = new List<WSM.UserDataCmpInfo>();

            string stringcmd = "";
            stringcmd = "select top 1 *  FROM ["+ WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SECURITY)  + "].dbo.TSEUserLogin where FTUserName='" + username + "'";
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            System.Data.DataTable dt = Cnn.GetDataTable(stringcmd,WSM.Conn.DB.DataBaseName.DB_SECURITY );

            if (dt.Rows.Count > 0)
            {

                dAuthenusername = true;

                if (WSM.Conn.DB.FuncDecryptData(dt.Rows[0]["FTPassword"].ToString()) == userpassword)
                {
                    dAuthenpassword = true;
                    dEmpId = (int)(Microsoft.VisualBasic.Conversion.Val(dt.Rows[0]["FNHsysEmpId"].ToString()));
                    dAuthentication = true;


                    stringcmd = "  SELECT   C.FNHSysCmpId, C.FTCmpCode";
                    stringcmd += Constants.vbCrLf + " FROM            [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SECURITY) + "].dbo.TSEUserLoginPermission AS A WITH(NOLOCK) INNER JOIN";
                    stringcmd += Constants.vbCrLf + "  [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SECURITY) + "].dbo.TSEPermissionCmp AS B WITH(NOLOCK)  ON A.FNHSysPermissionID = B.FNHSysPermissionID INNER JOIN";
                    stringcmd += Constants.vbCrLf + " [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_MASTER) + "].dbo.TCNMCmp AS C WITH(NOLOCK)  ON B.FNHSysCmpId = C.FNHSysCmpId";
                    stringcmd += Constants.vbCrLf + " WHERE A.FTUserName='" + username + "'";
             
                    stringcmd += Microsoft.VisualBasic.Constants.vbCrLf + " group by   C.FNHSysCmpId, C.FTCmpCode ";


                    System.Data.DataTable dtcmp = Cnn.GetDataTable(stringcmd, WSM.Conn.DB.DataBaseName.DB_SECURITY);

                    if (dtcmp.Rows.Count > 0) {
                        foreach (System.Data.DataRow R in dtcmp.Rows)
                        {
                            WSM.UserDataCmpInfo usercmp = new WSM.UserDataCmpInfo();

                            usercmp.CmpId = (int)Conversion.Val((R["FNHSysCmpId"]).ToString());
                            usercmp.CmpCode = R["FNHSysCmpId"].ToString();

                            CmInfo.Add(usercmp);
                        }

                     }


                }


            };
            

            WSM.UserDataInfo emps = new WSM.UserDataInfo
            {
                EmpId = dEmpId
                                                    ,
                Authentication = dAuthentication
                                                    ,
                Authenusername = dAuthenusername
                                                    ,
                Authenpassword = dAuthenpassword,
                CmpIdInfo = CmInfo
            };

            string jsondata  = JsonConvert.SerializeObject(emps);


            return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted, Content = new StringContent(jsondata, System.Text.Encoding.UTF8, "application/json") };

        }


    }
}
