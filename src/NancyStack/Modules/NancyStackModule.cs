using Nancy;
using Nancy.Security;
using NancyStack.Configuration;
using NancyStack.ModelBinding;
using NancyStack.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Modules
{
    public class NancyStackModule : NancyModule
    {
        private readonly IUrlRouteRegister routeRegister;
        private readonly IRouteHandlerFactory routeFactory;

        public NancyStackModule()
        {
            routeRegister = UrlRoute.Instance;
            routeFactory = new RouteHandlerFactory(this);

            this.Before += CheckCsrfToken;
        }

        private Response CheckCsrfToken(NancyContext ctx)
        {
            if (NancyStackWiring.ConfigurationOptions.CsrfEnabled)
            {
                try
                {
                    if (ctx.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ValidateCsrfToken(TimeSpan.FromDays(7));
                    }
                }
                catch (CsrfValidationException)
                {
                    return Response.AsText("Invalid CSRF token").WithStatusCode(403);
                }
            }
            return null;
        }

        public IRouteHandlerFactory AddRoute
        {
            get
            {
                return routeFactory;
            }
        }



        //protected IRouteHandlerBuilder<TModel> GetRoute<TModel>(string route)
        //{
        //    return routeFactory.Get<TModel>(route);
        //}

        //protected IRouteHandlerBuilder<TModel> PostRoute<TModel>(string route)
        //{
        //    return routeFactory.Post<TModel>(route);
        //}

        //protected IRouteHandlerBuilder<TModel> PutRoute<TModel>(string route)
        //{
        //    return routeFactory.Put<TModel>(route);
        //}

        //protected IRouteHandlerBuilder<TModel> DeleteRoute<TModel>(string route)
        //{
        //    return routeFactory.Delete<TModel>(route);
        //}

        //protected IRouteHandlerBuilder<TModel> PatchRoute<TModel>(string route)
        //{
        //    return routeFactory.Patch<TModel>(route);
        //}

        //protected IRouteHandlerBuilder<TModel> OptionsRoute<TModel>(string route)
        //{
        //    return routeFactory.Options<TModel>(route);
        //}
    }
}