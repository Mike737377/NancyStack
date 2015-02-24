using Nancy;
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

        public NancyStackModule()
        {
            routeRegister = UrlRoute.Instance;
        }

        public virtual IApplicationBus Bus
        {
            get
            {
                return ServiceFactory.Container.GetInstance<IApplicationBus>();
            }
        }

        protected IRouteHandlerBuilder<TModel> GetRoute<TModel>(string route)
        {
            return new RouteHandlerBuilder<TModel>(this, route, Get);
        }

        protected IRouteHandlerBuilder<TModel> PostRoute<TModel>(string route)
        {
            return new RouteHandlerBuilder<TModel>(this, route, Post);
        }

        protected IRouteHandlerBuilder<TModel> PutRoute<TModel>(string route)
        {
            return new RouteHandlerBuilder<TModel>(this, route, Put);
        }

        protected IRouteHandlerBuilder<TModel> DeleteRoute<TModel>(string route)
        {
            return new RouteHandlerBuilder<TModel>(this, route, Delete);
        }

        protected IRouteHandlerBuilder<TModel> PatchRoute<TModel>(string route)
        {
            return new RouteHandlerBuilder<TModel>(this, route, Patch);
        }
        
        protected IRouteHandlerBuilder<TModel> OptionsRoute<TModel>(string route)
        {
            return new RouteHandlerBuilder<TModel>(this, route, Options);
        }
    }
}
