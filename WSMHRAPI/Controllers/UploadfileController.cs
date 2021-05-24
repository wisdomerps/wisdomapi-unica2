using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace WSMHRAPI.Controllers
{
    public class UploadfileController : ApiController
    {
        // GET: api/Uploadfile
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Uploadfile/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Uploadfile
        public async Task<HttpResponseMessage> Uploadfile(int fnhsysempid, string LeaveType, string FDStartDate, string EndDate)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = System.Web.Hosting.HostingEnvironment.MapPath("~/fileattach");
            var provider = new MultipartFormDataStreamProvider(root);
            int result = 0;
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                int x = 0;
                foreach (MultipartFileData file in provider.FileData)
                {


                    byte[] bytes = System.IO.File.ReadAllBytes(file.LocalFileName);
                    var orname = file.Headers.ContentDisposition.Name.ToString();
                    string[] subs = orname.Split('|');


                    string _cmd = "";
                    //_cmd = "exec  dbo.sp_attachfile";
                    //SqlCommand cmd = new SqlCommand(cmd, SQLConn.Cnn);


                    //        string sql =
                    //"INSERT INTO Production.ProductCategory (Name) VALUES (@Name); "
                    //+ "SELECT CAST(scope_identity() AS int)";

                 

                    //string fnhsysempid = "";
                    string leavetype = "";
                  //  string FDStartDate = "";
                    string FDEndDate = "";
                    string fileType = file.Headers.ContentType.MediaType.ToString();

                    string sql =   "exec dbo.sp_attachfile @id, @leaveType, @FDStartDate, @FDEndDate , @file, @fileType";
                    using (SqlConnection conn = new SqlConnection(WSM.Conn.DB.ConnectionString(WSM.Conn.DB.DataBaseName.DB_HR)))
                    {
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = fnhsysempid;
                        cmd.Parameters.Add("@leaveType", SqlDbType.VarChar).Value = leavetype;
                        cmd.Parameters.Add("@FDStartDate", SqlDbType.VarChar).Value = FDStartDate;
                        cmd.Parameters.Add("@FDEndDate", SqlDbType.VarChar).Value = FDEndDate;
                        cmd.Parameters.Add("@file", SqlDbType.VarBinary).Value = bytes;
                        cmd.Parameters.Add("@fileType", SqlDbType.VarChar).Value = fileType;



                        try
                        {
                            conn.Open();
                            result = (Int16)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                if (result == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotModified,"");
                }

            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }




          //  return Request.CreateResponse(HttpStatusCode.OK);


        }

        // PUT: api/Uploadfile/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Uploadfile/5
        public void Delete(int id)
        {
        }
    }
}