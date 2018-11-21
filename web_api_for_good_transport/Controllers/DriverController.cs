using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using web_api_for_good_transport.Models;

namespace web_api_for_good_transport.Controllers
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
                string sql = @"select user_id, first_name, last_name, email, 
                                phone_number, cnic_number, profile_picture, ut.user_type
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
                string sql = @" select u.* from tbl_users u
                                inner join tbl_driver_vehicle dv
                                on u.user_id = dv.driver_id
                                inner join tbl_vehicle v
                                on v.vehicle_id = dv.vehicle_id
                                where v.tansporter_id = @transporter_id";

                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                
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
    }
}
