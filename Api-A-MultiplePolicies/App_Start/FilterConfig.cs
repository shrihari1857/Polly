using System.Web;
using System.Web.Mvc;

namespace Api_A_MultiplePolicies
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
