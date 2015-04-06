using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing.Handlers
{
    public class OnScenarioContext<TModel> : IModuleHolder
    {
        private readonly INancyModule module;
        public TModel Model { get; private set; }

        public OnScenarioContext(INancyModule module, TModel model)
        {
            this.module = module;
            this.Model = model;
        }

        public dynamic RenderView(string view)
        {
            return module.View[view, Model];
        }

        INancyModule IModuleHolder.Module
        {
            get
            {
                return module;
            }
        }
    }
}
