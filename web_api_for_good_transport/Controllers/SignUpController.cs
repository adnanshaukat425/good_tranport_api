using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using web_api_for_good_transport.Models;

namespace web_api_for_good_transport.Controllers
{
    public class SignUpController : ApiController
    {
        [System.Web.Http.Route("api/signup/get_signup")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] User user)
        {
            JObject obj = new JObject();
            try
            {
                //File.AppendAllText(@"D:\FYP\Log_good_transport.txt", req.ToString() + "OKKK");
                SqlCommand cmd = new SqlCommand();
                string sql = DAL.get_sql("insert", "tbl_users", user, new string[] { "user_id" }, out cmd) + ";SELECT IDENT_CURRENT('tbl_users')";
                string result = DAL.SelectScalar(sql, cmd).ToString();
                user.user_id = Convert.ToInt32(result);
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }
    }
}
