using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_A.Controllers
{
    public class ValuesController : ApiController
    {
        public const int MAX_RETRIES = 3;
        
        // GET api/values
        public IEnumerable<string> Get()
        {
            var retriesLeft = MAX_RETRIES;
            IEnumerable<string> result = new List<string>();

            while (retriesLeft > 0)
            {
                try
                {
                    //var random = new Random().Next(1, 3);
                    //if (random == 1)
                    //    throw new HttpRequestException("Simulated exception");
                    
                    var client = new RestClient("https://localhost:44372");
                    var request = new RestRequest("api/values", DataFormat.Json);

                    var response = client.Get(request);
                    result = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);
                    break;
                }
                catch (Exception ex)
                {
                    retriesLeft--;
                    
                    if(retriesLeft == 0)
                        throw ex;
                }
            }
            return result;
        }
    }
}
