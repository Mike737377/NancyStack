﻿using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    public interface IReturingWithAuthRouteHandlerBuilder<TModel, TReturnModel> : IReturningRouteHandlerBuilder<TModel, TReturnModel>
    {
        IReturningRouteHandlerBuilder<TModel, TReturnModel> WithAuthentication();

        IReturningRouteHandlerBuilder<TModel, TReturnModel> WithRoles(params string[] roles);

        IReturningRouteHandlerBuilder<TModel, TReturnModel> WithRoles(IEnumerable<string> roles);
    }

    public interface IReturningRouteHandlerBuilder<TModel, TReturnModel>
    {
        IReturningRouteHandlerBuilder<TModel, TReturnModel> On(Func<TReturnModel, bool> on, Func<TReturnModel, dynamic> result);

        void OnSuccess(Func<TReturnModel, dynamic> result);

        void OnSuccess(Func<NancyContext, TReturnModel, dynamic> result);

        IReturningRouteHandlerBuilder<TModel, TReturnModel> OnValidationError<TValidationModel>(Func<TValidationModel, dynamic> specificValidationError);
    }

    public interface IRouteHandlerBuilder<TModel>
    {
        IReturingWithAuthRouteHandlerBuilder<TModel, TReturnModel> Returning<TReturnModel>();
    }

    public interface IValidationErrorHandler
    {
        dynamic Handle(dynamic model);
    }
}