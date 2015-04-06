using Nancy;
using Nancy.Security;
using NancyStack.Configuration;
using NancyStack.Handlers;
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
        internal static readonly Dictionary<Type, Func<INancyModule, dynamic>> Default = new Dictionary<Type, Func<INancyModule, dynamic>>();
    }

    public class RouteHandlerContext
    {
        public INancyModule NancyModule { get; set; }
        public IModelBinder ModelBinder { get; set; }
        public IHandlerRegistry HandlerRegistry { get; set; }
    }

    public class NancyRouteRegister : INancyRouteRegister
    {
        private readonly NancyModule.RouteBuilder actionBuilder;

        public NancyRouteRegister(NancyModule.RouteBuilder actionBuilder)
        {
            this.actionBuilder = actionBuilder;
        }

        public void Register(string path, Func<dynamic, dynamic> routeInvoker)
        {
            actionBuilder[path] = routeInvoker;
        }
    }

    public interface INancyRouteRegister
    {
        void Register(string path, Func<dynamic,dynamic> routeInvoker);
    }


    public class RouteHandlerBuilder<TModel> : IRouteHandlerBuilder<TModel>
    {
        private readonly INancyRouteRegister routeRegister;
        private readonly string route;
        private readonly RouteHandlerContext context;

        public RouteHandlerBuilder(RouteHandlerContext context, string route, INancyRouteRegister routeRegister)
        {
            this.route = route;
            this.routeRegister = routeRegister;
            this.context = context;
        }

        public IAuthRouteHandlerBuilder<TModel, TReturnModel> Returning<TReturnModel>()
        {
            return new ReturningRouteHandlerBuilder<TModel, TReturnModel>(context, route, routeRegister);
        }
    }

    public class ReturningRouteHandlerBuilder<TModel, TReturnModel> :
        IAuthRouteHandlerBuilder<TModel, TReturnModel>,
        IValidationRouteHandlerBuilder<TModel, TReturnModel>,
        IOnRouteHandlerBuilder<TModel, TReturnModel>
    {
        private readonly INancyRouteRegister routeRegister;
        private readonly string route;
        private List<string> claims;
        private List<OnConditionHandler<TReturnModel>> onHandlers = new List<OnConditionHandler<TReturnModel>>();
        private Func<OnScenarioContext<TReturnModel>, dynamic> onSuccess;
        private IValidationErrorHandler onValidationError;
        private bool withAuthentication;
        private readonly RouteHandlerContext context;

        public ReturningRouteHandlerBuilder(RouteHandlerContext context, string route, INancyRouteRegister routeRegister)
        {
            this.context = context;
            this.route = route;
            this.routeRegister = routeRegister;
        }

        public dynamic Handle(dynamic parameters)
        {
            var model = context.ModelBinder.BindAndValidate<TModel>(context.NancyModule);

            if (!context.NancyModule.ModelValidationResult.IsValid)
            {
                if (onValidationError == null)
                {
                    throw new NotImplementedException("Module routing definition is missing validation error handling");
                }

                return onValidationError.Handle(context.NancyModule);
            }

            return Handle(context.NancyModule, model);
        }

        public dynamic Handle(INancyModule module, dynamic model)
        {
            if (withAuthentication)
            {
                module.RequiresAuthentication();
            }

            if (claims != null)
            {
                module.RequiresClaims(claims.Select(x => x.ToString()));
            }

            var result = context.HandlerRegistry.Execute<TModel, TReturnModel>(model);

            foreach (var on in onHandlers)
            {
                if (on.Condition(result)) return on.HandleResult(result);
            }

            return onSuccess(result);
        }


        #region ValidationScenarios

        public IOnRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>(Func<OnScenarioContext<TValidationModel>, dynamic> validationError)
        {
            this.onValidationError = new ValidationErrorHandler<TValidationModel>(validationError);
            return this;
        }

        public IOnRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>()
        {
            this.onValidationError = new ValidationErrorHandler<TValidationModel>((x) => ValidationRouteHandlers.Default[typeof(TValidationModel)]((x as IModuleHolder).Module));
            return this;
        }

        #endregion

        #region AuthScenarios

        public IValidationRouteHandlerBuilder<TModel, TReturnModel> WithAuthentication()
        {
            withAuthentication = true;
            return this;
        }

        public IValidationRouteHandlerBuilder<TModel, TReturnModel> WithRoles(params string[] roles)
        {
            claims = new List<string>();
            claims.AddRange(roles);
            return this;
        }

        public IValidationRouteHandlerBuilder<TModel, TReturnModel> WithRoles(IEnumerable<string> roles)
        {
            claims = roles.ToList();
            return this;
        }

        #endregion
        
        #region OnScenarios

        public IOnRouteHandlerBuilder<TModel, TReturnModel> On(Func<TReturnModel, bool> on, Func<OnScenarioContext<TReturnModel>, dynamic> result)
        {
            onHandlers.Add(new OnConditionHandler<TReturnModel>(on, result));
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
            routeRegister.Register(route, this.Handle);

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

        #endregion

    }

    public interface IModuleHolder
    {
        INancyModule Module { get; }
    }

}