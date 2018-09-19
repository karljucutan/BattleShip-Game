using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameConsumer.Services
{
    public class ServiceClientWrapper
    {
        public ServiceResponse Send(ServiceRequest request)
        {
            var stringBuilder = new StringBuilder();
            var baseAddress = request.BaseAddress;
            var method = string.Empty;
            var keys = request.RequestParameters?.Keys.GetEnumerator();
            var parameters = request.RequestParameters;

            try
            {    
                if (keys != null)
                {
                    for (var i = 0; i < parameters.Count; i++)
                    {
                        keys.MoveNext();

                        if (i != 0)
                        {
                            stringBuilder.Append("&");
                        }

                        stringBuilder.Append(string.Format("{0}{1}{2}", keys.Current, "=", parameters[keys.Current]));
                    }        
                }

                baseAddress = $"{baseAddress}?{stringBuilder.ToString()}";

                using (var client = new HttpClient())
                {
                    var response = request.HttpProtocol.Equals(Protocols.HTTP_GET) ? client.GetAsync(baseAddress).Result :
                        request.HttpProtocol.Equals(Protocols.HTTP_POST) ? client.PostAsync(baseAddress, new StringContent(request.Body ?? string.Empty, Encoding.Default, "application/json")).Result :
                        client.PutAsync(baseAddress, new StringContent(request.Body ?? string.Empty, Encoding.Default, "application/json")).Result;

                    using (var sr = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                    {
                        return new ServiceResponse { Response = sr.ReadToEnd(), Status = response.StatusCode };
                    }
                }
            }
            catch (Exception e)
            {
                string message = "A call to {0} failed. " + e.Message + " :::: " + ErrorMessage(e);
                return new ServiceResponse { Status = HttpStatusCode.ServiceUnavailable, Message = message };
            }
        }

        private string ErrorMessage(Exception ex)
        {
            StringBuilder message = new StringBuilder(ex.Message);
            if (ex.InnerException != null)
            {
                message.Append(" - ");
                message.Append(ErrorMessage(ex.InnerException));
            }
            return message.ToString();
        }
    }
}

