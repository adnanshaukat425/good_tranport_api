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

namespace web_api_for_good_transport.Controllers
{
    public class DashboardGraphController : ApiController
    {
        [System.Web.Http.Route("api/dashboard_graph/top_driver_wrt_transporter")]
        public HttpResponseMessage get_top_driver_wrt_transporter(string period, string transporter_id, string date_from, string date_to)
        {
            //date_from and date_to in yyyy-MM-dd format.
            JObject obj = new JObject();
            try
            {
                string condition_sql = "";
                if (period.Trim().ToLower() == "this week")
                {
                    condition_sql = " CAST(tbl_order_detail.completion_datetime as date) >= cast (GETDATE() - (case when DATEPART(dw, getDate()) = 1 then 6 else DATEPART(dw, getDate()) end) as date) ";
                }
                else if (period.Trim().ToLower() == "last week")
                {
                    condition_sql = @" CAST(tbl_order_detail.completion_datetime as date) >= 
                            cast (GETDATE() - (case when DATEPART(dw, getDate()) = 1 then 6 else DATEPART(dw, getDate()) end) - 7 as date)
                                    and CAST(tbl_order_detail.completion_datetime as date) <= 
                            cast (GETDATE() - (case when DATEPART(dw, getDate()) = 1 then 6 else DATEPART(dw, getDate()) end) - 1 as date) ";
                }
                else if (period.Trim().ToLower() == "this month")
                {
                    condition_sql = @"month(tbl_order_detail.completion_datetime) = month (GETDATE())
                                    and YEAR(tbl_order_detail.completion_datetime) = YEAR(getdate())";
                }
                else if(period.Trim().ToLower() == "last month"){
                    condition_sql = @"month(tbl_order_detail.completion_datetime) = case when MONTH(getDate()) = 1 then 12 else MONTH(getdate()) - 1 end 
                                    and YEAR(tbl_order_detail.completion_datetime) = YEAR(getdate())";
                }
                else if (period.Trim().ToLower() == "other")
                {
                    condition_sql = @"tbl_order_detail.completion_datetime between '" + date_from + "' and '" + date_to + "'";
                }
                string sql = @"select top (5) tbl_order_detail.driver_id, 
                                UPPER(LEFT(tbl_users.first_name,1)) +
                                LOWER(SUBSTRING(tbl_users.first_name, 2, LEN(tbl_users.first_name))) 
                                + ' ' + 
                                UPPER(LEFT(tbl_users.last_name,1)) +
                                LOWER(SUBSTRING(tbl_users.last_name, 2, LEN(tbl_users.last_name))) as name, COUNT(order_detail_id) as total_orders from tbl_order_detail
                                inner join tbl_users on tbl_order_detail.driver_id = tbl_users.user_id
                                inner join tbl_driver_transporter on tbl_driver_transporter.driver_id = tbl_users.user_id
                                where  " + condition_sql + @" and tbl_driver_transporter.transporter_id = 7 
                                and tbl_order_detail.status_id = 5
                                group by tbl_order_detail.driver_id, tbl_users.first_name, tbl_users.last_name
                                order by AVG(tbl_order_detail.rating)";

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@transporter_id", transporter_id);

                DataTable dt = DAL.Select(sql, cmd);

                List<object> x_axis = new List<object>();
                List<object> y_axis = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    x_axis.Add(dr["name"].ToString());
                    y_axis.Add(dr["total_orders"].ToString());
                }

                DashboardGraph dashboardGraph = new DashboardGraph();
                dashboardGraph.x_axis = x_axis;
                dashboardGraph.y_axis = y_axis;

                return Request.CreateResponse(HttpStatusCode.OK, dashboardGraph);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
        }

        [System.Web.Http.Route("api/dashboard_graph/get_last_six_month_order_of_driver")]
        public HttpResponseMessage get_last_six_month_order_of_driver(string driver_id)
        {
            JObject obj = new JObject();

            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"spGetLastSixMonthOrderOfDriver";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                DataTable dt = DAL.Select(sql, cmd);

                List<object> x_axis = new List<object>();
                List<object> y_axis = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    x_axis.Add(dr["month"].ToString());
                    y_axis.Add(dr["count"].ToString());
                }

                DashboardGraph dashboardGraph = new DashboardGraph();
                dashboardGraph.x_axis = x_axis;
                dashboardGraph.y_axis = y_axis;

                return Request.CreateResponse(HttpStatusCode.OK, dashboardGraph);

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
