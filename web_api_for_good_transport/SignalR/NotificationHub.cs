using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using web_api_for_good_transport.Models;
using Newtonsoft.Json.Linq;

namespace web_api_for_good_transport.SignalR
{
    public class NotificationHub : Hub
    {
        private static ConcurrentDictionary<string, string> connected_clients = new ConcurrentDictionary<string, string>();         // <connectionId, userName>
        string log_file_path = HttpContext.Current.Server.MapPath("~/Logs/NotificationLog.txt");
        IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        public override Task OnConnected()
        {
            string user_id = Context.Request.Headers["user_id"];

            File.AppendAllText(log_file_path, "---Connecting Hub, user_id: " + user_id + ", ConnectionId: " + Context.ConnectionId + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            connected_clients.TryAdd(Context.ConnectionId, user_id);
            //File.AppendAllText(log_file_path, "---Argument:message -> " + message + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            //DoConnect();
            //Clients.AllExcept(Context.ConnectionId).broadcastMessage(new ChatMessage() { UserName = userName, Message = "I'm Online" });
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            string id = Context.ConnectionId;
            string user_id;
            connected_clients.TryRemove(id, out user_id);
            File.AppendAllText(log_file_path, "---Disconnecting Hub, user_id: " + user_id + ", ConnectionId: " + Context.ConnectionId + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            //Clients.AllExcept(Context.ConnectionId).broadcastMessage(new ChatMessage() { UserName = userName, Message = "I'm Offline" });
            return base.OnDisconnected();
        }

        public void DoConnect(){

        }

        public override Task OnReconnected()
        {
            //DoConnect();

            string user_id = Context.QueryString["user_id"];

            File.AppendAllText(log_file_path, "---Reconnecting Hub, user_id: " + user_id + ", ConnectionId: " + Context.ConnectionId + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            connected_clients.TryAdd(Context.ConnectionId, user_id);

            //Clients.AllExcept(Context.ConnectionId).broadcastMessage(new ChatMessage() { UserName = userName, Message = "I'm Online Again" });
            return base.OnReconnected();
        }

        public void InsertNotification(Notification notification)
        {
            JObject obj = new JObject();
            notification.InsertNotification(out obj);
            BroadCastNotification(notification);
        }

        public void SendNotification(Notification notification)
        {
            File.AppendAllText(log_file_path, "---Calling Method:Send Notification---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            File.AppendAllText(log_file_path, "---Argument:notification_id -> " + notification.notification_id + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);

            BroadCastNotification(notification);
        }

        public void BroadCastNotification(Notification notification)
        {
            File.AppendAllText(log_file_path, "---BroadCasting Notification--" + notification.notification_id + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);

            String[] broadCastClients = connected_clients.Where(x => x.Value == notification.user_id + "").Select(x => x.Key).ToArray();
            hubContext.Clients.Clients(broadCastClients).BroadCastNotification(notification);
        }
    }
}