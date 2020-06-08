using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api_A_RetryPolicy.Controllers
{
    public class ValuesController : ApiController
    {
        public const int MAX_RETRIES = 3;
        private readonly AsyncRetryPolicy<Data> _retryPolicy;

        public ValuesController()
        {
            _retryPolicy = Policy<Data>.Handle<Exception>()
                .RetryAsync(retryCount: MAX_RETRIES, (ex, i) => 
                { 
                    Console.WriteLine($"Making retry {i} due to {ex}."); 
                });

            #region More examples
            //_retryPolicy = Policy<IEnumerable<string>>.Handle<HttpRequestException>(exception =>
            //{
            //    return exception.Message != "Simulated exception";
            //}).RetryAsync(retryCount: MAX_RETRIES);
            //_retryPolicy = Policy<IEnumerable<string>>.Handle<HttpRequestException>().WaitAndRetryAsync(MAX_RETRIES, sleepDurationProvider: times => TimeSpan.FromMilliseconds(times * 100)); 
            #endregion
        }

        // GET api/values
        public async Task<Data> Get()
        {
            return
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var client = new RestClient("https://localhost:44372");
                    var request = new RestRequest("api/values", DataFormat.Json);

                    return await client.GetAsync<Data>(request);
                });
        }
    }
    public class Data
    {
        public IEnumerable<string> List { get; set; }
    }
}
