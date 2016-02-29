using Microsoft.AspNet.SignalR;
using NLog.SignalR;

namespace NLog.SignalR.Sample.Web.Hubs
{

    // When set in the attribute, the Username and Password 
    // will require any NLog targets pointing at this
    // hub to have a matching username and password set

    // The Roles and Users attributes when set will
    // require users accessing the hub as part of the web
    // project to be authenticated and have a matching 
    // username and role

    [NLogSignalRAuthorization(
        Username = "NLogUser",
        Password = "NLogUserPassword"
        //Roles = "Administrators",
        //Users = "Admin"
        )]
    public class LoggingHub : Hub<ILoggingHub>
    {
        public void Log(LogEvent logEvent)
        {
            Clients.Others.Log(logEvent);
        }

    }
}