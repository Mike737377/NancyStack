using Nancy;
using NancyStack.ModelBinding;
using NancyStack.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing.Handlers
{
    public class ValidationErrorHandler<TValidationModel> : IValidationErrorHandler
    {
        private readonly Func<OnScenarioContext<TValidationModel>, dynamic> onError;

        public ValidationErrorHandler(Func<OnScenarioContext<TValidationModel>, dynamic> onError)
        {
            this.onError = onError;
        }

        public dynamic Handle(INancyModule module)
        {
            return onError(new OnScenarioContext<TValidationModel>(module, module.Bind<TValidationModel>()));
        }
    }
}