using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Configuration;
using System.Net.Http;
using HigiMiddlewareModels;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace HigiWeb.Hubs
{
    public class ProcessHub : Hub
    {
        private static Dictionary<string, HashSet<string>> _connectionStore = new Dictionary<string, HashSet<string>>();

        private static HttpClient httpClient = new HttpClient();

        public void Hello()
        {
            Clients.All.hello();
        }

        public override Task OnConnected()
        {
            var connectionID = Context.ConnectionId;

            _connectionStore.Add(
                    connectionID,
                    new HashSet<string>(new string[] {  }));

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionID = Context.ConnectionId;

            _connectionStore.Remove(connectionID);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var connectionID = Context.ConnectionId;

            if (!_connectionStore.ContainsKey(connectionID))
            {
                _connectionStore.Add(
                    connectionID,
                    new HashSet<string>(new string[] { }));
            }
            
            return base.OnReconnected();
        }

        public static string GetConnectionIdByJobId(string jobID)
        {
            string connectionID = null;

            foreach (var key in _connectionStore.Keys)
            {
                HashSet<string> jobs;
                _connectionStore.TryGetValue(key, out jobs);

                if (jobs.Contains(jobID))
                {
                    connectionID = key;
                    break;
                }
            }

            return connectionID;
        }

        public async Task Send(string region)
        {
            Guid jobID = Guid.NewGuid();
            var connectionID = Context.ConnectionId;
            
            HashSet<string> jobs;
            _connectionStore.TryGetValue(connectionID, out jobs);
            jobs.Add(jobID.ToString());


            string middlewareApiEndPoint = ConfigurationManager.AppSettings["MiddlewareApiEndpoint"];

            UserQuery userQuery = new UserQuery { Region = region, JobID = jobID.ToString() };
            var content = new StringContent(JsonConvert.SerializeObject(userQuery), Encoding.UTF8, "application/json");

            bool isJobQueued = false;

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.PostAsync(middlewareApiEndPoint + "User/FetchRandomUser", content);

            if (response.IsSuccessStatusCode)
            {
                isJobQueued = true;
            }
            
            Clients.Clients(new[] { connectionID }).messageAddedToQueue(jobID, region, isJobQueued);
        }
    }
}