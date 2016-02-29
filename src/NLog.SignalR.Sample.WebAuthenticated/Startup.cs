using Microsoft.Owin;
using NLog.Config;
using NLog.SignalR;
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

            ConfigurationItemFactory.Default.Targets
          .RegisterDefinition("SignalR", typeof(SignalRTarget));

        }
    }
}
