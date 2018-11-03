using Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.Http;

namespace Api.Controllers
{
    public class LocationController : ApiController
    {
        [System.Web.Http.Route("api/location/get_location")]
        public IHttpActionResult get_location()
        {
            JObject obj = new JObject();
            dynamic result = null;
            try
            {
                string sql = @"select * from tbl_location";
                obj["success"] = true;
                obj["data"] = JsonConvert.DeserializeObject<dynamic>(DAL.serializeDataTable(sql, new SqlCommand()));
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                result = JsonConvert.DeserializeObject<dynamic>(obj.ToString());
            }

            return Ok ( new { obj });
        }

        [System.Web.Http.Route("api/location/get_location")]
        public IHttpActionResult get_location_by_id(int location_id)
        {
            string result = "";
            JObject obj = new JObject(); 
            try
            {
                string sql = @"select * from tbl_location where location_id = @location_id";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Parameters.AddWithValue("@location_id", location_id);
                obj["success"] = true;
                obj["data"] = JsonConvert.DeserializeObject<dynamic>(DAL.serializeDataTable(sql, new SqlCommand()));
            }
            catch (Exception ex)
            {
                obj["success"] = false; 
                obj["data"] = ex.Message + " " + ex.StackTrace;
                result = JsonConvert.DeserializeObject<dynamic>(obj.ToString());
            }
            return Ok(new { obj });
        }
    }
}
