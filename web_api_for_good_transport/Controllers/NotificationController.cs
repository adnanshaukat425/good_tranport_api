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
        [System.Web.Http.Route("api/notification/get_notification")]
        public HttpResponseMessage get_notification()
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"select * from tbl_notification";
                List<Notification> notification_list = new List<Notification>();
                DataTable dt = DAL.Select(sql);
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
    }
}
