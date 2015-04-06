using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing.Handlers
{
    public class OnSuccessHandler<TModel>
    {
        public OnSuccessHandler(Func<OnScenarioContext<TModel>, dynamic> result)
        {
            this.HandleResult = result;
        }

        public Func<OnScenarioContext<TModel>, dynamic> HandleResult { get; private set; }
    }
}
