using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameConsumer.Services
{
    public class ServiceResponse
    {
        public string Response { get; set; }
        public string Message { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
