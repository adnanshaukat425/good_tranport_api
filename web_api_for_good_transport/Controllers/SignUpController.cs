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
                SqlCommand cmd = new SqlCommand();
                string s = @"select count(user_id) from tbl_users where Lower(email) = Lower(@email)";
                cmd.Parameters.AddWithValue("@email", user.email);
                object ob = DAL.SelectScalar(s, cmd);
                int count = ob != null ? Convert.ToInt32(ob.ToString()) : 1;
                if (count > 0)
                {
                    user.user_id = -2;
                    return Request.CreateResponse(HttpStatusCode.OK, user);                   
                }
                cmd = new SqlCommand();
                string sql = DAL.get_sql("insert", "tbl_users", user, new string[] { "user_id" }, "", out cmd) + ";SELECT IDENT_CURRENT('tbl_users')";
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

        [System.Web.Http.Route("api/signup/check_if_email_already_present")]
        [HttpPost]
        public HttpResponseMessage check_if_email_already_present([FromBody] User user)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string s = @"select count(user_id) from tbl_users where Lower(email) = Lower(@email)";
                cmd.Parameters.AddWithValue("@email", user.email);
                object ob = DAL.SelectScalar(s, cmd);
                int count = ob != null ? Convert.ToInt32(ob.ToString()) : 1;
                if (count > 0)
                {
                    user.user_id = -2;
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                cmd = new SqlCommand();
                user.user_id = -1;
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/signup/add_driver_to_transporter")]
        [HttpGet]
        public HttpResponseMessage add_driver_to_transporter(string driver_id, string transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"INSERT INTO tbl_driver_transporter (driver_id, transporter_id) VALUES (@driver_id, @transporter_id)";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                DAL.CreateUpdateDelete(sql, cmd);
                return Request.CreateResponse(HttpStatusCode.OK, true);
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
