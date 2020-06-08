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

namespace Api_A_MultiplePolicies.Controllers
{
    public class ValuesController : ApiController
    {
        public const int MAX_RETRIES = 3;
        private AuthenticalService _authenticalService;
        private string _authenticalResult;

        public ValuesController()
        {
            _authenticalService = new AuthenticalService();
        }
        // GET api/values
        public async Task<IEnumerable<string>> Get()
        {
            if (_authenticalResult == null)
                _authenticalResult = await _authenticalService.GetAccessToken();

            var _unauthorizedPolicy =
                Policy.Handle<HttpException>(ex => ex.GetHttpCode() == (int)HttpStatusCode.Unauthorized)
                .RetryAsync(
                    async (ex, retryCount) => 
                    {
                        _authenticalResult = await _authenticalService.GetAccessToken();
                    });

            var _timeoutPolicy =
                Policy.Handle<HttpException>(ex => ex.GetHttpCode() == (int)HttpStatusCode.RequestTimeout)
                .RetryAsync(MAX_RETRIES, 
                    async (ex, retryCount) =>
                    {
                        await Task.Delay(300);
                    });
            
            return 
                await _unauthorizedPolicy
                .WrapAsync(_timeoutPolicy)
                .ExecuteAsync(
                    async () =>
                    {
                        var random = new Random().Next(1, 3);
                        if (random == 1)
                            throw new HttpRequestException("Simulated exception");
                    
                        var client = new RestClient("https://localhost:44372");
                        var request = new RestRequest("api/values", DataFormat.Json);
                    
                        return await client.GetAsync<IEnumerable<string>>(request);
                    });
        }
    }

    public class AuthenticalService
    {
        public async Task<string> GetAccessToken()
        {
            return await Task.Run(() => { return "aasdadasdas334234dfwdfdf"; });
        }
    }
}
