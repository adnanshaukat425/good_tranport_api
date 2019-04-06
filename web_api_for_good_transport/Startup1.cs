using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(web_api_for_good_transport.Startup1))]

namespace web_api_for_good_transport
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCors(CorsOptions.AllowAll);
            app.Map("/signalr", map => {
                var hubConfiguration = new HubConfiguration
                {
                    EnableJSONP = true,
                    EnableJavaScriptProxies = false
                };
                map.RunSignalR(hubConfiguration);
            });
            
        }
    }
}
