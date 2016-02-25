namespace NLog.SignalR
{
    using Microsoft.AspNet.SignalR;
    using System;
    using System.Linq;
    using Microsoft.AspNet.SignalR.Hubs;

    namespace Nlog.SignalR
    {
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public class NLogSignalRAuthorizationAttribute : AuthorizeAttribute
        {

            public string Username { get; set; }

            public string Password { get; set; }

            public virtual bool AuthorizeUser(IRequest request)
            {
                var user = request.User;

                if (user.Identity.IsAuthenticated)
                {
                    var roles = GetUserRoles();
                    var users = GetUsers();

                    if (roles != null)
                    {
                        bool inRole = false;
                        foreach (var role in roles)
                        {
                            if (user.IsInRole(role))
                            {
                                inRole = true;
                                break;
                            }
                        }

                        if (!inRole)
                        {
                            return false;
                        }
                    }

                    if (users != null)
                    {
                        bool isInUsers = false;

                        if (!users.Contains(user.Identity.Name))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                return false;
            }

            public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
            {
                if (AuthorizeUser(request))
                {
                    return true;
                }
                return AuthorizeNLogHub(request);
            }

            private bool AuthorizeNLogHub(IRequest request)
            {

                var base64CredentialString = request.Headers.Get("Authorization");

                if (base64CredentialString == null)
                {
                    return false;
                }

                var credentialBytes = Convert.FromBase64String(base64CredentialString);
                var credentialString = System.Text.Encoding.ASCII.GetString(credentialBytes);
                var credentialSplits = credentialString.Split(':');

                if (credentialSplits.Length != 2)
                {
                    return false;
                }

                var userName = credentialSplits[0];
                var password = credentialSplits[1];

                if (userName == Username && password == Password)
                {
                    return true;
                }
                return false;
            }

            private string[] GetUserRoles()
            {
                return Roles?.Split(',');
            }

            private string[] GetUsers()
            {
                return Users?.Split(',');
            }

            protected override bool UserAuthorized(System.Security.Principal.IPrincipal user)
            {
                return true;
            }
        }
    }

}
