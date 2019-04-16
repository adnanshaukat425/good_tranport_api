﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using web_api_for_good_transport.Models;

namespace web_api_for_good_transport.SignalR
{
    public class TrackingHub : Hub
    {
        private static ConcurrentDictionary<string, string> connected_clients = new ConcurrentDictionary<string, string>();         // <connectionId, userName>
        string log_file_path = HttpContext.Current.Server.MapPath("~/Logs/TrackingLog.txt");
        LogManager log_manager = new LogManager();
        IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<TrackingHub>();

        public override Task OnConnected()
        {
            log_manager.file_path = log_file_path;
            string order_detail_id = Context.QueryString["order_detail_id"];
            order_detail_id = "1";//For testing
            log_manager.InsertLog("---Connecting Hub, order_detail_id: " + order_detail_id + ", ConnectionId: " + Context.ConnectionId + "---");
            connected_clients.TryAdd(Context.ConnectionId, order_detail_id);
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            string id = Context.ConnectionId;
            string order_detail_id;
            connected_clients.TryRemove(id, out order_detail_id);
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
            log_manager.InsertLog("---Reconnecting Hub, order_detail_id: " + order_detail_id + ", ConnectionId: " + Context.ConnectionId + "---");
            connected_clients.TryAdd(Context.ConnectionId, order_detail_id);
            return base.OnReconnected();
        }

        public void InsertLocation(Route route)
        {
            Route insert_route = new Route(log_file_path);
            log_manager.file_path = HttpContext.Current.Server.MapPath("~/Logs/TrackingLog.txt"); ;
            log_manager.InsertLog("--InsertLocation Called-- " + route.latitude + " " + route.longitude);
            insert_route.InsertRoute(route);
            BroadCastRoute(route);
        }

        public void BroadCastRoute(Route route)
        {

            log_manager.InsertLog("---UpdateLocation--" + route.order_detail_id + "---");
            //log_manager.InsertLog("---HubContext--" + hubContext.ToString() + "---");

            String[] broadCastClients = connected_clients.Where(x => x.Value == route.order_detail_id + "").Select(x => x.Key).ToArray();
            log_manager.InsertLog("---UpdateLocation--" + broadCastClients.Count() + "---");
            if (broadCastClients.Count() == 0)
            {
                log_manager.InsertLog("---UpdateLocation-- Broadcasting location to " + broadCastClients.Count() + " clients ---");
                hubContext.Clients.All.UpdateLocation(route);
            }
            else
            {
                log_manager.InsertLog("---UpdateLocation-- multicasting location to " + broadCastClients.Count() + " clients ---");
                hubContext.Clients.Clients(broadCastClients).UpdateLocation(route);
            }
            log_manager.InsertLog("--Successfully updated location");
        }
    }
}