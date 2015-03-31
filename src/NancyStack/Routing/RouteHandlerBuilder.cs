using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using NancyStack.Configuration;
using NancyStack.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    public class OnHandler<TModel>
    {
        public OnHandler(Func<TModel, bool> on, Func<TModel, dynamic> result)
        {
            this.Condition = on;
            this.HandleResult = result;
        }

        public Func<TModel, bool> Condition { get; private set; }
        public Func<TModel, dynamic> HandleResult { get; private set; }
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
        private Func<NancyContext, TReturnModel, dynamic> onSuccess;
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
                    throw new NotImplementedException("Validation not implemented");
                }

                return onValidationError.Handle(parameters);
            }

            if (withAuthentication)
            {
                module.RequiresAuthentication();
            }

            if (claims != null)
            {
                module.RequiresClaims(claims.Select(x => x.ToString()));
            }

            //files do not get bound automatically in nancy - do this here manually
            if (module.Request.Files.Count() > 0)
            {
                var fileProperties = model.GetType().GetProperties().Where(x => x.PropertyType == typeof(HttpFile));
                fileProperties.Each(x => x.SetValue(model, module.Request.Files.FirstOrDefault(v => v.Key == x.Name), null));
            }

            var result = NancyStackWiring.HandlerRegistry.Execute<TModel, TReturnModel>(model);

            foreach (var on in onHandlers)
            {
                if (on.Condition(result)) return on.HandleResult(result);
            }

            return onSuccess(this.module.Context, result);
        }

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> On(Func<TReturnModel, bool> on, Func<TReturnModel, dynamic> result)
        {
            onHandlers.Add(new OnHandler<TReturnModel>(on, result));
            return this;
        }

        public void OnSuccess(Func<TReturnModel, dynamic> result)
        {
            OnSuccess((ctx, model) => result(model));
        }

        public void OnSuccess(Func<NancyContext, TReturnModel, dynamic> result)
        {
            onSuccess = result;

            UrlRoute.Instance.Register(typeof(TModel), route);
            actionBuilder[route] = this.Handle;

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

        public IReturningRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>(Func<TValidationModel, dynamic> validationError)
        {
            this.onValidationError = new ValidationErrorHandler<TValidationModel>(module, validationError);
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

    public class ValidationErrorHandler<TValidationModel> : IValidationErrorHandler
    {
        private readonly NancyStackModule module;
        private readonly Func<TValidationModel, dynamic> onError;

        public ValidationErrorHandler(NancyStackModule module, Func<TValidationModel, dynamic> onError)
        {
            this.module = module;
            this.onError = onError;
        }

        public dynamic Handle(dynamic model)
        {
            return onError(module.Bind<TValidationModel>());
        }
    }
}