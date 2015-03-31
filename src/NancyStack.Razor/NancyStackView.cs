using Nancy.ViewEngines.Razor;
using NancyStack.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Razor
{
    public abstract class NancyStackView<TViewModel> : NancyRazorViewBase<TViewModel>
    {
        public string UrlFor<TModel>()
        {
            return UrlRoute.For<TModel>();
        }

        public string UrlFor<TModel>(TModel model)
        {
            return UrlRoute.For(model);
        }
    }
}