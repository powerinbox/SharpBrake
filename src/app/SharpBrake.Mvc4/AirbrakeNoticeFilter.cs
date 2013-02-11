using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SharpBrake;

namespace SharpBrake.Mvc4
{
    public class AirbrakeNoticeFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var extraParams = new Dictionary<string, string>
                                  {
                                      {"controller", (string) filterContext.RouteData.Values["controller"]},
                                      {"action", (string) filterContext.RouteData.Values["action"]}
                                  };

            foreach (var key in filterContext.RouteData.Values.Keys.Where(key => !extraParams.ContainsKey(key)))
            {
                extraParams.Add(key,filterContext.RouteData.Values[key] as string);
            }

            if(filterContext.Result is ViewResult)
            {
                if (!extraParams.ContainsKey("__master"))
                    extraParams.Add("__master", ((ViewResult) filterContext.Result).MasterName);

                if (!extraParams.ContainsKey("__view"))
                    extraParams.Add("__view", ((ViewResult)filterContext.Result).ViewName);
            }

            filterContext.Exception.SendToAirbrake(extraParams);
        }
    }
}