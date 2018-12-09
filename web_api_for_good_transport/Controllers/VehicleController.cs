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
        [System.Web.Http.Route("api/vehicle/get_vehicle_wrt_transporter")]
        public HttpResponseMessage get_vehicle_wrt_transporter(string transporter_id)
        {
            JObject obj = new JObject();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetVehicleWrtTransporter";

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
    }
}
