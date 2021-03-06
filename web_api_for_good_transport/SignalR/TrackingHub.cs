﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using web_api_for_good_transport.Models;
using System.Data;
using System.Threading;

namespace web_api_for_good_transport.SignalR
{
    public class TrackingHub : Hub
    {
        private static ConcurrentDictionary<string, string> connected_clients = new ConcurrentDictionary<string, string>();         // <connectionId, userName>

        private static ConcurrentDictionary<string, string> connected_clients_for_transporter = new ConcurrentDictionary<string, string>(); 
        string log_file_path = HttpContext.Current.Server.MapPath("~/Logs/TrackingLog.txt");
        LogManager log_manager = new LogManager();
        IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<TrackingHub>();

        public override Task OnConnected()
        {
            log_manager.file_path = log_file_path;
            string order_detail_id = Context.Request.Headers["order_detail_id"];
            string user_id = Context.Request.Headers["user_id"];
            //order_detail_id = "1";//For testing
            log_manager.InsertLog("---Connecting Hub, order_detail_id: " + order_detail_id + ", User_id "+ user_id +", ConnectionId: " + Context.ConnectionId + "---");
            connected_clients.TryAdd(Context.ConnectionId, order_detail_id);
            connected_clients_for_transporter.TryAdd(Context.ConnectionId, user_id);
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            string id = Context.ConnectionId;
            string order_detail_id;
            string user_id;
            connected_clients.TryRemove(id, out order_detail_id);
            connected_clients_for_transporter.TryRemove(Context.ConnectionId, out user_id);
            log_manager.InsertLog("---Disconnecting Hub, order_detail_id: " + order_detail_id + ", ConnectionId: " + Context.ConnectionId + "---");
            return base.OnDisconnected();
        }

        public void DoConnect()
        {

        }

        public override Task OnReconnected()
        {
            //DoConnect();
            string order_detail_id = Context.QueryString["order_detail_id"];
            string user_id = Context.QueryString["user_id"];
            log_manager.InsertLog("---Reconnecting Hub, order_detail_id: " + order_detail_id + ", ConnectionId: " + Context.ConnectionId + "---");
            connected_clients.TryAdd(Context.ConnectionId, order_detail_id);
            connected_clients_for_transporter.TryAdd(Context.ConnectionId, user_id);
            return base.OnReconnected();
        }

        public void InsertLocation(Route route)
        {
            Route insert_route = new Route(log_file_path);
            log_manager.file_path = HttpContext.Current.Server.MapPath("~/Logs/TrackingLog.txt");
            log_manager.InsertLog("--InsertLocation Called-- " + route.latitude + " " + route.longitude);
            insert_route.InsertRoute(route);
            BroadCastRoute(route);
        }

        public void BroadCastRoute(Route route)
        {
            log_manager.InsertLog("---UpdateLocation--" + route.order_detail_id + "---");
            string data = route.latitude + ";" + route.longitude;
            String[] broadCastClients = connected_clients.Where(x => x.Value == route.order_detail_id + "").Select(x => x.Key).ToArray();
            log_manager.InsertLog("---UpdateLocation--" + broadCastClients.Count() + "---");
            if (broadCastClients.Count() == 0)
            {
                log_manager.InsertLog("---UpdateLocation-- Broadcasting location to " + broadCastClients.Count() + " clients ---");
                hubContext.Clients.All.UpdateLocation(data);
            }
            else
            {
                log_manager.InsertLog("---UpdateLocation-- multicasting location to " + broadCastClients.Count() + " clients ---");
                hubContext.Clients.Clients(broadCastClients).UpdateLocation(data);
            }
            log_manager.InsertLog("--Successfully updated location");
            log_manager.InsertLog("-----------------------Connected Clients (" + connected_clients.Count + ")-----------------------");
        }

        public void UpdateDriverWrtTransporter(string latitude, string longitude, int driver_id, string driver_name, string user_id)
        {
            log_manager.file_path = HttpContext.Current.Server.MapPath("~/Logs/TrackingLog.txt");
            log_manager.InsertLog("---UpdateDriverLocation--" + driver_id + ", " + user_id + "---");
            string data = latitude + ";" + longitude + ";" + driver_id + ";" + driver_name;
            String[] broadCastClients = connected_clients_for_transporter.Where(x => x.Value == user_id + "").Select(x => x.Key).ToArray();

            log_manager.InsertLog("---UpdateDriverLocation--" + broadCastClients.Count() + "---");
            hubContext.Clients.Clients(broadCastClients).UpdateDriverLocationForTransporter(data);

            log_manager.InsertLog("--Successfully updated Driver location");
            log_manager.InsertLog("-----------------------Connected Clients (" + connected_clients_for_transporter.Count + ")-----------------------");
        }

        private void infinite_routing(LogManager log_manager, Route route, IHubContext hubContext)
        {
            try
            {
            //restart:
                log_manager.InsertLog("Getting lat long");
                string sql = "select latitude, longitude from tbl_route";
                DataTable dt = DAL.Select(sql);
                log_manager.InsertLog("Starting broadcast");
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    route.latitude = dt.Rows[i][0].ToString();
                //    route.longitude = dt.Rows[i][1].ToString();
                //    log_manager.InsertLog("Latitude: " + route.latitude);
                //    log_manager.InsertLog("longitude: " + route.longitude);
                //    log_manager.InsertLog("sending lat long to ");
                //    string data = route.latitude + ";" + route.longitude;
                //    hubContext.Clients.All.UpdateLocation(data);
                //    log_manager.InsertLog("successfully sent");
                //    Thread.Sleep(2000);
                //}
                //goto restart;
            }
            catch (Exception ex)
            {
                log_manager.InsertLog("Excetion in sending route: " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}