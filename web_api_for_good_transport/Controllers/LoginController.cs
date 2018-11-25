using web_api_for_good_transport.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class LoginController : ApiController
    {
        [System.Web.Http.Route("api/login/login")]
        public HttpResponseMessage get_login(string email, string password)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"SELECT SmartTransport.dbo.isLogin(@email, @password)";
                SqlCommand cmd = new SqlCommand();
                email = HttpUtility.UrlDecode(email);
                password = HttpUtility.UrlDecode(password);
                
                cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

                bool _result = (bool)DAL.SelectScalar(sql, cmd);
                obj["success"] = true;
                obj["is_logged_in"] = _result;
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                obj["is_logged_in"] = false;
            }

            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

        [System.Web.Http.Route("api/login/login_details")]
        public HttpResponseMessage get_login_details(string email, string password)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"SELECT * from tbl_users where SmartTransport.dbo.isLogin(@email, @password) = 1 AND email = @email";
                SqlCommand cmd = new SqlCommand();
                email = HttpUtility.UrlDecode(email);
                password = HttpUtility.UrlDecode(password);

                cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
                User user = new User();

                DataTable dt = DAL.Select(sql, cmd);
                foreach (DataRow dr in dt.Rows)
                {
                    user.user_id = Convert.ToInt32(dr["user_id"].ToString());
                    user.user_type_id = Convert.ToInt32(dr["user_type_id"].ToString());
                    user.first_name = dr["first_name"].ToString();
                    user.last_name = dr["last_name"].ToString();
                    user.email = dr["email"].ToString();
                    user.phone_number = dr["phone_number"].ToString();
                    user.cnic_number = dr["cnic_number"].ToString();
                    user.profile_picture = dr["profile_picture"].ToString();
                    user.password = dr["password"].ToString();
                }
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
