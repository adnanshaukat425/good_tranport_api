using web_api_for_good_transport.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.Http;
using System.Net.Http;
using System.Collections.Generic;
using System.Data;
using System.Net;
using web_api_for_good_transport.SignalR;

namespace web_api_for_good_transport.Models
{
    public class LocationController : ApiController
    {
        TrackingHub tracking_hub = new TrackingHub();
        
        [System.Web.Http.Route("api/location/get_location")]
        public HttpResponseMessage get_location()
        {
            JObject obj = new JObject();
            try
            {
                List<Location> loc = new List<Location>();
                string sql = @"select * from tbl_location";
                DataTable dt = DAL.Select(sql, new SqlCommand());
                foreach (DataRow dr in dt.Rows)
                {
                    Location temp = new Location();
                    temp.location_id = Convert.ToInt32(dr["location_id"].ToString());
                    temp.location_name = dr["location_name"].ToString();
                    temp.longitude = dr["longitude"].ToString();
                    temp.latitude = dr["latitude"].ToString();
                    loc.Add(temp);
                }
                return Request.CreateResponse(HttpStatusCode.OK, loc);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
        }

        [System.Web.Http.Route("api/location/get_location")]
        public HttpResponseMessage get_location_by_id(int location_id)
        {
            JObject obj = new JObject();
            try
            {
                Location loc = new Location();
                string sql = @"select * from tbl_location where location_id = @location_id";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@location_id", location_id);
                DataTable dt = DAL.Select(sql, cmd);
                foreach (DataRow dr in dt.Rows)
                {
                    loc.location_id = Convert.ToInt32(dr["location_id"].ToString());
                    loc.location_name = dr["location_name"].ToString();
                    loc.longitude = dr["longitude"].ToString();
                    loc.latitude = dr["latitude"].ToString();
                }
                return Request.CreateResponse(HttpStatusCode.OK, loc);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/location/get_all_source")]
        public HttpResponseMessage get_all_source()
        {
            JObject obj = new JObject();
            try
            {
                List<Location> loc = new List<Location>();
                string sql = @"spGetAllSource";

                DataTable dt = DAL.Select(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    Location temp = new Location();
                    temp.location_id = Convert.ToInt32(dr["source_id"].ToString());
                    temp.location_name = dr["location_name"].ToString();
                    temp.latitude = dr["latitude"].ToString();
                    temp.longitude = dr["longitude"].ToString();
                    loc.Add(temp);
                }
                return Request.CreateResponse(HttpStatusCode.OK, loc);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
        }

        [System.Web.Http.Route("api/location/get_all_destination_wrt_source")]
        public HttpResponseMessage get_all_destination_wrt_source(string source_id)
        {
            JObject obj = new JObject();
            try
            {
                List<Location> loc = new List<Location>();
                string sql = @"spGetDestinationWRTSource";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@source_id", source_id);
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = DAL.Select(sql, cmd);
                foreach (DataRow dr in dt.Rows)
                {
                    Location temp = new Location();
                    temp.location_id = Convert.ToInt32(dr["destination_id"].ToString());
                    temp.location_name = dr["location_name"].ToString();
                    temp.latitude = dr["latitude"].ToString();
                    temp.longitude = dr["longitude"].ToString();
                    loc.Add(temp);
                }
                return Request.CreateResponse(HttpStatusCode.OK, loc);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
        }

        [HttpGet]
        [System.Web.Http.Route("api/location/update_current_lat_long")]
        public HttpResponseMessage update_current_lat_long(int driver_id, string latitude, string longitude, string driver_name, string transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"spUpdateLatLong";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@user_id", driver_id);//driver_id
                cmd.Parameters.AddWithValue("@latitude", latitude);
                cmd.Parameters.AddWithValue("@longitude", longitude);

                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = DAL.Select(sql, cmd);
                obj["success"] = true;
                obj["data"] = "updated";
                
                tracking_hub.UpdateDriverWrtTransporter(latitude, longitude, driver_id, driver_name, transporter_id);
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }

            
        }

        [HttpGet]
        [System.Web.Http.Route("api/location/get_driver_latLng_from_transporter_id")]
        public HttpResponseMessage get_driver_latLng_from_transporter_id(int transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = "spGetDriverLatLngForTransporter";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);
                cmd.CommandType = CommandType.StoredProcedure;

                string response = DAL.SerializeDataTable(sql, cmd);
                obj["success"] = response != "[]" ? true : false;
                obj["data"] = response;

                return Request.CreateResponse(HttpStatusCode.OK, obj, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj, Configuration.Formatters.JsonFormatter);
            }
        }
    }
}
