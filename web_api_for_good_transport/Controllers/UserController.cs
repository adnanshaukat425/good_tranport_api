﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using web_api_for_good_transport.Models;

namespace web_api_for_good_transport.Controllers
{
    public class UserController : ApiController
    {
        [System.Web.Http.Route("api/user/get_user_by_id")]
        public HttpResponseMessage get_user_by_id(string user_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"SELECT * from tbl_users where user_id = @user_id";
                SqlCommand cmd = new SqlCommand();
                user_id = HttpUtility.UrlDecode(user_id);

                cmd.Parameters.Add("@user_id", SqlDbType.VarChar).Value = user_id;
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

        [System.Web.Http.Route("api/user/get_user_by_email")]
        public HttpResponseMessage get_user_by_email(string email)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"SELECT * from tbl_users where email = @email";
                SqlCommand cmd = new SqlCommand();
                email = HttpUtility.UrlDecode(email);

                cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
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