using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class Notification
    {
        public int notification_id { get; set; }
        public string notification_message { get; set; }
        public int user_id { get; set; }
        public int is_seen { get; set; }
        public int is_pushed { get; set; }
        public string redirected_page { get; set; }
        public string notification_for { get; set; }
        public string notification_for_user_id { get; set; }
        public string notification_title { get; set; }

        public bool InsertNotification(out JObject obj)
        {
            obj = new JObject();
            try
            {
                string sql = @"spInsertNotification";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@notification_message", this.notification_message);
                cmd.Parameters.AddWithValue("@user_id", this.user_id);
                cmd.Parameters.AddWithValue("@is_seen", this.is_seen);
                cmd.Parameters.AddWithValue("@is_pushed", this.is_pushed);
                cmd.Parameters.AddWithValue("@redirected_page", this.redirected_page);
                cmd.Parameters.AddWithValue("@notification_for_user_id", this.notification_for_user_id);
                cmd.Parameters.AddWithValue("@notification_for", this.notification_for);
                cmd.Parameters.AddWithValue("@notification_title", this.notification_title);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                DAL.CreateUpdateDelete(sql, cmd);
                return true;
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return false;
            }
        }
    }
}