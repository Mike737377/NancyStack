using Nancy;
using NancyStack.Modules;
using NancyStack.Routing.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    public interface IRouteHandlerBuilder<TModel>
    {
        IAuthRouteHandlerBuilder<TModel, TReturnModel> Returning<TReturnModel>();
    }

    public interface IAuthRouteHandlerBuilder<TModel, TReturnModel> : IValidationRouteHandlerBuilder<TModel, TReturnModel>
    {
        IValidationRouteHandlerBuilder<TModel, TReturnModel> WithAuthentication();

        IValidationRouteHandlerBuilder<TModel, TReturnModel> WithRoles(params string[] roles);

        IValidationRouteHandlerBuilder<TModel, TReturnModel> WithRoles(IEnumerable<string> roles);
    }

    public interface IValidationRouteHandlerBuilder<TModel, TReturnModel> : IOnRouteHandlerBuilder<TModel, TReturnModel>
    {
        IOnRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>(Func<OnScenarioContext<TValidationModel>, dynamic> specificValidationError);

        IOnRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>();
    }

    public interface IOnRouteHandlerBuilder<TModel, TReturnModel>
    {
        IOnRouteHandlerBuilder<TModel, TReturnModel> On(Func<TReturnModel, bool> on, Func<OnScenarioContext<TReturnModel>, dynamic> result);

        void OnSuccess(Func<OnScenarioContext<TReturnModel>, dynamic> result);
    }



    public interface IValidationErrorHandler
    {
        dynamic Handle(INancyModule module);
    }
}