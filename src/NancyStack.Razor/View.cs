using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Razor
{
    public abstract class View<TViewModel> : NancyRazorViewBase<TViewModel>
    {
    }
}
