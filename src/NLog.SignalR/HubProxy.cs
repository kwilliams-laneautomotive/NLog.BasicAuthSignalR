using System;
using System.Net;
using Microsoft.AspNet.SignalR.Client;

namespace NLog.SignalR
{
    public class HubProxy
    {
        private readonly SignalRTarget _target;
        private readonly string _username;
        private readonly string _password;
        public HubConnection Connection;
        private IHubProxy _proxy;

        public HubProxy(SignalRTarget target, string username, string password)
        {
            _target = target;
            _username = username;
            _password = password;
        }

        public void Log(LogEvent logEvent)
        {
            EnsureProxyExists();

            if (_proxy != null)
                _proxy.Invoke(_target.MethodName, logEvent);
        }

        public void EnsureProxyExists()
        {
            if (_proxy == null || Connection == null)
            {
                BeginNewConnection();
            } 

            else if (Connection.State == ConnectionState.Disconnected)
            {
                StartExistingConnection();
            }
        }

        private void BeginNewConnection()
        {
            try
            {
                Connection = new HubConnection(_target.Uri);
                Connection.Credentials = GetBasicAuthenticationCredentials();
                _proxy = Connection.CreateHubProxy(_target.HubName);
                Connection.Start().Wait();

                _proxy.Invoke("Notify", Connection.ConnectionId);
            }
            catch (Exception)
            {
                _proxy = null;
            }
        }

        private void StartExistingConnection()
        {
            try
            {
                Connection.Start().Wait();
            }
            catch (Exception)
            {
                _proxy = null;
            }
        }

        private NetworkCredential GetBasicAuthenticationCredentials()
        {
            if (_username != null && _password != null)
            {
                return new NetworkCredential
                {
                    UserName = _username,
                    Password = _password,
                };
            }
            return null;
        }

    }
}
