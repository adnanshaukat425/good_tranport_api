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
    public class VehicleController : ApiController
    {
        [System.Web.Http.Route("api/vehicle/get_unassigned_vehicle_wrt_transporter")]
        public HttpResponseMessage get_unassigned_vehicle_wrt_transporter(string transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetUnAssignVehicleWrtTransporter";

                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                string result = DAL.SerializeDataTable(sql, cmd);
                var vehicles = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, vehicles);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/vehicle/get_assigned_vehicle_wrt_transporter")]
        public HttpResponseMessage get_assigned_vehicle_wrt_transporter(string transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetAssignVehicleWrtTransporter";

                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                string result = DAL.SerializeDataTable(sql, cmd);
                var vehicles = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, vehicles);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/vehicle/get_all_vehicle_wrt_transporter")]
        public HttpResponseMessage get_all_vehicle_wrt_transporter(string transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetAllVehicleWrtTransporter";

                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                string result = DAL.SerializeDataTable(sql, cmd);
                var vehicles = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, vehicles);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/vehicle/get_vehicle_wrt_transporter_with_driver")]
        public HttpResponseMessage get_vehicle_wrt_transporter_with_driver(string transporter_id, string driver_id)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetVehicleAndDriverWrtTransporter";

                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                string result = DAL.SerializeDataTable(sql, cmd);
                var vehicles = JsonConvert.DeserializeObject(result);

                return Request.CreateResponse(HttpStatusCode.OK, vehicles);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpPost]
        [System.Web.Http.Route("api/vehicle/add_vehicle_wrt_transporter")]
        public HttpResponseMessage add_vehicle_wrt_transporter([FromBody] Vehicle vehicle)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = "SELECT count(*) from tbl_vehicle where LOWER(vehicle_number) = LOWER(@vehicle_number)";
                cmd.Parameters.AddWithValue("@vehicle_number", vehicle.vehicle_number);
                object res_obj = DAL.SelectScalar(sql, cmd);

                if (res_obj != null && !String.IsNullOrEmpty(res_obj.ToString()))
                {
                    if (Convert.ToInt32(res_obj.ToString()) <= 0)
                    {
                        sql = DAL.get_sql("insert", "tbl_vehicle", vehicle, new string[] { "vehicle_id" }, null, out cmd);
                        string result = DAL.CreateUpdateDelete(sql, cmd);
                        if (result != null && !String.IsNullOrEmpty(result) && result != "0")
                        {
                            sql = "select max(vehicle_id) from tbl_vehicle";
                            res_obj = DAL.SelectScalar(sql, cmd);
                            vehicle.vehicle_id = res_obj != null ? Convert.ToInt32(res_obj.ToString()) : 0;
                            return Request.CreateResponse(HttpStatusCode.OK, vehicle);
                        }
                        else
                        {
                            obj["success"] = false;
                            obj["data"] = "Request failed";
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable, obj);
                        }
                    }
                    else
                    {
                        obj["success"] = false;
                        obj["data"] = "Vehicle Already Present";
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, obj);
                    }
                }
                else
                {
                    obj["success"] = false;
                    obj["data"] = "Request failed";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
                }
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpPost]
        [System.Web.Http.Route("api/vehicle/update_vehicle_wrt_transporter")]
        public HttpResponseMessage update_vehicle_wrt_transporter([FromBody] Vehicle vehicle)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = "spUpdateVehicle";
                cmd.Parameters.AddWithValue("@vehicle_id", vehicle.vehicle_id);
                cmd.Parameters.AddWithValue("@vehicle_type_id", vehicle.vehicle_type_id);
                cmd.Parameters.AddWithValue("@container_type_id", vehicle.container_type_id);
                cmd.Parameters.AddWithValue("@vehicle_number", vehicle.vehicle_number);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                object result_ = DAL.SelectScalar(sql, cmd);
                string result = result_ != null ? result_.ToString() : "";

                if (result != null && !String.IsNullOrEmpty(result) && result != "0")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, vehicle);
                }
                else
                {
                    if (result == "Vehicle number already present")
                    {
                        obj["success"] = false;
                        obj["data"] = "Vehicle number already present";
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, obj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, vehicle);
                    }
                }
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpGet]
        [System.Web.Http.Route("api/vehicle/get_vehicle")]
        public HttpResponseMessage get_vehicle(int vehicle_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"select vehicle_id, vehicle_number, coalesce(vehicle_type_id,0) as vehicle_type_id, coalesce(container_type_id, 0) as container_type_id, transporter_id
                            from tbl_vehicle where vehicle_id = @vehicle_id";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@vehicle_id", vehicle_id);

                string result = DAL.SerializeDataTable(sql, cmd);
                List<Vehicle> vehicle = JsonConvert.DeserializeObject<List<Vehicle>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, vehicle[0]);
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
