using Nancy;
using NancyStack.Configuration;
using NancyStack.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    public interface IRouteHandlerFactory
    {
        IRouteHandlerBuilder<TModel> Get<TModel>(string route);
        IRouteHandlerBuilder<TModel> Delete<TModel>(string route);
        IRouteHandlerBuilder<TModel> Options<TModel>(string route);
        IRouteHandlerBuilder<TModel> Patch<TModel>(string route);
        IRouteHandlerBuilder<TModel> Post<TModel>(string route);
        IRouteHandlerBuilder<TModel> Put<TModel>(string route);
    }

    public class RouteHandlerFactory : IRouteHandlerFactory
    {
        private readonly RouteHandlerContext context;

        public RouteHandlerFactory(INancyModule nancyModule)
        {
            context = new RouteHandlerContext()
            {
                HandlerRegistry = NancyStackWiring.HandlerRegistry,
                ModelBinder = new ModelBinder(),
                NancyModule = nancyModule
            };
        }

        private IRouteHandlerBuilder<TModel> CreateBuilder<TModel>(Func<NancyModule, NancyModule.RouteBuilder> routeBuilder, string route)
        {
            return new RouteHandlerBuilder<TModel>(context, route, new NancyRouteRegister(routeBuilder(context.NancyModule as NancyModule)));
        }

        public IRouteHandlerBuilder<TModel> Get<TModel>(string route)
        {
            return CreateBuilder<TModel>(x => x.Get, route);
        }

        public IRouteHandlerBuilder<TModel> Delete<TModel>(string route)
        {
            return CreateBuilder<TModel>(x => x.Delete, route);
        }

        public IRouteHandlerBuilder<TModel> Options<TModel>(string route)
        {
            return CreateBuilder<TModel>(x => x.Options, route);
        }

        public IRouteHandlerBuilder<TModel> Patch<TModel>(string route)
        {
            return CreateBuilder<TModel>(x => x.Patch, route);
        }

        public IRouteHandlerBuilder<TModel> Post<TModel>(string route)
        {
            return CreateBuilder<TModel>(x => x.Post, route);
        }

        public IRouteHandlerBuilder<TModel> Put<TModel>(string route)
        {
            return CreateBuilder<TModel>(x => x.Put, route);
        }
    }
}
