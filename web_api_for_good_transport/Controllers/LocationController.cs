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

namespace web_api_for_good_transport.Models
{
    public class LocationController : ApiController
    {
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

        [System.Web.Http.Route("api/location/update_current_lat_long")]
        public HttpResponseMessage update_current_lat_long(int user_id, string latitude, string longitude)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"spUpdateLatLong";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@latitude", latitude);
                cmd.Parameters.AddWithValue("@longitude", longitude);

                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = DAL.Select(sql, cmd);
                obj["success"] = true;
                obj["data"] = "updated";
                return Request.CreateResponse(HttpStatusCode.OK, obj);
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
