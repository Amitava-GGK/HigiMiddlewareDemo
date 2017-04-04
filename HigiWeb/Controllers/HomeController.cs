using HigiMiddlewareModels;
using HigiWeb.Hubs;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HigiWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult JobNotification(JobNotification jobNotification)
        {
            var connectionID = ProcessHub.GetConnectionIdByJobId(jobNotification.JobID);

            if (connectionID != null)
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProcessHub>();
                hubContext.Clients.Clients(new[] { connectionID }).jobCompleted(
                    jobNotification.JobID, 
                    jobNotification.JobStatus, 
                    JsonConvert.SerializeObject(jobNotification.User));
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}