using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing.Handlers
{
    public class OnConditionHandler<TModel>
    {
        public OnConditionHandler(Func<TModel, bool> on, Func<OnScenarioContext<TModel>, dynamic> result)
        {
            this.Condition = on;
            this.HandleResult = result;
        }

        public Func<TModel, bool> Condition { get; private set; }
        public Func<OnScenarioContext<TModel>, dynamic> HandleResult { get; private set; }
    }
}