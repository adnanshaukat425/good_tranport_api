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

        [HttpPost]
        [System.Web.Http.Route("api/vehicle/add_vehicle_wrt_transporter")]
        public HttpResponseMessage get_all_vehicle_wrt_transporter([FromBody] Vehicle vehicle)
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
    }
}
