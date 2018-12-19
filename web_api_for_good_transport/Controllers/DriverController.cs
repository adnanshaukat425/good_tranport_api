using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using web_api_for_good_transport.Models;

namespace web_api_for_good_transport.Models
{
    public class DriverController : ApiController
    {
        [System.Web.Http.Route("api/driver/get_drivers")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"select user_id, first_name, last_name, email, 
                                phone_number, cnic_number, profile_picture, ut.user_type
                                from tbl_users u
                                inner join tbl_user_type ut
                                on u.user_type_id = ut.user_type_id
                                where ut.user_type_id = 2";
                
                //User type id for driver is 2.

                string result = DAL.SerializeDataTable(sql);
                var drivers = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, drivers);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/driver/get_driver")]
        [HttpGet]
        public HttpResponseMessage GetDriverById(string driver_id)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"select u.*
                                from tbl_users u
                                inner join tbl_user_type ut
                                on u.user_type_id = ut.user_type_id
                                where ut.user_type_id = 2 and u.user_id = @driver_id";

                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                //User type id for driver is 2.

                string result = DAL.SerializeDataTable(sql, cmd);
                var drivers = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, drivers);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/dirver/get_driver_wrt_transporter")]
        public HttpResponseMessage get_driver_wrt_transporter(string transporter_id)
        {
            //string transporter_id = "7";
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetAllDriversWrtTransporter";

                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                string result = DAL.SerializeDataTable(sql, cmd);
                var drivers = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, drivers);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/dirver/get_active_driver_wrt_transporter")]
        public HttpResponseMessage get_active_driver_wrt_transporter(string transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetActiveDriversWrtTransporter";

                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                cmd.Parameters.AddWithValue("@status", 1);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                string result = DAL.SerializeDataTable(sql, cmd);
                var drivers = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, drivers);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/dirver/update_drivers_vehicle")]
        public HttpResponseMessage update_drivers_vehicle([FromBody] Object json_data)
        {
            JObject obj = new JObject();
            try
            {
                string json = json_data.ToString();
                dynamic obje = JsonConvert.DeserializeObject(json);
                string user_id = obje.user_id;
                string vehicle_id = obje.vehicle_id;
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Add("@user_id", user_id);
                cmd.Parameters.Add("@vehicle_id", vehicle_id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                DataTable dt = DAL.Select("spUpdateDriverVehicle", cmd);
                if (dt.Rows[0]["result"].ToString() == "-1")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Already assign to another driver");
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Assign successfully");
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
