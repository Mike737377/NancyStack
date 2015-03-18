using Nancy.ViewEngines.Razor;
using NancyStack.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Razor
{
    public static class UrlHelperRouting
    {
        public static string For<TModel, TRouteModel>(this UrlHelpers<TModel> helper, TRouteModel routeModel)
        {
            return UrlRoute.For(routeModel);
        }
    }
}
