using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow
{
    class Program
    {
        public const int MAX_RETRIES = 3;
        static void Main(string[] args)
        {
            var retriesLeft = MAX_RETRIES;

            while (retriesLeft > 0)
            {
                try
                {
                    var random = new Random().Next(1, 3);
                    if (random == 1)
                        throw new Exception("Simulated exception");

                    var client = new RestClient("https://localhost:44372");
                    var request = new RestRequest("api/values", DataFormat.Json);

                    var response = client.Get(request);
                    var result = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);

                    Console.WriteLine(string.Join(System.Environment.NewLine, result));
                    break;
                }
                catch (Exception ex)
                {
                    retriesLeft--;

                    if (retriesLeft == 0)
                        throw ex;
                }
            }
            Console.ReadLine();
        }
    }
}
