using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigiMiddlewareModels
{
    public class JobNotification
    {
        public string JobID { get; set; }

        public string JobStatus { get; set; }

        public User User { get; set; }
    }
}
