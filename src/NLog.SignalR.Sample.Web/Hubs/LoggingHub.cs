using Microsoft.AspNet.SignalR;

namespace NLog.SignalR.Sample.Web.Hubs
{
    [Authorize]
    public class LoggingHub : Hub<ILoggingHub>
    {
        public void Log(LogEvent logEvent)
        {
            Clients.Others.Log(logEvent);
        }
    }
}