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
using web_api_for_good_transport.SignalR;

namespace web_api_for_good_transport.Controllers
{
    public class NotificationController : ApiController
    {
        NotificationHub notification_hub = new NotificationHub();
        
        [HttpPost]
        public void Post([FromBody] Notification notification)
        {
            notification_hub.BroadCastNotification(notification);
        }

        [HttpPost]
        [System.Web.Http.Route("api/notification/insert_notification")]
        public HttpResponseMessage insert_notification([FromBody] Notification notification)
        {
            JObject obj = new JObject();
            if (notification.InsertNotification(out obj)) {
                notification_hub.BroadCastNotification(notification);
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpGet]
        [System.Web.Http.Route("api/notification/get_broadcast_notification")]
        public HttpResponseMessage get_broadcast_notification(string user_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"select * from tbl_notification where notification_for_user_id = @user_id and is_pushed = 0";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Add("@user_id", user_id);
                List<Notification> notification_list = new List<Notification>();
                DataTable dt = DAL.Select(sql, cmd);
                foreach (DataRow item in dt.Rows)
                {
                    Notification notification = new Notification();
                    notification.notification_id = Convert.ToInt32(item["notification_id"].ToString());
                    notification.notification_message = item["notification_message"].ToString();
                    notification.is_seen = Convert.ToInt32(item["user_id"].ToString());
                    notification.is_pushed = Convert.ToInt32(item["is_seen"].ToString());
                    notification_list.Add(notification);
                }
                return Request.CreateResponse(HttpStatusCode.OK, notification_list);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpPost]
        [System.Web.Http.Route("api/notification/set_notification_to_pushed")]
        public HttpResponseMessage set_notification_to_pushed([FromBody] int[] notification_ids)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"Update tbl_notification set is_pushed = 1 where notification_id in (" + String.Join(",", notification_ids) + ")";

                string row_effected = DAL.CreateUpdateDelete(sql);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [HttpPost]
        [System.Web.Http.Route("api/notification/check_tracking")]
        public HttpResponseMessage check_tracking([FromBody] Route route)
        {
            TrackingHub tracking_hub = new TrackingHub();
            tracking_hub.InsertLocation(route);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [System.Web.Http.Route("api/notification/check_location_broadcasting")]
        public HttpResponseMessage check_location_broadcasting([FromBody] Route route)
        {
            TrackingHub tracking_hub = new TrackingHub();
            tracking_hub.BroadCastRoute(route);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


    }
}