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

        [System.Web.Http.Route("api/driver/get_driver_wrt_transporter")]
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

        [System.Web.Http.Route("api/driver/get_active_driver_wrt_transporter")]
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

        [System.Web.Http.Route("api/driver/update_drivers_vehicle")]
        [HttpPost]
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
                cmd.Parameters.AddWithValue("@user_id", user_id);
                if (vehicle_id == "0")
                {
                    cmd.Parameters.AddWithValue("@vehicle_id", DBNull.Value);    
                }
                else
                {
                    cmd.Parameters.AddWithValue("@vehicle_id", vehicle_id);
                }
                
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

        [HttpPost]
        [System.Web.Http.Route("api/driver/get_driver_wrt_order")]
        public HttpResponseMessage get_driver_wrt_order([FromBody] Order order)
        {
            JObject obj = new JObject();
            try
            {
                string sql = "spGetDriverWRTOrder";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@container_type_id", order.container_type_id);
                cmd.Parameters.AddWithValue("@vehicle_type_id", order.vehicle_type_id);

                DataTable driver_details = DAL.Select(sql, cmd);

                sql = "spGetLocationLatLong";
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@location_id", order.source_id);

                DataTable source_details = DAL.Select(sql, cmd);
                driver_details.Columns.Add("distance");
                driver_details.Columns.Add("order_id");
                for (int i = 0; i < driver_details.Rows.Count; i++)
                {
                    driver_details.Rows[i]["distance"] = Math.Sqrt(
                        Math.Pow(Convert.ToSingle(driver_details.Rows[i]["current_latitude"].ToString()) -
                        Convert.ToSingle(source_details.Rows[0]["latitude"].ToString()), 2) +
                        Math.Pow(Convert.ToSingle(driver_details.Rows[i]["current_longitude"].ToString()) -
                        Convert.ToSingle(source_details.Rows[0]["longitude"].ToString()), 2));

                    driver_details.Rows[i]["order_id"] = order.order_id;
                }
                DataView dv = driver_details.DefaultView;
                dv.Sort = "distance asc";
                DataTable sorted_dataTable = dv.ToTable();

                obj["drivers"] = DAL.SerializeDataTable(sorted_dataTable);
                return Request.CreateResponse(HttpStatusCode.OK, obj, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpGet]
        [System.Web.Http.Route("api/driver/get_driver_details_for_customer")]
        public HttpResponseMessage get_driver_details_for_customer(string driver_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = "spGetDriverDetailsForCustomer";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                string driver_details = DAL.SerializeDataTable(sql, cmd);

                obj["success"] = true;
                obj["data"] = driver_details;

                return Request.CreateResponse(HttpStatusCode.OK, obj, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpGet]
        [System.Web.Http.Route("api/driver/get_driver_wrt_order_id")]
        public HttpResponseMessage get_driver_wrt_order_id(string order_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = "spGetDriverWRTOrderId";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@order_id", order_id);
                string driver_details = DAL.SerializeDataTable(sql, cmd);

                obj["drivers"] = DAL.SerializeDataTable(sql, cmd);
                return Request.CreateResponse(HttpStatusCode.OK, obj, Configuration.Formatters.JsonFormatter);
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