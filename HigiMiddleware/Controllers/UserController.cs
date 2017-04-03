using HigiMiddlewareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace HigiMiddleware.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private static HttpClient httpClient = new HttpClient();
        private CloudQueue queue;

        public UserController()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();

            this.queue = queueClient.GetQueueReference("higi-queue");
            this.queue.CreateIfNotExists();

        }

        [HttpPost]
        public async Task<IHttpActionResult> FetchRandomUser(UserQuery query)
        {
            if (query == null)
            {
                query = new UserQuery { Region = null };
            }

            string message = JsonConvert.SerializeObject(query);

            CloudQueueMessage queueMessage = new CloudQueueMessage(message);
            await this.queue.AddMessageAsync(queueMessage);

            return Ok();
        }
    }
}
