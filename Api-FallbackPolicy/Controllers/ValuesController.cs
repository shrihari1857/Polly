using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api_FallbackPolicy.Controllers
{
    public class ValuesController : ApiController
    {
        public const int MAX_RETRIES = 3;

        // GET api/values
        public async Task<IEnumerable<string>> GetAsync()
        {
            var clientA = new RestClient("https://localhost:44372");
            var requestA = new RestRequest("api/values", DataFormat.Json);

            var t = await clientA.GetAsync<IEnumerable<string>>(requestA);


            var _fallbackPolicy =
                Policy<IEnumerable<string>>.Handle<Exception>()
                .FallbackAsync(
                    async (cancellationToken) =>
                    {
                         return await FallbackMethod();
                    });

            var _retryPolicy =
                Policy.Handle<HttpException>(ex => ex.GetHttpCode() == (int)HttpStatusCode.InternalServerError)
                .RetryAsync(MAX_RETRIES);

            return
                await _fallbackPolicy
                .WrapAsync(_retryPolicy)
                .ExecuteAsync(
                    async () =>
                    {
                        var client = new RestClient("https://localhost:44372");
                        var request = new RestRequest("api/values", DataFormat.Json);

                        return await client.GetAsync<IEnumerable<string>>(request);
                    });
        }

        public async Task<IEnumerable<string>> FallbackMethod()
        {
            return 
                await Task.Run(
                    async () => 
                    {
                        var client = new RestClient("https://localhost:44390");
                        var request = new RestRequest("api/values", DataFormat.Json);

                        return await client.GetAsync<IEnumerable<string>>(request);
                    });
        }
    }
}
