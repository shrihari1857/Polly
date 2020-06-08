using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api_B.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public async Task<Data> Get()
        {
            //throw new Exception("TestError");
            return await Task.Run(() => new Data { List = new string[] { "Api-B-1", "Api-B-2" } });
        }
    }

    public class Data
    {
        public IEnumerable<string> List { get; set; }
    }
}
