using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NLog.SignalR.Sample.WebAuthenticated.Startup))]
namespace NLog.SignalR.Sample.WebAuthenticated
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
