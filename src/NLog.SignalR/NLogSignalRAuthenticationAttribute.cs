namespace NLog.SignalR
{
    using Microsoft.AspNet.SignalR;
    using System;
    using System.Linq;
    using Microsoft.AspNet.SignalR.Hubs;


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

                    if (roles != null && roles.Length > 0)
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

                    if (users != null && users.Length > 0)
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

                if (base64CredentialString.Length <= "Basic ".Length)
                {
                    // String isn't long enough to contain encoded data
                    return false; 
                }

                var baseCredentialStringMinusPrefix = base64CredentialString.Substring("Basic ".Length);

                var credentialBytes = Convert.FromBase64String(baseCredentialStringMinusPrefix);
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
                return Roles?.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }

            private string[] GetUsers()
            {
                return Users?.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }

            protected override bool UserAuthorized(System.Security.Principal.IPrincipal user)
            {
                return true;
            }
        }
    }
