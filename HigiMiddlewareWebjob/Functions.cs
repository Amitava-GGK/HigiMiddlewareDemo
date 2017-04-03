using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using HigiMiddlewareModels;
using Newtonsoft.Json;
using System.Net.Http;

namespace HigiMiddlewareWebjob
{
    public class Functions
    {
        private static HttpClient httpClient = new HttpClient();
        private const string UINamesAPI = "https://uinames.com/api/";

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static async void ProcessQueueMessage([QueueTrigger("higi-queue")] string message, TextWriter log)
        {
            log.WriteLine(message);

            Console.WriteLine("Query received:\r\n {0}", message);

            UserQuery query = JsonConvert.DeserializeObject<UserQuery>(message);

            StringBuilder apiEndpint = new StringBuilder(UINamesAPI);

            if (query.Region != null)
            {
                apiEndpint.Append(string.Format("?region={0}", query.Region));
            }

            HttpResponseMessage response = await httpClient.GetAsync(apiEndpint.ToString());

            if (response.IsSuccessStatusCode)
            {
                User data = null;

                try
                {
                    data = await response.Content.ReadAsAsync<User>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occured while reading the response content.\r\n {0}", ex);
                }

                Console.WriteLine("Received random user. \r\n {0}", JsonConvert.SerializeObject(data));
            }
            else
            {
                Console.WriteLine("Failed to fetch data from api");
            }
        }
    }
}
