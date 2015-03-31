using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    public static class UrlRoute
    {
        public static IUrlRouteRegister Instance = new UrlRouteRegister();

        public static void Register<TModel>(string route)
        {
            Instance.Register(typeof(TModel), route);
        }

        public static void Register(Type type, string route)
        {
            Instance.Register(type, route);
        }

        public static string For<TModel>(TModel routeModel)
        {
            return Instance.For(routeModel);
        }

        public static string For<TModel>()
        {
            return Instance.For<TModel>();
        }
    }
}