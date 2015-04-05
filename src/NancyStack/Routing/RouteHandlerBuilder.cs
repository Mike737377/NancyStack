using Nancy;
using Nancy.Security;
using NancyStack.Configuration;
using NancyStack.ModelBinding;
using NancyStack.Modules;
using NancyStack.Routing.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    internal static class ValidationRouteHandlers
    {
        internal static readonly Dictionary<Type, Func<NancyStackModule, dynamic>> Default = new Dictionary<Type, Func<NancyStackModule, dynamic>>();
    }

    public class ReturningRouteHandlerBuilder<TModel, TReturnModel> :
        IReturingWithAuthRouteHandlerBuilder<TModel, TReturnModel>,
        IReturningRouteHandlerBuilder<TModel, TReturnModel>
    {
        private readonly NancyModule.RouteBuilder actionBuilder;
        private readonly NancyStackModule module;
        private readonly string route;
        private List<string> claims;
        private List<OnHandler<TReturnModel>> onHandlers = new List<OnHandler<TReturnModel>>();
        private Func<OnScenarioContext<TReturnModel>, dynamic> onSuccess;
        private IValidationErrorHandler onValidationError;
        private bool withAuthentication;

        public ReturningRouteHandlerBuilder(NancyStackModule module, string route, NancyModule.RouteBuilder actionBuilder)
        {
            this.module = module;
            this.route = route;
            this.actionBuilder = actionBuilder;
        }

        public dynamic Handle(dynamic parameters)
        {
            var model = module.BindAndValidate<TModel>();

            if (!module.ModelValidationResult.IsValid)
            {
                if (onValidationError == null)
                {
                    throw new NotImplementedException("Module routing definition is missing validation error handling");
                }

                return onValidationError.Handle(module);
            }

            return Handle(module, model);
        }

        public dynamic Handle(NancyStackModule module, dynamic model)
        {
            if (withAuthentication)
            {
                module.RequiresAuthentication();
            }

            if (claims != null)
            {
                module.RequiresClaims(claims.Select(x => x.ToString()));
            }

            var result = NancyStackWiring.HandlerRegistry.Execute<TModel, TReturnModel>(model);

            foreach (var on in onHandlers)
            {
                if (on.Condition(result)) return on.HandleResult(result);
            }

            return onSuccess(result);
        }

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> On(Func<TReturnModel, bool> on, Func<OnScenarioContext<TReturnModel>, dynamic> result)
        {
            onHandlers.Add(new OnHandler<TReturnModel>(on, result));
            return this;
        }

        public void OnSuccess(Func<OnScenarioContext<TReturnModel>, dynamic> result)
        {
            //    OnSuccess((ctx, model) => result(model));
            //}

            //public void OnSuccess(Func< OnScenarioContext< NancyContext TReturnModel, dynamic> result)
            //{
            var modelType = typeof(TModel);

            onSuccess = result;

            UrlRoute.Instance.Register(modelType, route);
            actionBuilder[route] = this.Handle;

            if (!ValidationRouteHandlers.Default.ContainsKey(modelType))
            {
                ValidationRouteHandlers.Default.Add(modelType, this.Handle);
            }

            //make sure instance exists
            //try
            //{
            //    NancyStackWiring.HandlerRegister.GetHandlerFor<TModel, TReturnModel>();
            //}
            //catch (Exception ex)
            //{
            //    throw new HandlerMissingException(route, typeof(TModel), typeof(TReturnModel), ex);
            //}
        }

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>(Func<OnScenarioContext<TValidationModel>, dynamic> validationError)
        {
            this.onValidationError = new ValidationErrorHandler<TValidationModel>(validationError);
            return this;
        }

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>()
        {
            this.onValidationError = new ValidationErrorHandler<TValidationModel>((x) => ValidationRouteHandlers.Default[typeof(TValidationModel)]((x as IModuleHolder).Module));
            return this;
        }

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> WithAuthentication()
        {
            withAuthentication = true;
            return this;
        }

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> WithRoles(params string[] roles)
        {
            claims = new List<string>();
            claims.AddRange(roles);
            return this;
        }

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> WithRoles(IEnumerable<string> roles)
        {
            claims = roles.ToList();
            return this;
        }
    }

    public class RouteHandlerBuilder<TModel> : IRouteHandlerBuilder<TModel>
    {
        private readonly NancyModule.RouteBuilder actionBuilder;
        private readonly NancyStackModule module;
        private readonly string route;

        public RouteHandlerBuilder(NancyStackModule module, string route, NancyModule.RouteBuilder actionBuilder)
        {
            this.route = route;
            this.module = module;
            this.actionBuilder = actionBuilder;
        }

        public IReturingWithAuthRouteHandlerBuilder<TModel, TReturnModel> Returning<TReturnModel>()
        {
            return new ReturningRouteHandlerBuilder<TModel, TReturnModel>(module, route, actionBuilder);
        }
    }

    public interface IModuleHolder
    {
        NancyStackModule Module { get; }
    }

    public class OnScenarioContext<TModel> : IModuleHolder
    {
        private readonly NancyStackModule module;
        public TModel Model { get; private set; }

        public OnScenarioContext(NancyStackModule module, TModel model)
        {
            this.module = module;
            this.Model = model;
        }

        public dynamic RenderView(string view)
        {
            return module.View[view, Model];
        }

        public NancyStackModule Module
        {
            get
            {
                return module;
            }
        }
    }
}