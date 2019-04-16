using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class Route
    {
        public int route_id { get; set; }
        public int order_detail_id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public DateTime date_time { get; set; }
        LogManager log_manager;

        public Route(string log_file_path)
        {
            log_manager = new LogManager();
            log_manager.file_path = log_file_path;
        }

        public void InsertRoute(Route route)
        {
            try
            {
                log_manager.InsertLog("Route.InsertRoute called");
                SqlCommand cmd = new SqlCommand();
                string sql = DAL.get_sql("insert", "tbl_route", route, new string[] { "route_id", "date_time" }, null, out cmd);
                log_manager.InsertLog("Route.InsertRoute called SQL: " + sql);
                DAL.CreateUpdateDelete(sql, cmd);

                log_manager.InsertLog("Exiting Route.InsertRoute With: OK");
            }
            catch (Exception ex)
            {
                log_manager.InsertLog("Exception: Route.InsertRoute: " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}